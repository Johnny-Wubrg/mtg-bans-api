using Microsoft.EntityFrameworkCore;
using MtgBans.Data.Entities;

namespace MtgBans.Data;

public class MtgBansContext(DbContextOptions<MtgBansContext> options) : DbContext(options)
{
  public DbSet<Format> Formats { get; set; }
  public DbSet<Card> Cards { get; set; }
  public DbSet<CardAlias> CardAliases { get; set; }
  public DbSet<CardLegalityStatus> CardLegalityStatuses { get; set; }
  public DbSet<Printing> Printings { get; set; }
  public DbSet<Announcement> Announcements { get; set; }
  public DbSet<Publication> Publications { get; set; }
  public DbSet<Expansion> Expansions { get; set; }
  public DbSet<CardLegalityRationale> CardLegalityRationales { get; set; }
  public DbSet<CardLegalityRationaleVote> CardLegalityRationaleVotes { get; set; }
  public DbSet<CardLegalityRationaleVoteNonce> CardLegalityRationaleVoteNonces { get; set; }
  public DbSet<CardNotorietyIndex> CardNotorietyIndices { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Card>()
      .HasOne(c => c.CanonicalPrinting)
      .WithMany()
      .HasForeignKey(c => c.CanonicalId)
      .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<Card>()
      .HasOne(c => c.NotorietyIndex)
      .WithOne(n => n.Card)
      .HasForeignKey<CardNotorietyIndex>(n => n.CardScryfallId);

    modelBuilder.Entity<FormatStatusWeight>()
      .Property(w => w.Weight)
      .HasColumnType("numeric(5,2)");

    modelBuilder.Entity<CardNotorietyIndex>()
      .Property(n => n.IndexValue)
      .HasColumnType("numeric(6,2)");

    modelBuilder.Entity<Classification>()
      .Property(c => c.NotorietyWeight)
      .HasColumnType("numeric(3,2)");

    modelBuilder.Entity<CardLegalityRationale>().ToTable("card_legality_rationale");
    modelBuilder.Entity<CardLegalityRationaleVote>().ToTable("card_legality_rationale_vote");

    base.OnModelCreating(modelBuilder);
  }
}