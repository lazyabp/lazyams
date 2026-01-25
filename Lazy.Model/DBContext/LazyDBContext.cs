namespace Lazy.Model.DBContext;

public class LazyDBContext : DbContext
{
    public LazyDBContext(DbContextOptions<LazyDBContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<Carousel> Carousels { get; set; }
    public DbSet<RoleMenu> RoleMenus { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Lazy.Model.Entity.File> Files { get; set; }
    public DbSet<Config> Configs { get; set; }
    public DbSet<SocialiteUser> SocialiteUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //extension method
        modelBuilder.ConfigureAdminManagement();
    }
}
