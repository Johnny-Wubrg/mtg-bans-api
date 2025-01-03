using Microsoft.EntityFrameworkCore;
using MtgBans.Data;
using MtgBans.Data.Entities;
using MtgBans.Models.Formats;

namespace MtgBans.Services.Services;

public interface IFormatService
{
  Task<IEnumerable<FormatModel>> GetAll(CancellationToken cancellationToken = default);
  Task<FormatModel> GetById(int id, CancellationToken cancellationToken = default);
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

  public async Task<FormatModel> GetById(int id, CancellationToken cancellationToken = default)
  {
    var format = await _context.Formats.Include(f => f.Events).FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    return EntityToModel(format);
  }

  public static FormatModel EntityToModel(Format format)
  {
    return new FormatModel
    {
      Id = format.Id,
      Name = format.Name,
      Events = format.Events.Select(e => new FormatEventModel
      {
        NameUpdate = e.NameUpdate,
        DateEffective = e.DateEffective,
        Description = e.Description
      })
    };
  }
}