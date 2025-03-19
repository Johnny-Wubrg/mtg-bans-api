﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MtgBans.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MtgBans.Data.Migrations
{
    [DbContext(typeof(MtgBansContext))]
    [Migration("20250313191912_CardLegalityStatusCleanup")]
    partial class CardLegalityStatusCleanup
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CardClassification", b =>
                {
                    b.Property<Guid>("CardsScryfallId")
                        .HasColumnType("uuid")
                        .HasColumnName("cards_scryfall_id");

                    b.Property<int>("ClassificationsId")
                        .HasColumnType("integer")
                        .HasColumnName("classifications_id");

                    b.HasKey("CardsScryfallId", "ClassificationsId")
                        .HasName("pk_card_classification");

                    b.HasIndex("ClassificationsId")
                        .HasDatabaseName("ix_card_classification_classifications_id");

                    b.ToTable("card_classification", (string)null);
                });

            modelBuilder.Entity("MtgBans.Data.Entities.Announcement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("DateAnnounced")
                        .HasColumnType("date")
                        .HasColumnName("date_announced");

                    b.Property<DateOnly>("DateEffective")
                        .HasColumnType("date")
                        .HasColumnName("date_effective");

                    b.Property<string[]>("Sources")
                        .HasColumnType("text[]")
                        .HasColumnName("sources");

                    b.Property<string>("Summary")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("summary");

                    b.HasKey("Id")
                        .HasName("pk_announcements");

                    b.ToTable("announcements", (string)null);
                });

            modelBuilder.Entity("MtgBans.Data.Entities.Card", b =>
                {
                    b.Property<Guid>("ScryfallId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("scryfall_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnName("name");

                    b.Property<string>("ScryfallImageUri")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("scryfall_image_uri");

                    b.Property<string>("ScryfallUri")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("scryfall_uri");

                    b.Property<string>("SortName")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnName("sort_name");

                    b.HasKey("ScryfallId")
                        .HasName("pk_cards");

                    b.ToTable("cards", (string)null);
                });

            modelBuilder.Entity("MtgBans.Data.Entities.CardAlias", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<Guid>("CardScryfallId")
                        .HasColumnType("uuid")
                        .HasColumnName("card_scryfall_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_card_aliases");

                    b.HasIndex("CardScryfallId")
                        .HasDatabaseName("ix_card_aliases_card_scryfall_id");

                    b.ToTable("card_aliases", (string)null);
                });

            modelBuilder.Entity("MtgBans.Data.Entities.CardLegalityEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("AnnouncementId")
                        .HasColumnType("integer")
                        .HasColumnName("announcement_id");

                    b.Property<Guid>("CardScryfallId")
                        .HasColumnType("uuid")
                        .HasColumnName("card_scryfall_id");

                    b.Property<DateOnly>("DateEffective")
                        .HasColumnType("date")
                        .HasColumnName("date_effective");

                    b.Property<int?>("FormatId")
                        .HasColumnType("integer")
                        .HasColumnName("format_id");

                    b.Property<int>("StatusId")
                        .HasColumnType("integer")
                        .HasColumnName("status_id");

                    b.HasKey("Id")
                        .HasName("pk_card_legality_event");

                    b.HasIndex("AnnouncementId")
                        .HasDatabaseName("ix_card_legality_event_announcement_id");

                    b.HasIndex("CardScryfallId")
                        .HasDatabaseName("ix_card_legality_event_card_scryfall_id");

                    b.HasIndex("FormatId")
                        .HasDatabaseName("ix_card_legality_event_format_id");

                    b.HasIndex("StatusId")
                        .HasDatabaseName("ix_card_legality_event_status_id");

                    b.ToTable("card_legality_event", (string)null);
                });

            modelBuilder.Entity("MtgBans.Data.Entities.CardLegalityStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("integer")
                        .HasColumnName("display_order");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("label");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("pk_card_legality_statuses");

                    b.ToTable("card_legality_statuses", (string)null);
                });

            modelBuilder.Entity("MtgBans.Data.Entities.Classification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("DateApplied")
                        .HasColumnType("date")
                        .HasColumnName("date_applied");

                    b.Property<DateOnly?>("DateLifted")
                        .HasColumnType("date")
                        .HasColumnName("date_lifted");

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("integer")
                        .HasColumnName("display_order");

                    b.Property<string>("Summary")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("summary");

                    b.HasKey("Id")
                        .HasName("pk_classification");

                    b.ToTable("classification", (string)null);
                });

            modelBuilder.Entity("MtgBans.Data.Entities.Expansion", b =>
                {
                    b.Property<Guid>("ScryfallId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("scryfall_id");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("character varying(8)")
                        .HasColumnName("code");

                    b.Property<DateOnly>("DateReleased")
                        .HasColumnType("date")
                        .HasColumnName("date_released");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("name");

                    b.Property<string>("ScryfallUri")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("scryfall_uri");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("type");

                    b.HasKey("ScryfallId")
                        .HasName("pk_expansions");

                    b.ToTable("expansions", (string)null);
                });

            modelBuilder.Entity("MtgBans.Data.Entities.ExpansionLegality", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("DateEntered")
                        .HasColumnType("date")
                        .HasColumnName("date_entered");

                    b.Property<DateOnly?>("DateExited")
                        .HasColumnType("date")
                        .HasColumnName("date_exited");

                    b.Property<Guid?>("ExpansionScryfallId")
                        .HasColumnType("uuid")
                        .HasColumnName("expansion_scryfall_id");

                    b.Property<int>("FormatId")
                        .HasColumnType("integer")
                        .HasColumnName("format_id");

                    b.HasKey("Id")
                        .HasName("pk_expansion_legality");

                    b.HasIndex("ExpansionScryfallId")
                        .HasDatabaseName("ix_expansion_legality_expansion_scryfall_id");

                    b.HasIndex("FormatId")
                        .HasDatabaseName("ix_expansion_legality_format_id");

                    b.ToTable("expansion_legality", (string)null);
                });

            modelBuilder.Entity("MtgBans.Data.Entities.Format", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("integer")
                        .HasColumnName("display_order");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<bool>("IsCoreOnly")
                        .HasColumnType("boolean")
                        .HasColumnName("is_core_only");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_formats");

                    b.ToTable("formats", (string)null);
                });

            modelBuilder.Entity("MtgBans.Data.Entities.FormatEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("AnnouncementId")
                        .HasColumnType("integer")
                        .HasColumnName("announcement_id");

                    b.Property<DateOnly>("DateEffective")
                        .HasColumnType("date")
                        .HasColumnName("date_effective");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("description");

                    b.Property<int?>("FormatId")
                        .HasColumnType("integer")
                        .HasColumnName("format_id");

                    b.Property<string>("NameUpdate")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("name_update");

                    b.HasKey("Id")
                        .HasName("pk_format_event");

                    b.HasIndex("AnnouncementId")
                        .HasDatabaseName("ix_format_event_announcement_id");

                    b.HasIndex("FormatId")
                        .HasDatabaseName("ix_format_event_format_id");

                    b.ToTable("format_event", (string)null);
                });

            modelBuilder.Entity("MtgBans.Data.Entities.Printing", b =>
                {
                    b.Property<Guid>("ScryfallId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("scryfall_id");

                    b.Property<Guid>("CardScryfallId")
                        .HasColumnType("uuid")
                        .HasColumnName("card_scryfall_id");

                    b.Property<Guid>("ExpansionScryfallId")
                        .HasColumnType("uuid")
                        .HasColumnName("expansion_scryfall_id");

                    b.HasKey("ScryfallId")
                        .HasName("pk_printings");

                    b.HasIndex("CardScryfallId")
                        .HasDatabaseName("ix_printings_card_scryfall_id");

                    b.HasIndex("ExpansionScryfallId")
                        .HasDatabaseName("ix_printings_expansion_scryfall_id");

                    b.ToTable("printings", (string)null);
                });

            modelBuilder.Entity("CardClassification", b =>
                {
                    b.HasOne("MtgBans.Data.Entities.Card", null)
                        .WithMany()
                        .HasForeignKey("CardsScryfallId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_card_classification_cards_cards_scryfall_id");

                    b.HasOne("MtgBans.Data.Entities.Classification", null)
                        .WithMany()
                        .HasForeignKey("ClassificationsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_card_classification_classification_classifications_id");
                });

            modelBuilder.Entity("MtgBans.Data.Entities.CardAlias", b =>
                {
                    b.HasOne("MtgBans.Data.Entities.Card", "Card")
                        .WithMany("Aliases")
                        .HasForeignKey("CardScryfallId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_card_aliases_cards_card_scryfall_id");

                    b.Navigation("Card");
                });

            modelBuilder.Entity("MtgBans.Data.Entities.CardLegalityEvent", b =>
                {
                    b.HasOne("MtgBans.Data.Entities.Announcement", null)
                        .WithMany("Changes")
                        .HasForeignKey("AnnouncementId")
                        .HasConstraintName("fk_card_legality_event_announcements_announcement_id");

                    b.HasOne("MtgBans.Data.Entities.Card", "Card")
                        .WithMany("LegalityEvents")
                        .HasForeignKey("CardScryfallId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_card_legality_event_cards_card_scryfall_id");

                    b.HasOne("MtgBans.Data.Entities.Format", "Format")
                        .WithMany()
                        .HasForeignKey("FormatId")
                        .HasConstraintName("fk_card_legality_event_formats_format_id");

                    b.HasOne("MtgBans.Data.Entities.CardLegalityStatus", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_card_legality_event_card_legality_statuses_status_id");

                    b.Navigation("Card");

                    b.Navigation("Format");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("MtgBans.Data.Entities.ExpansionLegality", b =>
                {
                    b.HasOne("MtgBans.Data.Entities.Expansion", null)
                        .WithMany("Legalities")
                        .HasForeignKey("ExpansionScryfallId")
                        .HasConstraintName("fk_expansion_legality_expansions_expansion_scryfall_id");

                    b.HasOne("MtgBans.Data.Entities.Format", "Format")
                        .WithMany()
                        .HasForeignKey("FormatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_expansion_legality_formats_format_id");

                    b.Navigation("Format");
                });

            modelBuilder.Entity("MtgBans.Data.Entities.FormatEvent", b =>
                {
                    b.HasOne("MtgBans.Data.Entities.Announcement", "Announcement")
                        .WithMany()
                        .HasForeignKey("AnnouncementId")
                        .HasConstraintName("fk_format_event_announcements_announcement_id");

                    b.HasOne("MtgBans.Data.Entities.Format", null)
                        .WithMany("Events")
                        .HasForeignKey("FormatId")
                        .HasConstraintName("fk_format_event_formats_format_id");

                    b.Navigation("Announcement");
                });

            modelBuilder.Entity("MtgBans.Data.Entities.Printing", b =>
                {
                    b.HasOne("MtgBans.Data.Entities.Card", "Card")
                        .WithMany("Printings")
                        .HasForeignKey("CardScryfallId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_printings_cards_card_scryfall_id");

                    b.HasOne("MtgBans.Data.Entities.Expansion", "Expansion")
                        .WithMany("Cards")
                        .HasForeignKey("ExpansionScryfallId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_printings_expansions_expansion_scryfall_id");

                    b.Navigation("Card");

                    b.Navigation("Expansion");
                });

            modelBuilder.Entity("MtgBans.Data.Entities.Announcement", b =>
                {
                    b.Navigation("Changes");
                });

            modelBuilder.Entity("MtgBans.Data.Entities.Card", b =>
                {
                    b.Navigation("Aliases");

                    b.Navigation("LegalityEvents");

                    b.Navigation("Printings");
                });

            modelBuilder.Entity("MtgBans.Data.Entities.Expansion", b =>
                {
                    b.Navigation("Cards");

                    b.Navigation("Legalities");
                });

            modelBuilder.Entity("MtgBans.Data.Entities.Format", b =>
                {
                    b.Navigation("Events");
                });
#pragma warning restore 612, 618
        }
    }
}
