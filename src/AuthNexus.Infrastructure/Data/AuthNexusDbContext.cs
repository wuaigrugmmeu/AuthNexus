using Microsoft.EntityFrameworkCore;
using DomainEntities = AuthNexus.Domain.Entities;

namespace AuthNexus.Infrastructure.Data;

/// <summary>
/// 应用数据库上下文
/// </summary>
public class AuthNexusDbContext : DbContext
{
    /// <summary>
    /// 应用表
    /// </summary>
    public DbSet<DomainEntities.Application> Applications { get; set; }
    
    /// <summary>
    /// 权限定义表
    /// </summary>
    public DbSet<DomainEntities.PermissionDefinition> PermissionDefinitions { get; set; }
    
    /// <summary>
    /// 角色表
    /// </summary>
    public DbSet<DomainEntities.Role> Roles { get; set; }
    
    /// <summary>
    /// 用户身份表
    /// </summary>
    public DbSet<DomainEntities.UserIdentity> UserIdentities { get; set; }
    
    /// <summary>
    /// 角色-权限关联表
    /// </summary>
    public DbSet<DomainEntities.RolePermissionAssignment> RolePermissionAssignments { get; set; }
    
    /// <summary>
    /// 用户-角色关联表
    /// </summary>
    public DbSet<DomainEntities.UserRoleAssignment> UserRoleAssignments { get; set; }
    
    /// <summary>
    /// 用户-直接权限关联表
    /// </summary>
    public DbSet<DomainEntities.UserDirectPermissionAssignment> UserDirectPermissionAssignments { get; set; }

    public AuthNexusDbContext(DbContextOptions<AuthNexusDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置应用表
        modelBuilder.Entity<DomainEntities.Application>(entity =>
        {
            entity.ToTable("Applications");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.AppUID).IsUnique();
            entity.Property(e => e.AppUID).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.HashedApiKey).IsRequired();
            entity.Property(e => e.HashedClientSecret).IsRequired();
            entity.Property(e => e.IsEnabled).HasDefaultValue(true);
        });

        // 配置权限定义表
        modelBuilder.Entity<DomainEntities.PermissionDefinition>(entity =>
        {
            entity.ToTable("PermissionDefinitions");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ApplicationId, e.Code }).IsUnique();
            entity.Property(e => e.Code).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            
            // 外键关系
            entity.HasOne(e => e.Application)
                  .WithMany()
                  .HasForeignKey(e => e.ApplicationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 配置角色表
        modelBuilder.Entity<DomainEntities.Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ApplicationId, e.Name }).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            
            // 外键关系
            entity.HasOne(e => e.Application)
                  .WithMany()
                  .HasForeignKey(e => e.ApplicationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 配置用户身份表
        modelBuilder.Entity<DomainEntities.UserIdentity>(entity =>
        {
            entity.ToTable("UserIdentities");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ApplicationId, e.ExternalUserId }).IsUnique();
            entity.Property(e => e.ExternalUserId).IsRequired().HasMaxLength(100);
            
            // 外键关系
            entity.HasOne(e => e.Application)
                  .WithMany()
                  .HasForeignKey(e => e.ApplicationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 配置角色-权限关联表
        modelBuilder.Entity<DomainEntities.RolePermissionAssignment>(entity =>
        {
            entity.ToTable("RolePermissionAssignments");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.RoleId, e.PermissionDefinitionId }).IsUnique();
            
            // 外键关系
            entity.HasOne(e => e.Role)
                  .WithMany(r => r.Permissions)
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.PermissionDefinition)
                  .WithMany()
                  .HasForeignKey(e => e.PermissionDefinitionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 配置用户-角色关联表
        modelBuilder.Entity<DomainEntities.UserRoleAssignment>(entity =>
        {
            entity.ToTable("UserRoleAssignments");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserIdentityId, e.RoleId }).IsUnique();
            
            // 外键关系
            entity.HasOne(e => e.UserIdentity)
                  .WithMany(u => u.Roles)
                  .HasForeignKey(e => e.UserIdentityId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Role)
                  .WithMany()
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 配置用户-直接权限关联表
        modelBuilder.Entity<DomainEntities.UserDirectPermissionAssignment>(entity =>
        {
            entity.ToTable("UserDirectPermissionAssignments");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserIdentityId, e.PermissionDefinitionId }).IsUnique();
            
            // 外键关系
            entity.HasOne(e => e.UserIdentity)
                  .WithMany(u => u.DirectPermissions)
                  .HasForeignKey(e => e.UserIdentityId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.PermissionDefinition)
                  .WithMany()
                  .HasForeignKey(e => e.PermissionDefinitionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}