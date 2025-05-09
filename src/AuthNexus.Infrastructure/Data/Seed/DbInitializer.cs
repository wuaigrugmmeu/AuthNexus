using AuthNexus.Domain.Entities;
using AuthNexus.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace AuthNexus.Infrastructure.Data.Seed;

/// <summary>
/// 数据库初始化器，用于生成测试数据
/// </summary>
public class DbInitializer
{
    private readonly AuthNexusDbContext _context;
    private readonly ILogger<DbInitializer> _logger;
    private readonly IPasswordHashingService _passwordHashingService;

    public DbInitializer(
        AuthNexusDbContext context, 
        ILogger<DbInitializer> logger,
        IPasswordHashingService passwordHashingService)
    {
        _context = context;
        _logger = logger;
        _passwordHashingService = passwordHashingService;
    }

    /// <summary>
    /// 初始化数据库
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            // 确保数据库创建并应用迁移
            await _context.Database.EnsureCreatedAsync();
            
            // 检查数据库是否已有数据
            if (await _context.Applications.AnyAsync())
            {
                _logger.LogInformation("数据库已包含测试数据。跳过初始化。");
                return; // 如果已有数据，则跳过初始化
            }

            // 创建测试数据
            await SeedTestDataAsync();
            
            // 创建默认用户凭据
            await CreateDefaultUsers();

