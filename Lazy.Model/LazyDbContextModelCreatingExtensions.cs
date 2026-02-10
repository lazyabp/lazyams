using Lazy.Shared.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class LazyDbContextModelCreatingExtensions
{
    private const string TablePrefix = "";

    /// <summary>
    /// Admin
    /// </summary>
    /// <param name="modelBuilder"></param>
    public static void ConfigureBaseManagement(this ModelBuilder modelBuilder)
    {
        ConfigureUser(modelBuilder);
        ConfigureRole(modelBuilder);
        ConfigureMenu(modelBuilder);

        ConfigureConfig(modelBuilder);
        ConfigureSocialiteUser(modelBuilder);
        ConfigureFile(modelBuilder);

        ConfigureAutoJob(modelBuilder);
        ConfigureAutoJobLog(modelBuilder);

        ConfigureCarousel(modelBuilder);
    }

    public static void ConfigureBusinessManagement(this ModelBuilder modelBuilder)
    {
        ConfigurePackage(modelBuilder);
        ConfigurePackageFeature(modelBuilder);
        ConfigureOrder(modelBuilder);
        ConfigureUserSubscription(modelBuilder);
    }

    #region Base configuration

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
            b.ConfigureAudit();
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
            b.ConfigureAudit();
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
            b.HasOne(cs => cs.Parent).WithMany(cs => cs.Children).HasForeignKey(cs => cs.ParentId);
            b.ConfigureAudit();
            b.HasMany(x => x.Roles).WithMany(x => x.Menus);
            b.HasIndex(x => new { x.IsDeleted, x.MenuType, x.ParentId, x.CreatedAt });
        });
    }

    private static void ConfigureAutoJob(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AutoJob>(b =>
        {
            b.ToTable(TablePrefix + "AutoJob");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.JobGroupName)
                .IsRequired()
                .HasMaxLength(EntityConsts.MaxLength50);
            b.Property(x => x.JobName)
                .IsRequired()
                .HasMaxLength(EntityConsts.MaxLength50);
            b.Property(cs => cs.JobStatus).HasConversion(
               v => v.ToString(),
               v => (JobStatus)Enum.Parse(typeof(JobStatus), v)
            ).HasMaxLength(EntityConsts.MaxLength32);
            b.Property(x => x.CronExpression)
                .IsRequired()
                .HasMaxLength(EntityConsts.MaxLength50);
            b.Property(x => x.StartAt)
                .IsRequired(false);
            b.Property(x => x.EndAt)
                .IsRequired(false);
            b.Property(x => x.NextStartAt)
                .IsRequired(false);
            b.Property(x => x.Remark)
                .IsRequired(false)
                .HasMaxLength(EntityConsts.MaxLength255);
            b.ConfigureAudit();
            b.HasIndex(x => x.JobGroupName);
            b.HasIndex(x => x.JobName);
        });
    }

    private static void ConfigureAutoJobLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AutoJobLog>(b =>
        {
            b.ToTable(TablePrefix + "AutoJobLog");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.JobGroupName)
                .IsRequired()
                .HasMaxLength(EntityConsts.MaxLength50);
            b.Property(x => x.JobName)
                .IsRequired()
                .HasMaxLength(EntityConsts.MaxLength50);
            b.Property(x => x.LogStatus)
                .IsRequired(false);
            b.Property(x => x.Remark)
                .IsRequired(false);
            b.ConfigureAudit();
            b.HasIndex(x => x.JobGroupName);
            b.HasIndex(x => x.JobName);
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

    #endregion

    #region Business configuration

    private static void ConfigurePackage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Package>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(EntityConsts.MaxLength100);
            b.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(EntityConsts.MaxLength50);
            b.Property(x => x.Version)
                .HasMaxLength(EntityConsts.MaxLength50);
            b.Property(x => x.IsActive)
                .IsRequired();
            b.Property(x => x.Price)
                .IsRequired(true);
            b.Property(x => x.DiscountedPrice)
                .IsRequired(false);
            b.Property(x => x.DurationUnit)
                .IsRequired()
                .HasConversion(
                    v => v.ToString(),
                    v => (DurationUnit)Enum.Parse(typeof(DurationUnit), v)
                )
                .HasDefaultValue(DurationUnit.Month)
                .HasMaxLength(EntityConsts.MaxLength32);
            b.Property(x => x.SortOrder)
                .IsRequired(true);
            b.Property(x => x.Description)
                .IsRequired(false);
            b.ConfigureAudit();
            b.HasMany(x => x.Features);
            b.HasIndex(x => x.Code).IsUnique();
        });
    }

    private static void ConfigurePackageFeature(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PackageFeature>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.PackageId)
                .IsRequired();
            b.Property(x => x.FeatureKey)
                .IsRequired()
                .HasMaxLength(EntityConsts.MaxLength100);
            b.Property(x => x.FeatureValue)
                .IsRequired();
            b.Property(x => x.FeatureType)
                .IsRequired();
            b.Property(x => x.Description)
                .IsRequired(false);
            b.ConfigureAudit();
            b.HasOne(x => x.Package);
            b.HasIndex(x => x.PackageId);
            b.HasIndex(x => new { x.PackageId, x.FeatureKey}).IsUnique();
        });
    }

    private static void ConfigureOrder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.OrderNo)
                .IsRequired(true)
                .HasMaxLength(EntityConsts.MaxLength128);
            b.Property(x => x.TradeNo)
                .IsRequired(false)
                .HasMaxLength(EntityConsts.MaxLength128);
            b.Property(x => x.UserId)
                .IsRequired();
            b.Property(x => x.PackageId)
                .IsRequired();
            b.Property(x => x.OrderType)
                .IsRequired();
            b.Property(x => x.Status)
                .IsRequired();
            b.Property(x => x.Price)
                .IsRequired();
            b.Property(x => x.Quantity)
                .IsRequired();
            b.Property(x => x.Amount)
                .IsRequired();
            b.Property(x => x.Currency)
                .IsRequired();
            b.Property(x => x.PayType)
                .IsRequired();
            b.Property(x => x.PaidAt)
                .IsRequired(false);
            b.Property(x => x.CompletedAt)
                .IsRequired(false);
            b.Property(x => x.CanceledAt)
                .IsRequired(false);
            b.Property(x => x.FailedAt)
                .IsRequired(false);
            b.Property(x => x.FailReason)
                .IsRequired(false);
            b.Property(x => x.RefundedAt)
                .IsRequired(false);
            b.Property(x => x.RefundAmount)
                .IsRequired(false);
            b.Property(x => x.RefundReason)
                .IsRequired(false);
            b.ConfigureAudit();
            b.HasOne(x => x.User);
            b.HasOne(x => x.Package);
            b.HasIndex(x => x.OrderNo).IsUnique();
            b.HasIndex(x => x.TradeNo).IsUnique();
            b.HasIndex(x => x.UserId);
            b.HasIndex(x => x.PackageId);
        });
    }

    private static void ConfigureUserSubscription(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSubscription>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.UserId)
                .IsRequired();
            b.Property(x => x.PackageId)
                .IsRequired();
            b.Property(x => x.LastOrderId)
                .IsRequired(false);
            b.Property(x => x.StartAt)
                .IsRequired();
            b.Property(x => x.EndAt)
                .IsRequired();
            b.Property(x => x.Status)
                .IsRequired();
            b.ConfigureAudit();
            b.HasOne(x => x.User);
            b.HasOne(x => x.Package);
        });
    }

    #endregion

    /// <summary>
    /// configuration for audit properties
    /// </summary>
    /// <typeparam name="T">your model<Teacher></typeparam>
    /// <param name="b">The delegate parameter passed to Entity<Teacher></param>
    public static void ConfigureAudit<T>(this EntityTypeBuilder<T> b)
        where T : class
    {
        if (b.Metadata.ClrType.IsSubclassOf(typeof(BaseEntityWithCreatingAudit)))
        {
            b.Property(nameof(BaseEntityWithCreatingAudit.CreatedBy))
                .IsRequired(true)
                .HasColumnName(nameof(BaseEntityWithCreatingAudit.CreatedBy));

            b.Property(nameof(BaseEntityWithCreatingAudit.CreatedAt))
                .IsRequired(true)
                .HasColumnName(nameof(BaseEntityWithCreatingAudit.CreatedAt));
        }

        if (b.Metadata.ClrType.IsSubclassOf(typeof(BaseEntityWithUpdatingAudit)))
        {
            b.Property(nameof(BaseEntityWithUpdatingAudit.UpdatedBy))
                .IsRequired(false)
                .HasColumnName(nameof(BaseEntityWithUpdatingAudit.UpdatedBy));

            b.Property(nameof(BaseEntityWithUpdatingAudit.UpdatedAt))
                .IsRequired(false)
                .HasColumnName(nameof(BaseEntityWithUpdatingAudit.UpdatedAt));
        }

        if (b.Metadata.ClrType.IsSubclassOf(typeof(BaseEntityWithDeletingAudit)))
        {
            b.Property(nameof(BaseEntityWithDeletingAudit.IsDeleted))
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName(nameof(BaseEntityWithDeletingAudit.IsDeleted));

            b.Property(nameof(BaseEntityWithDeletingAudit.DeletedBy))
                .IsRequired(false)
                .HasColumnName(nameof(BaseEntityWithDeletingAudit.DeletedBy));

            b.Property(nameof(BaseEntityWithDeletingAudit.DeletedAt))
                .IsRequired(false)
                .HasColumnName(nameof(BaseEntityWithDeletingAudit.DeletedAt));
        }
    }





    private static void ConfigureCarousel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Carousel>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
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
            b.Property(x => x.StartAt)
                .IsRequired(false);
            b.Property(x => x.EndAt)
                .IsRequired(false);
            b.Property(x => x.Position)
                .IsRequired();
            b.Property(x => x.Description)
                .IsRequired(false);
            b.HasIndex(x => x.Position);
            b.ConfigureAudit();
        });
    }
}