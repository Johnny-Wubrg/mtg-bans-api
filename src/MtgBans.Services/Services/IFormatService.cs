using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MtgBans.Data;
using MtgBans.Data.Entities;
using MtgBans.Models.Formats;

namespace MtgBans.Services.Services;

public interface IFormatService
{
  Task<IEnumerable<FormatModel>> GetAll(CancellationToken cancellationToken = default);
  Task<FormatDetailModel> GetById(int id, CancellationToken cancellationToken = default);
  Task<FormatDetailModel> GetBySlug(string slug, CancellationToken cancellationToken = default);
}

public class FormatService : IFormatService
{
  private readonly MtgBansContext _context;

  public FormatService(MtgBansContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<FormatModel>> GetAll(CancellationToken cancellationToken = default)
  {
    var formats = await _context.Formats
      .OrderBy(f => f.DisplayOrder)
      .Include(f => f.Events)
      .ToListAsync(cancellationToken);
    return formats.Select(EntityToModel);
  }

  public Task<FormatDetailModel> GetById(int id, CancellationToken cancellationToken = default) => Get(f => f.Id == id, cancellationToken);

  public Task<FormatDetailModel> GetBySlug(string slug, CancellationToken cancellationToken = default) => Get(f => f.Slug == slug, cancellationToken);

  private async Task<FormatDetailModel> Get(Expression<Func<Format, bool>> expression,
    CancellationToken cancellationToken)
  {
    var format = await _context.Formats.Include(f => f.Events).FirstOrDefaultAsync(expression, cancellationToken);
    return EntityToDetailModel(format);
  }

  public static FormatModel EntityToModel(Format format)
  {
    var model = new FormatModel();
    FillBaseModel(format, model);
    return model;
  }

  public static FormatDetailModel EntityToDetailModel(Format format)
  {
    var model = new FormatDetailModel();
    FillBaseModel(format, model);
    return model;
  }

  private static void FillBaseModel(Format format, FormatModel model)
  {
    model.Id = format.Id;
    model.Name = format.Name;
    model.Events = format.Events.OrderBy(e => e.DateEffective).Select(e => new FormatEventModel
    {
      NameUpdate = e.NameUpdate,
      DateEffective = e.DateEffective,
      Description = e.Description
    });
  }
}
