using Microsoft.EntityFrameworkCore;

namespace LagerApp.Models;

public partial class EliasaphramSeDb7Context : DbContext
{
    private readonly IConfiguration _configuration;
    public EliasaphramSeDb7Context(DbContextOptions<EliasaphramSeDb7Context> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<List> Lists { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseMySql(connectionString, ServerVersion.Parse("10.6.16-mariadb"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("latin1_swedish_ci")
            .HasCharSet("latin1");

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.Property(e => e.Id).HasColumnType("int(11)");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex").IsUnique();

            entity.Property(e => e.AccessFailedCount).HasColumnType("int(11)");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.LockoutEnd).HasMaxLength(6);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j
                            .ToTable("AspNetUserRoles")
                            .HasCharSet("utf8mb4")
                            .UseCollation("utf8mb4_general_ci");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.Property(e => e.Id).HasColumnType("int(11)");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<List>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("List")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.ArticleNumber, "ArticleNumber").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Quantity).HasColumnType("int(11)");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.ArticleNumber, "ArticleNumber").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Dimension).HasPrecision(18, 2);
            entity.Property(e => e.Material).HasMaxLength(255);
            entity.Property(e => e.PurchasePrice).HasPrecision(18, 2);
            entity.Property(e => e.Quantity).HasColumnType("int(11)");
            entity.Property(e => e.SellingPrice).HasPrecision(18, 2);
            entity.Property(e => e.Weight).HasPrecision(18, 2);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
