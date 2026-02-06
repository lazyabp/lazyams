using Lazy.Shared.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class LazyDbContextModelCreatingExtensions
{
    private const string TablePrefix = "";

    /// <summary>
    /// Admin
    /// </summary>
    /// <param name="modelBuilder"></param>
    public static void ConfigureAdminManagement(this ModelBuilder modelBuilder)
    {
        ConfigureUser(modelBuilder);
        ConfigureRole(modelBuilder);
        ConfigureMenu(modelBuilder);
        //ConfigureRoleMenu(modelBuilder);
        //ConfigureUserRole(modelBuilder);

        ConfigureConfig(modelBuilder);
        ConfigureSocialiteUser(modelBuilder);
        ConfigureFile(modelBuilder);
        ConfigureCarousel(modelBuilder);
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable(TablePrefix + "User");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.UserName).IsRequired().HasMaxLength(UserEntityConsts.MaxUserNameLength);
            b.Property(x => x.NickName).HasMaxLength(UserEntityConsts.MaxUserNameLength);
            b.Property(x => x.Password).HasMaxLength(UserEntityConsts.MaxPasswordLength);
            b.Property(x => x.Email).IsRequired().HasMaxLength(UserEntityConsts.MaxEmailLength);
            b.Property(x => x.Age).HasDefaultValue(0);
            //b.HasMany(x => x.UserRoles);
            b.Property(x => x.Avatar).HasMaxLength(UserEntityConsts.MaxAvatarLength);
            b.Property(x => x.Access).HasConversion(
                v => v.ToString(),
                v => (Access)Enum.Parse(typeof(Access), v)).HasMaxLength(EntityConsts.MaxLength32);
            b.Property(x => x.Gender).HasConversion(
                v => v.ToString(),
                v => (Gender)Enum.Parse(typeof(Gender), v)).HasMaxLength(EntityConsts.MaxLength32);
            b.Property(x => x.IsAdministrator).IsRequired().HasDefaultValue(false);
            b.Property(x => x.IsActive).IsRequired().HasDefaultValue(false);
            b.Property(x => x.Address).HasMaxLength(UserEntityConsts.MaxAddressLength);
            // b.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");//for SQLite
            b.ConfigureSoftDelete();
            b.HasMany(x => x.Roles).WithMany(x => x.Users).UsingEntity<UserRole>(
                TablePrefix + "UserRole",
                r => r.HasOne<Role>().WithMany().HasForeignKey(ur => ur.RoleId),
                l => l.HasOne<User>().WithMany().HasForeignKey(ur => ur.UserId),
                je =>
                {
                    je.HasKey(ur => new { ur.UserId, ur.RoleId });
                    je.Property(ur => ur.UserId).ValueGeneratedNever();
                    je.Property(ur => ur.RoleId).ValueGeneratedNever();
                }
            );
            b.HasIndex(x => new { x.IsDeleted, x.CreatedAt });
        });
    }

    private static void ConfigureRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(b =>
        {
            b.ToTable(TablePrefix + "Role");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.RoleName).IsRequired().HasMaxLength(RoleEntityConsts.MaxRoleNameLength);
            b.Property(x => x.Description).IsRequired().HasMaxLength(RoleEntityConsts.MaxDescriptionLength);
            b.Property(x => x.IsActive).IsRequired().HasDefaultValue(false);
            b.HasMany(x => x.Users).WithMany(x => x.Roles).UsingEntity<UserRole>(
                TablePrefix + "UserRole",
                r => r.HasOne<User>().WithMany().HasForeignKey(ur => ur.UserId),
                l => l.HasOne<Role>().WithMany().HasForeignKey(ur => ur.RoleId),
                je =>
                {
                    je.HasKey(ur => new { ur.UserId, ur.RoleId });
                    je.Property(ur => ur.UserId).ValueGeneratedNever();
                    je.Property(ur => ur.RoleId).ValueGeneratedNever();
                }
            );
            b.HasMany(x => x.Menus).WithMany(x => x.Roles).UsingEntity<RoleMenu>(
                TablePrefix + "RoleMenu",
                r => r.HasOne<Menu>().WithMany().HasForeignKey(rm => rm.MenuId),
                l => l.HasOne<Role>().WithMany().HasForeignKey(rm => rm.RoleId),
                je =>
                {
                    je.HasKey(rm => new { rm.RoleId, rm.MenuId });
                    je.Property(rm => rm.RoleId).ValueGeneratedNever();
                    je.Property(rm => rm.MenuId).ValueGeneratedNever();
                }
            );
            b.ConfigureSoftDelete();
        });
    }

    private static void ConfigureMenu(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Menu>(b =>
        {
            b.ToTable(TablePrefix + "Menu");
            b.HasKey(x => x.Id);
            b.Property(e => e.Id).ValueGeneratedNever();
            b.HasIndex(x => x.ParentId);
            b.Property(cs => cs.Name).IsRequired().HasMaxLength(MenuEntityConsts.MaxNameLength);
            b.Property(cs => cs.Title).IsRequired().HasMaxLength(MenuEntityConsts.MaxNameLength);
            b.Property(cs => cs.Icon).HasDefaultValue("el-icon-menu").HasMaxLength(MenuEntityConsts.MaxIconLength);
            b.Property(cs => cs.Permission).HasMaxLength(MenuEntityConsts.MaxPermissionLength);
            b.Property(cs => cs.Route).HasMaxLength(MenuEntityConsts.MaxRouteLength);
            b.Property(cs => cs.Component).HasMaxLength(MenuEntityConsts.MaxComponentLength);
            b.Property(cs => cs.MenuType).HasConversion(
               v => v.ToString(),
               v => (MenuType)Enum.Parse(typeof(MenuType), v)
            ).HasMaxLength(MenuEntityConsts.MaxMenuTypeLength);
            b.Property(x => x.IsActive).IsRequired().HasDefaultValue(false);
            //b.HasMany(cs => cs.RoleMenus);
            b.HasOne(cs => cs.Parent).WithMany(cs => cs.Children).HasForeignKey(cs => cs.ParentId);
            b.ConfigureSoftDelete();
            b.HasMany(x => x.Roles).WithMany(x => x.Menus);
            b.HasIndex(x => new { x.IsDeleted, x.MenuType, x.ParentId, x.CreatedAt });
        });
    }

    //private static void ConfigureRoleMenu(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<RoleMenu>(b =>
    //    {
    //        b.ToTable(TablePrefix + "RoleMenu");
    //        b.HasKey(x => x.Id);
    //        b.Property(x => x.Id).ValueGeneratedNever();
    //        b.HasKey(rm => new { rm.RoleId, rm.MenuId });
    //        //b.HasOne(rm => rm.Role)
    //        //  .WithMany(r => r.RoleMenus)
    //        //  .HasForeignKey(rm => rm.RoleId);
    //        //b.HasOne(rm => rm.Menu)
    //        // .WithMany(m => m.RoleMenus)
    //        // .HasForeignKey(rm => rm.MenuId);
    //    });
    //}

    //private static void ConfigureUserRole(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<UserRole>(b =>
    //    {
    //        b.ToTable(TablePrefix + "UserRole");
    //        // b.HasKey(x => x.Id);
    //        b.Property(x => x.Id).ValueGeneratedNever();
    //        b.HasKey(ur => new { ur.UserId, ur.RoleId });
    //        //b.HasOne(ur => ur.User)
    //        //      .WithMany(u => u.UserRoles)
    //        //      .HasForeignKey(ur => ur.UserId);
    //        //      //.OnDelete(DeleteBehavior.Cascade);
    //        //b.HasOne(ur => ur.Role)
    //        //      .WithMany(r => r.UserRoles)
    //        //      .HasForeignKey(ur => ur.RoleId);
    //    });
    //}

    private static void ConfigureCarousel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Carousel>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(CarouselEntityConsts.MaxTitleLength);
            b.Property(x => x.ImageUrl)
                .IsRequired()
                .HasMaxLength(CarouselEntityConsts.MaxImageUrlLength);
            b.Property(x => x.RedirectUrl)
                .HasMaxLength(CarouselEntityConsts.MaxRedirectUrlLength);
            b.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(CarouselEntityConsts.DefaultIsActive);
            b.Property(x => x.UpdatedAt).IsRequired();
            b.Property(x => x.StartDate)
                .IsRequired(false);
            b.Property(x => x.EndDate)
                .IsRequired(false);
            b.Property(x => x.Position)
                .IsRequired();
            b.Property(x => x.Description)
                .IsRequired(false);
            b.HasIndex(x => x.Position);
            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.CreatedBy);
            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UpdatedBy);
        });
    }

    private static void ConfigureSocialiteUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SocialiteUser>(b =>
        {
            b.ToTable(TablePrefix + "SocialiteUser");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.Name).HasMaxLength(SocialiteUserEntityConsts.MaxNameLength);
            b.Property(x => x.Provider).HasMaxLength(SocialiteUserEntityConsts.MaxProviderLength);
            b.Property(x => x.ProviderId).HasMaxLength(SocialiteUserEntityConsts.MaxProviderIdLength);
            b.Property(x => x.OpenId).HasMaxLength(SocialiteUserEntityConsts.MaxOpenIdLength);
            b.Property(x => x.UnionId).HasMaxLength(SocialiteUserEntityConsts.MaxUnionIdLength);
            b.Property(x => x.LastIpAddress).HasMaxLength(SocialiteUserEntityConsts.MaxLastIpAddressLength);
            b.Property(x => x.AccessToken).HasMaxLength(SocialiteUserEntityConsts.MaxAccessTokenLength);
            b.ConfigureAudit();
            b.HasIndex(x => new { x.Provider, x.ProviderId });
            b.HasIndex(x => new { x.Provider, x.OpenId });
            b.HasIndex(x => new { x.Provider, x.UnionId });
        });
    }

    private static void ConfigureFile(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lazy.Model.Entity.File>(b =>
        {
            b.ToTable(TablePrefix + "File");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.Storage).HasConversion(
                v => v.ToString(),
                v => (StorageType)Enum.Parse(typeof(StorageType), v)).HasMaxLength(EntityConsts.MaxLength32);
            b.Property(x => x.FileType).HasConversion(
                v => v.ToString(),
                v => (FileType)Enum.Parse(typeof(FileType), v)).HasMaxLength(EntityConsts.MaxLength32);
            b.Property(x => x.BaseUrl).HasMaxLength(FileEntityConsts.MaxBaseUrlLength);
            b.Property(x => x.MimeType).HasMaxLength(FileEntityConsts.MaxMimeTypeLength);
            b.Property(x => x.FileExt).HasMaxLength(FileEntityConsts.MaxFileExtLength);
            b.Property(x => x.FileMd5).HasMaxLength(FileEntityConsts.MaxFileMd5Length);
            b.Property(x => x.FileHash).HasMaxLength(FileEntityConsts.MaxFileHashLength);
            b.Property(x => x.FileName).HasMaxLength(FileEntityConsts.MaxFileNameLength);
            b.Property(x => x.FilePath).HasMaxLength(FileEntityConsts.MaxFilePathLength);
            b.ConfigureAudit();
            b.HasIndex(x => x.FileMd5);
            b.HasIndex(x => x.FileHash);
        });
    }

    private static void ConfigureConfig(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Config>(b =>
        {
            b.ToTable(TablePrefix + "Config");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.Key).IsRequired().HasMaxLength(EntityConsts.MaxLength128);
            b.Property(x => x.TypeName).IsRequired().HasMaxLength(EntityConsts.MaxLength200);
            b.Property(x => x.Value).HasColumnType("text");
            b.HasKey(s => s.Key);
        });
    }

    /// <summary>
    /// configuration for audit properties
    /// </summary>
    /// <typeparam name="T">your model<Teacher></typeparam>
    /// <param name="b">The delegate parameter passed to Entity<Teacher></param>
    public static void ConfigureAudit<T>(this EntityTypeBuilder<T> b)
        where T : class
    {
        if (b.Metadata.ClrType.IsSubclassOf(typeof(BaseEntityWithAudit)))
        {
            b.Property(nameof(BaseEntityWithAudit.CreatedBy))
                .IsRequired(true)
                .HasColumnName(nameof(BaseEntityWithAudit.CreatedBy));

            b.Property(nameof(BaseEntityWithAudit.CreatedAt))
                .IsRequired(true)
                .HasColumnName(nameof(BaseEntityWithAudit.CreatedAt));

            b.Property(nameof(BaseEntityWithAudit.UpdatedBy))
                .IsRequired(false)
                .HasColumnName(nameof(BaseEntityWithAudit.UpdatedBy));

            b.Property(nameof(BaseEntityWithAudit.UpdatedAt))
                .IsRequired(false)
                .HasColumnName(nameof(BaseEntityWithAudit.UpdatedAt));
        }
    }

    /// <summary>
    /// configuration for audit properties
    /// </summary>
    /// <typeparam name="T">your model<Teacher></typeparam>
    /// <param name="b">The delegate parameter passed to Entity<Teacher></param>
    public static void ConfigureSoftDelete<T>(this EntityTypeBuilder<T> b)
        where T : class
    {
        b.ConfigureAudit();

        if (b.Metadata.ClrType.IsSubclassOf(typeof(BaseEntityWithSoftDelete)))
        {
            b.Property(nameof(BaseEntityWithSoftDelete.IsDeleted))
                .IsRequired(true)
                .HasDefaultValue(false)
                .HasColumnName(nameof(BaseEntityWithSoftDelete.IsDeleted));

            b.Property(nameof(BaseEntityWithSoftDelete.DeletedBy))
                .IsRequired(false)
                .HasColumnName(nameof(BaseEntityWithSoftDelete.DeletedBy));

            b.Property(nameof(BaseEntityWithSoftDelete.DeletedAt))
                .IsRequired(false)
                .HasColumnName(nameof(BaseEntityWithSoftDelete.DeletedAt));
        }
    }
}