using AuthNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DomainEntities = AuthNexus.Domain.Entities;
using AuthNexus.Domain.Repositories;
using AuthNexus.Domain.Entities;

namespace AuthNexus.Infrastructure.Repositories;

/// <summary>
/// 用户仓储实现
/// </summary>
public class UserRepository : EFCoreGenericRepository<DomainEntities.User>, IUserRepository
{
    public UserRepository(AuthNexusDbContext dbContext) : base(dbContext)
    {
    }

    /// <summary>
    /// 根据ID获取用户身份
    /// </summary>
    public async Task<DomainEntities.UserIdentity> GetByIdAsync(Guid id)
    {
        return await _dbContext.UserIdentities
            .Include(u => u.Application)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// 根据应用ID获取所有用户
    /// </summary>
    public async Task<IEnumerable<DomainEntities.UserIdentity>> GetByApplicationIdAsync(Guid applicationId)
    {
        return await _dbContext.UserIdentities
            .Include(u => u.Application)
            .Where(u => u.ApplicationId == applicationId)
            .ToListAsync();
    }

    /// <summary>
    /// 根据应用ID和外部用户ID获取用户
    /// </summary>
    public async Task<DomainEntities.UserIdentity> GetByExternalIdAsync(Guid applicationId, string externalUserId)
    {
        return await _dbContext.UserIdentities
            .Include(u => u.Application)
            .FirstOrDefaultAsync(u => u.ApplicationId == applicationId && u.ExternalUserId == externalUserId);
    }

    /// <summary>
    /// 获取用户（包含角色）
    /// </summary>
    public async Task<DomainEntities.UserIdentity> GetWithRolesAsync(Guid id)
    {
        return await _dbContext.UserIdentities
            .Include(u => u.Application)
            .Include(u => u.Roles)
                .ThenInclude(r => r.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// 获取用户（包含直接权限）
    /// </summary>
    public async Task<DomainEntities.UserIdentity> GetWithDirectPermissionsAsync(Guid id)
    {
        return await _dbContext.UserIdentities
            .Include(u => u.Application)
            .Include(u => u.DirectPermissions)
                .ThenInclude(p => p.PermissionDefinition)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// 获取用户（包含角色和直接权限）
    /// </summary>
    public async Task<DomainEntities.UserIdentity> GetWithRolesAndPermissionsAsync(Guid id)
    {
        return await _dbContext.UserIdentities
            .Include(u => u.Application)
            .Include(u => u.Roles)
                .ThenInclude(r => r.Role)
            .Include(u => u.DirectPermissions)
                .ThenInclude(p => p.PermissionDefinition)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// 创建新用户
    /// </summary>
    public async Task<DomainEntities.UserIdentity> CreateAsync(DomainEntities.UserIdentity user)
    {
        _dbContext.UserIdentities.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    public async Task<DomainEntities.UserIdentity> UpdateAsync(DomainEntities.UserIdentity user)
    {
        // 更新用户基本信息
        _dbContext.UserIdentities.Update(user);
        
        // 如果用户包含角色列表，更新角色关联
        if (user.Roles != null && user.Roles.Any())
        {
            var existingRoles = await _dbContext.UserRoleAssignments
                .Where(r => r.UserIdentityId == user.Id)
                .ToListAsync();
            
            // 添加新角色关联
            foreach (var role in user.Roles)
            {
                if (!existingRoles.Any(r => r.Id == role.Id))
                {
                    _dbContext.UserRoleAssignments.Add(role);
                }
            }
            
            // 移除不再需要的角色关联
            foreach (var existingRole in existingRoles)
            {
                if (!user.Roles.Any(r => r.Id == existingRole.Id))
                {
                    _dbContext.UserRoleAssignments.Remove(existingRole);
                }
            }
        }
        
        // 如果用户包含直接权限列表，更新权限关联
        if (user.DirectPermissions != null && user.DirectPermissions.Any())
        {
            var existingPermissions = await _dbContext.UserDirectPermissionAssignments
                .Where(p => p.UserIdentityId == user.Id)
                .ToListAsync();
            
            // 添加新权限关联
            foreach (var permission in user.DirectPermissions)
            {
                if (!existingPermissions.Any(p => p.Id == permission.Id))
                {
                    _dbContext.UserDirectPermissionAssignments.Add(permission);
                }
            }
            
            // 移除不再需要的权限关联
            foreach (var existingPermission in existingPermissions)
            {
                if (!user.DirectPermissions.Any(p => p.Id == existingPermission.Id))
                {
                    _dbContext.UserDirectPermissionAssignments.Remove(existingPermission);
                }
            }
        }
        
        await _dbContext.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// 获取用户所有权限（包括角色权限和直接权限）
    /// </summary>
    public async Task<IEnumerable<DomainEntities.PermissionDefinition>> GetAllPermissionsAsync(Guid applicationId, string externalUserId)
    {
        var user = await GetByExternalIdAsync(applicationId, externalUserId);
        if (user == null)
        {
            return Enumerable.Empty<DomainEntities.PermissionDefinition>();
        }

        // 获取用户的所有角色ID
        var roleIds = await _dbContext.UserRoleAssignments
            .Where(ur => ur.UserIdentityId == user.Id)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        // 获取这些角色的所有权限
        var rolePermissions = await _dbContext.RolePermissionAssignments
            .Where(rp => roleIds.Contains(rp.RoleId))
            .Select(rp => rp.PermissionDefinitionId)
            .ToListAsync();

        // 获取用户的直接权限
        var directPermissions = await _dbContext.UserDirectPermissionAssignments
            .Where(up => up.UserIdentityId == user.Id)
            .Select(up => up.PermissionDefinitionId)
            .ToListAsync();

        // 合并角色权限和直接权限（去重）
        var allPermissionIds = rolePermissions.Union(directPermissions).Distinct();

        // 获取权限定义
        return await _dbContext.PermissionDefinitions
            .Where(p => allPermissionIds.Contains(p.Id))
            .ToListAsync();
    }

    /// <summary>
    /// 检查用户是否拥有指定权限
    /// </summary>
    public async Task<bool> HasPermissionAsync(Guid applicationId, string externalUserId, string permissionCode)
    {
        var user = await GetByExternalIdAsync(applicationId, externalUserId);
        if (user == null)
        {
            return false;
        }

        // 获取权限ID
        var permission = await _dbContext.PermissionDefinitions
            .FirstOrDefaultAsync(p => p.ApplicationId == applicationId && p.Code == permissionCode);
        
        if (permission == null)
        {
            return false;
        }

        // 检查用户是否直接拥有此权限
        var hasDirectPermission = await _dbContext.UserDirectPermissionAssignments
            .AnyAsync(up => up.UserIdentityId == user.Id && up.PermissionDefinitionId == permission.Id);

        if (hasDirectPermission)
        {
            return true;
        }

        // 获取用户的所有角色ID
        var roleIds = await _dbContext.UserRoleAssignments
            .Where(ur => ur.UserIdentityId == user.Id)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        // 检查这些角色是否拥有此权限
        return await _dbContext.RolePermissionAssignments
            .AnyAsync(rp => roleIds.Contains(rp.RoleId) && rp.PermissionDefinitionId == permission.Id);
    }

    /// <summary>
    /// 根据用户名或邮箱获取用户
    /// </summary>
    public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
    }

    /// <summary>
    /// 检查用户名是否已存在
    /// </summary>
    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _dbSet.AnyAsync(u => u.Username == username);
    }

    /// <summary>
    /// 检查邮箱是否已存在
    /// </summary>
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }

    /// <summary>
    /// 获取用户的角色
    /// </summary>
    public async Task<IReadOnlyList<Role>> GetUserRolesAsync(Guid userId)
    {
        var roleIds = await _dbContext.UserRoleAssignments
            .Where(ur => ur.UserIdentityId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        return await _dbContext.Roles
            .Where(r => roleIds.Contains(r.Id))
            .ToListAsync();
    }

    /// <summary>
    /// 获取用户的权限
    /// </summary>
    public async Task<IReadOnlyList<Permission>> GetUserPermissionsAsync(Guid userId)
    {
        // 获取用户的直接权限ID
        var directPermissionIds = await _dbContext.UserDirectPermissionAssignments
            .Where(up => up.UserIdentityId == userId)
            .Select(up => up.PermissionDefinitionId)
            .ToListAsync();

        // 获取用户角色的权限ID
        var roleIds = await _dbContext.UserRoleAssignments
            .Where(ur => ur.UserIdentityId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        var rolePermissionIds = await _dbContext.RolePermissionAssignments
            .Where(rp => roleIds.Contains(rp.RoleId))
            .Select(rp => rp.PermissionDefinitionId)
            .ToListAsync();

        // 合并并去重
        var allPermissionIds = directPermissionIds.Union(rolePermissionIds).Distinct();

        return await _dbContext.PermissionDefinitions
            .Where(p => allPermissionIds.Contains(p.Id))
            .Select(p => new Permission
            {
                Id = p.Id,
                ApplicationId = p.ApplicationId,
                Name = p.Code,
                Description = p.Description
            })
            .ToListAsync();
    }

    /// <summary>
    /// 为用户分配角色
    /// </summary>
    public async Task AssignRoleToUserAsync(Guid userId, Guid roleId)
    {
        var exists = await _dbContext.UserRoleAssignments
            .AnyAsync(ur => ur.UserIdentityId == userId && ur.RoleId == roleId);

        if (!exists)
        {
            _dbContext.UserRoleAssignments.Add(new DomainEntities.UserRoleAssignment
            {
                Id = Guid.NewGuid(),
                UserIdentityId = userId,
                RoleId = roleId
            });

            await _dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 移除用户的角色
    /// </summary>
    public async Task RemoveRoleFromUserAsync(Guid userId, Guid roleId)
    {
        var assignment = await _dbContext.UserRoleAssignments
            .FirstOrDefaultAsync(ur => ur.UserIdentityId == userId && ur.RoleId == roleId);

        if (assignment != null)
        {
            _dbContext.UserRoleAssignments.Remove(assignment);
            await _dbContext.SaveChangesAsync();
        }
    }
}