using Microsoft.EntityFrameworkCore;
using SquadFile.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace SquadFile.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<SysUser> SysUsers { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }
        public DbSet<FileFolder> FileFolders { get; set; }
        public DbSet<FileRecord> FileRecords { get; set; }
        public DbSet<FolderPermission> FolderPermissions { get; set; }
        public DbSet<ShareRecord> ShareRecords { get; set; }
        public DbSet<LoginLog> LoginLogs { get; set; }
        public DbSet<DownloadLog> DownloadLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SysUser>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Role).HasConversion<string>();
                entity.Property(e => e.Status).HasConversion<string>();
                entity.Property(e => e.CreatedTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<SystemSettings>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DefaultLanguage).HasDefaultValue("zh-Hans");
                entity.HasData(
                    new SystemSettings
                    {
                        Id = 1,
                        DefaultLanguage = "zh-Hans",
                        SiteName = "小队快传",
                        LoginLogoPath = "/static/images/logo.png",
                        HomeLogoPath = "/static/images/logo.png",
                        MaxFileSize = 100,
                        StorageLimit = 10240,
                        CreatedTime = DateTime.Now,
                        UpdatedTime = DateTime.Now
                    }
                );
            });

            // 配置FileFolder实体
            modelBuilder.Entity<FileFolder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ParentId);
                entity.HasIndex(e => e.CreatedBy);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.CreatedTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // 配置FileRecord实体
            modelBuilder.Entity<FileRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.FolderId);
                entity.HasIndex(e => e.UploadedBy);
                entity.Property(e => e.OriginalName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.StorageName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Extension).HasMaxLength(50);
                entity.Property(e => e.UploadTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // 配置FolderPermission实体
            modelBuilder.Entity<FolderPermission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.FolderId, e.UserId }).IsUnique();
                entity.HasIndex(e => e.FolderId);
                entity.HasIndex(e => e.UserId);
                entity.Property(e => e.GrantTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // 配置ShareRecord实体
            modelBuilder.Entity<ShareRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ItemId);
                entity.HasIndex(e => e.ShortCode).IsUnique();
                entity.Property(e => e.ShortCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Password).HasMaxLength(255);
                entity.Property(e => e.CreatedTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                // 配置CreatedBy外键关系
                entity.HasOne<SysUser>()
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 配置LoginLog实体
            modelBuilder.Entity<LoginLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.LoginTime);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(255);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.LoginTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // 配置DownloadLog实体
            modelBuilder.Entity<DownloadLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.FileId);
                entity.HasIndex(e => e.DownloadTime);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(255);
                entity.Property(e => e.OriginalFileName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.DeviceType).HasMaxLength(100);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.DownloadTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // TemporaryDownloadToken实体配置已移除，因为现在使用内存存储
        }
    }
}