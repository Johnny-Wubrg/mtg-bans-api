using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MtgBans.Data;
using MtgBans.Data.Entities;
using MtgBans.Models.Formats;

namespace MtgBans.Services.Services;

public interface IFormatService
{
  Task<IEnumerable<FormatSummary>> GetAll(CancellationToken cancellationToken = default);
  Task<FormatDetail> GetById(int id, CancellationToken cancellationToken = default);
  Task<FormatDetail> GetBySlug(string slug, CancellationToken cancellationToken = default);
}

public class FormatService : IFormatService
{
  private readonly MtgBansContext _context;

  public FormatService(MtgBansContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<FormatSummary>> GetAll(CancellationToken cancellationToken = default)
  {
    var formats = await _context.Formats
      .OrderBy(f => f.DisplayOrder)
      .Include(f => f.Events)
      .ToListAsync(cancellationToken);
    return formats.Select(EntityToModel);
  }

  public Task<FormatDetail> GetById(int id, CancellationToken cancellationToken = default) =>
    Get(f => f.Id == id, cancellationToken);

  public Task<FormatDetail> GetBySlug(string slug, CancellationToken cancellationToken = default) =>
    Get(f => f.Slug == slug, cancellationToken);

  private async Task<FormatDetail> Get(Expression<Func<Format, bool>> expression,
    CancellationToken cancellationToken)
  {
    var format = await _context.Formats.Include(f => f.Events).FirstOrDefaultAsync(expression, cancellationToken);
    var limitationStatuses = await _context.CardLegalityStatuses.Where(e => e.Type == CardLegalityStatusType.Limitation).OrderBy(s => s.DisplayOrder).ToListAsync(cancellationToken);
    if (format is null) return null;

    var cards = await _context.Cards
      .Include(c => c.CanonicalPrinting)
      .Include(c => c.LegalityEvents).ThenInclude(e => e.Status)
      .Include(c => c.Classifications)
      .AsSplitQuery()
      .Where(c => c.LegalityEvents.Any(e => e.FormatId == format.Id))
      .ToListAsync(cancellationToken);
    var formatDetail = EntityToDetailModel(format);
    formatDetail.Timeline = CreateTimeline(format.Id, cards, limitationStatuses);
    return formatDetail;
  }

  private IEnumerable<FormatSnapshotDetail> CreateTimeline(int formatId, List<Card> cards,
    List<CardLegalityStatus> statuses)
  {
    return cards
      .SelectMany(c => c.LegalityEvents.Select(e => (Card: c, Event: e)))
      .Where(t => t.Event.FormatId == formatId)
      .GroupBy(t => t.Event.DateEffective)
      .OrderBy(g => g.Key)
      .Select(grp => new FormatSnapshotDetail
      {
        Date = grp.Key,
        Limitations = CardService.GetLimitations(grp.Key, cards, formatId)
      })
      .ToList();
  }

  public static FormatSummary EntityToModel(Format format)
  {
    var model = new FormatSummary();
    FillBaseModel(format, model);
    return model;
  }

  public static FormatDetail EntityToDetailModel(Format format)
  {
    var model = new FormatDetail();
    FillBaseModel(format, model);
    model.Events = format.Events.OrderBy(e => e.DateEffective).Select(e => new FormatEventDetail
    {
      NameUpdate = e.NameUpdate,
      DateEffective = e.DateEffective,
      Description = e.Description
    });

    return model;
  }

  private static void FillBaseModel(Format format, FormatSummary summary)
  {
    summary.Id = format.Id;
    summary.Name = format.Name;
    summary.Slug = format.Slug;
  }
}
