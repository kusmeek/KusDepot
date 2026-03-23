namespace KusDepot.CatalogDb;

public sealed class CatalogDBContext : DbContext
{
    public CatalogDBContext(DbContextOptions<CatalogDBContext> options) : base(options) {}

    public DbSet<Element>    Elements     => Set<Element>();
    public DbSet<Command>    Commands     => Set<Command>();
    public DbSet<Service>    Services     => Set<Service>();
    public DbSet<MultiMedia> MediaLibrary => Set<MultiMedia>();
    public DbSet<Note>       Notes        => Set<Note>();
    public DbSet<Tag>        Tags         => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Element>().ToTable("Elements");
        mb.Entity<Command>().ToTable("Commands");
        mb.Entity<Service>().ToTable("Services");
        mb.Entity<MultiMedia>().ToTable("MediaLibrary");
        mb.Entity<Note>().ToTable("Notes");
        mb.Entity<Tag>().ToTable("Tags");

        mb.Entity<Element>(e =>
        {
            e.HasKey(x => x.ID); e.Property(x => x.ID).ValueGeneratedNever();

            e.HasMany<Command>()
                .WithOne()
                .HasForeignKey(c => c.ID)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany<Service>()
                .WithOne()
                .HasForeignKey(s => s.ID)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne<MultiMedia>()
                .WithOne()
                .HasForeignKey<MultiMedia>(m => m.ID)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany<Note>()
                .WithOne()
                .HasForeignKey(n => n.ID)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany<Tag>()
                .WithOne()
                .HasForeignKey(t => t.ID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<Command>(c =>
        {
            c.HasKey(x => x.PrimaryKey);
        });

        mb.Entity<Service>(s =>
        {
            s.HasKey(x => x.PrimaryKey);
        });

        mb.Entity<MultiMedia>(m =>
        {
            m.HasKey(x => x.ID); m.Property(x => x.ID).ValueGeneratedNever();
        });

        mb.Entity<Note>(n =>
        {
            n.HasKey(x => x.PrimaryKey);
            n.HasIndex(x => x.Value);
            n.Property(x => x.Value).IsRequired();
            n.HasIndex(x => new { x.ID , x.Value }).IsUnique();
        });

        mb.Entity<Tag>(t =>
        {
            t.HasKey(x => x.PrimaryKey);
            t.HasIndex(x => x.Value);
            t.Property(x => x.Value).IsRequired();
            t.HasIndex(x => new { x.ID , x.Value }).IsUnique();
        });
    }
}