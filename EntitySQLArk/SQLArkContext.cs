namespace EntitySQLArk;

public partial class SQLArkContext : DbContext
{
    public SQLArkContext() { }

    public SQLArkContext(DbContextOptions<SQLArkContext> options) : base(options) { }

    public virtual DbSet<ActiveService>? ActiveServices {get;set;}

    public virtual DbSet<Element>?       Elements       {get;set;}

    public virtual DbSet<MultiMedia>?    MediaLibrary   {get;set;}

    public virtual DbSet<Note>?          Notes          {get;set;}

    public virtual DbSet<Tag>?           Tags           {get;set;}

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseSqlServer("Data Source=localhost\\KusDepot;Initial Catalog=SQLArk;Integrated Security=True;TrustServerCertificate=True");
    }
}