            _logger.LogInformation("数据库初始化完成，测试数据和默认用户已创建。");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "初始化数据库时发生错误");
            throw;
        }
    }

    /// <summary>
    /// 生成测试数据
    /// </summary>
    private async Task SeedTestDataAsync()
    {
        // 创建一个测试应用
        var testApp = CreateApplication(
            "test-app-001", 
            "测试应用1", 
            "test-api-key-001", 
            "test-client-secret-001",
            "这是一个用于测试的应用");
        
        var adminApp = CreateApplication(
            "admin-app-001", 
            "管理控制台", 
            "admin-api-key-001", 
            "admin-client-secret-001",
            "系统管理控制台应用");

        await _context.Applications.AddRangeAsync(testApp, adminApp);
        await _context.SaveChangesAsync();

        // 为测试应用创建权限
        var permissions = new List<PermissionDefinition>
        {
            new PermissionDefinition(testApp.Id, "user.read", "读取用户信息"),
            new PermissionDefinition(testApp.Id, "user.write", "修改用户信息"),
            new PermissionDefinition(testApp.Id, "user.delete", "删除用户"),
            new PermissionDefinition(testApp.Id, "order.read", "读取订单信息"),
            new PermissionDefinition(testApp.Id, "order.write", "修改订单信息"),
            new PermissionDefinition(testApp.Id, "order.delete", "删除订单"),
            new PermissionDefinition(testApp.Id, "product.read", "读取产品信息"),
            new PermissionDefinition(testApp.Id, "product.write", "修改产品信息"),
            new PermissionDefinition(testApp.Id, "product.delete", "删除产品")
        };

        // 为管理应用创建权限
        var adminPermissions = new List<PermissionDefinition>
        {
            new PermissionDefinition(adminApp.Id, "admin.full", "完全管理权限"),
            new PermissionDefinition(adminApp.Id, "admin.read", "只读管理权限"),
            new PermissionDefinition(adminApp.Id, "admin.user", "用户管理权限"),
            new PermissionDefinition(adminApp.Id, "admin.app", "应用管理权限"),
            // 添加系统权限常量
            new PermissionDefinition(adminApp.Id, "users:view", "查看用户"),
            new PermissionDefinition(adminApp.Id, "users:create", "创建用户"),
            new PermissionDefinition(adminApp.Id, "users:update", "更新用户"),
            new PermissionDefinition(adminApp.Id, "users:delete", "删除用户"),
            new PermissionDefinition(adminApp.Id, "roles:view", "查看角色"),
            new PermissionDefinition(adminApp.Id, "roles:create", "创建角色"),
            new PermissionDefinition(adminApp.Id, "roles:update", "更新角色"),
            new PermissionDefinition(adminApp.Id, "roles:delete", "删除角色"),
            new PermissionDefinition(adminApp.Id, "permissions:view", "查看权限"),
            new PermissionDefinition(adminApp.Id, "permissions:assign", "分配权限"),
            new PermissionDefinition(adminApp.Id, "applications:view", "查看应用"),
            new PermissionDefinition(adminApp.Id, "applications:create", "创建应用"),
            new PermissionDefinition(adminApp.Id, "applications:update", "更新应用"),
            new PermissionDefinition(adminApp.Id, "applications:delete", "删除应用")
        };

        await _context.PermissionDefinitions.AddRangeAsync(permissions);
        await _context.PermissionDefinitions.AddRangeAsync(adminPermissions);
        await _context.SaveChangesAsync();

        // 创建角色
        var roles = new List<Role>
        {
            new Role(testApp.Id, "admin", "系统管理员"),
            new Role(testApp.Id, "user-manager", "用户管理员"),
            new Role(testApp.Id, "order-manager", "订单管理员"),
            new Role(testApp.Id, "product-manager", "产品管理员"),
            new Role(testApp.Id, "reader", "只读用户")
        };

        var adminRoles = new List<Role>
        {
            new Role(adminApp.Id, "super-admin", "超级管理员"),
            new Role(adminApp.Id, "app-admin", "应用管理员"),
            new Role(adminApp.Id, "viewer", "观察者")
        };

        await _context.Roles.AddRangeAsync(roles);
        await _context.Roles.AddRangeAsync(adminRoles);
        await _context.SaveChangesAsync();

        // 为角色分配权限
        var rolePermissions = new List<RolePermissionAssignment>();

        // 为测试应用的角色分配权限
        // 管理员拥有所有权限
        foreach (var permission in permissions)
        {
            rolePermissions.Add(new RolePermissionAssignment(roles[0].Id, permission.Id));
        }

        // 用户管理员拥有用户相关权限
        rolePermissions.Add(new RolePermissionAssignment(roles[1].Id, permissions[0].Id)); // user.read
        rolePermissions.Add(new RolePermissionAssignment(roles[1].Id, permissions[1].Id)); // user.write
        rolePermissions.Add(new RolePermissionAssignment(roles[1].Id, permissions[2].Id)); // user.delete

        // 订单管理员拥有订单相关权限
        rolePermissions.Add(new RolePermissionAssignment(roles[2].Id, permissions[3].Id)); // order.read
        rolePermissions.Add(new RolePermissionAssignment(roles[2].Id, permissions[4].Id)); // order.write
        rolePermissions.Add(new RolePermissionAssignment(roles[2].Id, permissions[5].Id)); // order.delete

        // 产品管理员拥有产品相关权限
        rolePermissions.Add(new RolePermissionAssignment(roles[3].Id, permissions[6].Id)); // product.read
        rolePermissions.Add(new RolePermissionAssignment(roles[3].Id, permissions[7].Id)); // product.write
        rolePermissions.Add(new RolePermissionAssignment(roles[3].Id, permissions[8].Id)); // product.delete

        // 只读用户拥有所有读权限
        rolePermissions.Add(new RolePermissionAssignment(roles[4].Id, permissions[0].Id)); // user.read
        rolePermissions.Add(new RolePermissionAssignment(roles[4].Id, permissions[3].Id)); // order.read
        rolePermissions.Add(new RolePermissionAssignment(roles[4].Id, permissions[6].Id)); // product.read

        // 为管理应用的角色分配权限
        // 超级管理员拥有所有权限
        foreach (var permission in adminPermissions)
        {
            rolePermissions.Add(new RolePermissionAssignment(adminRoles[0].Id, permission.Id));
        }

        // 应用管理员拥有应用管理权限
        rolePermissions.Add(new RolePermissionAssignment(adminRoles[1].Id, adminPermissions[3].Id)); // admin.app
        
        // 观察者拥有只读权限
        rolePermissions.Add(new RolePermissionAssignment(adminRoles[2].Id, adminPermissions[1].Id)); // admin.read

        await _context.RolePermissionAssignments.AddRangeAsync(rolePermissions);
        await _context.SaveChangesAsync();

        // 创建测试用户
        var users = new List<UserIdentity>
        {
            new UserIdentity(testApp.Id, "user001"),
            new UserIdentity(testApp.Id, "user002"),
            new UserIdentity(testApp.Id, "user003"),
            new UserIdentity(testApp.Id, "user004"),
            new UserIdentity(testApp.Id, "user005")
        };

        var adminUsers = new List<UserIdentity>
        {
            new UserIdentity(adminApp.Id, "admin001"),
            new UserIdentity(adminApp.Id, "admin002")
        };

        await _context.UserIdentities.AddRangeAsync(users);
        await _context.UserIdentities.AddRangeAsync(adminUsers);
        await _context.SaveChangesAsync();

        // 为用户分配角色
        var userRoles = new List<UserRoleAssignment>
        {
            // 测试应用用户
            new UserRoleAssignment(users[0].Id, roles[0].Id), // user001 -> admin
            new UserRoleAssignment(users[1].Id, roles[1].Id), // user002 -> user-manager
            new UserRoleAssignment(users[2].Id, roles[2].Id), // user003 -> order-manager
            new UserRoleAssignment(users[3].Id, roles[3].Id), // user004 -> product-manager
            new UserRoleAssignment(users[4].Id, roles[4].Id), // user005 -> reader
            
            // 管理应用用户
            new UserRoleAssignment(adminUsers[0].Id, adminRoles[0].Id), // admin001 -> super-admin
            new UserRoleAssignment(adminUsers[1].Id, adminRoles[2].Id)  // admin002 -> viewer
        };

        await _context.UserRoleAssignments.AddRangeAsync(userRoles);
        await _context.SaveChangesAsync();

        // 为用户分配直接权限（测试覆盖角色权限的情况）
        var userDirectPermissions = new List<UserDirectPermissionAssignment>
        {
            // 给user002用户额外的订单读取权限
            new UserDirectPermissionAssignment(users[1].Id, permissions[3].Id),
            
            // 给user005用户额外的admin.app权限（跨应用的情况）
            // 注意：在实际业务中应该避免这种情况，这里只是为了测试
            new UserDirectPermissionAssignment(users[4].Id, adminPermissions[3].Id)
        };

        await _context.UserDirectPermissionAssignments.AddRangeAsync(userDirectPermissions);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 创建默认用户凭据
    /// </summary>
    private async Task CreateDefaultUsers()
    {
        try
        {
            // 查找管理应用
            var adminApp = await _context.Applications.FirstOrDefaultAsync(a => a.Name == "管理控制台");
            if (adminApp == null)
            {
                _logger.LogWarning("未找到管理控制台应用，无法创建默认用户的权限");
                return;
            }
            
            // 查找超级管理员角色
            var superAdminRole = await _context.Roles.FirstOrDefaultAsync(r => r.ApplicationId == adminApp.Id && r.Name == "super-admin");
            var viewerRole = await _context.Roles.FirstOrDefaultAsync(r => r.ApplicationId == adminApp.Id && r.Name == "viewer");
            
            if (superAdminRole == null || viewerRole == null)
            {
                _logger.LogWarning("未找到超级管理员或观察者角色，无法创建默认用户的权限");
                return;
            }
            
            // 创建超级管理员用户
            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@authnexus.com",
                PasswordHash = _passwordHashingService.HashPassword("Admin@123456"),
                DisplayName = "系统管理员",
                Status = UserStatus.Active,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            // 创建测试用户
            var testUser = new User
            {
                Username = "test",
                Email = "test@authnexus.com",
                PasswordHash = _passwordHashingService.HashPassword("Test@123456"),
                DisplayName = "测试用户",
                Status = UserStatus.Active,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            await _context.AddRangeAsync(adminUser, testUser);
            await _context.SaveChangesAsync();
            
            // 为默认用户创建UserIdentity
            var adminIdentity = new UserIdentity(adminApp.Id, adminUser.Username);
            var testIdentity = new UserIdentity(adminApp.Id, testUser.Username);
            
            await _context.UserIdentities.AddRangeAsync(adminIdentity, testIdentity);
            await _context.SaveChangesAsync();
            
            // 分配角色给用户
            var userRoles = new List<UserRoleAssignment>
            {
                new UserRoleAssignment(adminIdentity.Id, superAdminRole.Id), // admin -> super-admin
                new UserRoleAssignment(testIdentity.Id, viewerRole.Id)  // test -> viewer
            };
            
            await _context.UserRoleAssignments.AddRangeAsync(userRoles);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("创建默认用户成功。管理员账号: admin/Admin@123456, 测试账号: test/Test@123456");
            _logger.LogInformation("已为默认用户分配角色：admin -> super-admin, test -> viewer");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建默认用户凭据时发生错误");
            throw;
        }
    }

    /// <summary>
    /// 创建应用并哈希密钥
    /// </summary>
    private AuthNexus.Domain.Entities.Application CreateApplication(string appUid, string name, string apiKey, string clientSecret, string description)
    {
        // 简单哈希方法，实际应用中应该使用更安全的方法
        string HashSecret(string secret)
        {
            using var sha256 = SHA256.Create();
            var secretBytes = Encoding.UTF8.GetBytes(secret);
            var hashBytes = sha256.ComputeHash(secretBytes);
            return Convert.ToBase64String(hashBytes);
        }

        return new AuthNexus.Domain.Entities.Application(
            appUid,
            name,
            HashSecret(apiKey),
            HashSecret(clientSecret),
            description
        );
    }
}