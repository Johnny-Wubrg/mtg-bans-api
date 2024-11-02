using Microsoft.EntityFrameworkCore;
using MtgBans.Data.Entities;

namespace MtgBans.Data;

public class MtgBansContext(DbContextOptions<MtgBansContext> options) : DbContext(options)
{
  public DbSet<Format> Formats { get; set; }
  public DbSet<Card> Cards { get; set; }
  public DbSet<Announcement> Announcements { get; set; }
  public DbSet<Expansion> Expansions { get; set; }
}