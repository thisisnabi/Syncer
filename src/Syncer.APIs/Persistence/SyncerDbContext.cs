using Microsoft.EntityFrameworkCore;
using Syncer.APIs.Models.Domain;

namespace Syncer.APIs.Persistence;

public class SyncerDbContext(DbContextOptions<SyncerDbContext> dbContextOptions) : DbContext(dbContextOptions)
{
    public const string ConnectionStringName = "SvcDbContext";

    public DbSet<Presentation> Presentations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigurePresentation(modelBuilder);
        ConfigureEmoji(modelBuilder);
    }

    private static void ConfigureEmoji(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Emoji>().HasKey(x => x.Code);
        modelBuilder.Entity<Emoji>().Property(x => x.Code)
                                    .ValueGeneratedNever()
                                    .IsUnicode(false);

        modelBuilder.Entity<Emoji>().Property(x => x.ShortName)
                                    .HasMaxLength(100)
                                    .IsUnicode(false);
    }

    private static void ConfigurePresentation(ModelBuilder modelBuilder)
    {
        var presentation = modelBuilder.Entity<Presentation>();

        presentation.HasKey(x => x.Id);

        presentation.Property(x => x.Id)
                    .ValueGeneratedNever()
                    .IsUnicode(false);

        presentation.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode();

        presentation.Property(x => x.Description)
                    .IsRequired()
                    .HasMaxLength(2000)
                    .IsUnicode();

        presentation.Property(x => x.Speaker)
                    .IsRequired()
                    .IsUnicode(false);

        presentation.Property(x => x.Status).IsRequired();

        presentation.OwnsMany(x => x.Milestones, milestoneBuilder =>
        {
            milestoneBuilder.HasKey(x => x.Id);

            milestoneBuilder.Property(x => x.Status).IsRequired();
            milestoneBuilder.Property(x => x.PresentationId).IsRequired();

            milestoneBuilder.Property(x => x.Title).IsRequired().HasMaxLength(100).IsUnicode();
            milestoneBuilder.Property(x => x.Description).IsRequired().HasMaxLength(500).IsUnicode();

            milestoneBuilder.OwnsMany(x => x.Emojis, emojisBuilder =>
            {
                emojisBuilder.ToJson();
            });

            milestoneBuilder.OwnsMany(x => x.Reactions, rectionsBuilder =>
            {
                rectionsBuilder.ToJson();
            });
        });
    }
}
