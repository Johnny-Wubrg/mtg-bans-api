using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MtgBans.Data.Entities;

public class PublicationArchive
{
  public Publication Publication { get; set; }

  [Key, ForeignKey(nameof(Publication))]
  public int PublicationId { get; set; }

  public string ContentMarkdown { get; set; }

  public string ContentHtml { get; set; }
}
