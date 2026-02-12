namespace Lazy.Model.DBContext;

public class LazyDBContext : DbContext
{
    public LazyDBContext(DbContextOptions<LazyDBContext> options) : base(options)
    {
    }

    #region BaseManagement

    public DbSet<User> Users { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Lazy.Model.Entity.File> Files { get; set; }
    public DbSet<Config> Configs { get; set; }
    public DbSet<SocialiteUser> SocialiteUsers { get; set; }
    public DbSet<AutoJob> AutoJobs { get; set; }
    public DbSet<AutoJobLog> AutoJobLogs { get; set; }

    #endregion

    #region BusinessManagement

    public DbSet<Package> Packages { get; set; }
    public DbSet<PackageFeature> PackageFeatures { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderLog> OrderLogs { get; set; }
    public DbSet<UserSubscription> UserSubscriptions { get; set; }

    #endregion

    public DbSet<Carousel> Carousels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //extension method
        modelBuilder.ConfigureBaseManagement();
        modelBuilder.ConfigureBusinessManagement();
    }
}
