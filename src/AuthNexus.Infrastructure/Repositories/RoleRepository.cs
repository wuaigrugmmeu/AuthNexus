using AuthNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DomainEntities = AuthNexus.Domain.Entities;
using AuthNexus.Domain.Repositories;

namespace AuthNexus.Infrastructure.Repositories;

/// <summary>
/// 角色仓储实现
/// </summary>
public class RoleRepository : IRoleRepository
{
    private readonly AuthNexusDbContext _dbContext;

    public RoleRepository(AuthNexusDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// 根据ID获取角色
    /// </summary>
    public async Task<DomainEntities.Role> GetByIdAsync(Guid id)
    {
        return await _dbContext.Roles
            .Include(r => r.Application)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    /// <summary>
    /// 根据应用ID和角色名称获取角色
    /// </summary>
    public async Task<DomainEntities.Role> GetByNameAsync(Guid applicationId, string name)
    {
        return await _dbContext.Roles
            .Include(r => r.Application)
            .FirstOrDefaultAsync(r => r.ApplicationId == applicationId && r.Name == name);
    }

    /// <summary>
    /// 获取角色（包含权限）
    /// </summary>
    public async Task<DomainEntities.Role> GetWithPermissionsAsync(Guid id)
    {
        return await _dbContext.Roles
            .Include(r => r.Application)
            .Include(r => r.Permissions)
                .ThenInclude(p => p.PermissionDefinition)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    /// <summary>
    /// 获取应用的所有角色
    /// </summary>
    public async Task<IEnumerable<DomainEntities.Role>> GetByApplicationIdAsync(Guid applicationId)
    {
        return await _dbContext.Roles
            .Include(r => r.Application)
            .Where(r => r.ApplicationId == applicationId)
            .ToListAsync();
    }

    /// <summary>
    /// 创建新角色
    /// </summary>
    public async Task<DomainEntities.Role> CreateAsync(DomainEntities.Role role)
    {
        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync();
        return role;
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    public async Task<DomainEntities.Role> UpdateAsync(DomainEntities.Role role)
    {
        // 更新角色基本信息
        _dbContext.Roles.Update(role);
        
        // 如果角色包含权限列表，更新权限关联
        if (role.Permissions != null && role.Permissions.Any())
        {
            var existingPermissions = await _dbContext.RolePermissionAssignments
                .Where(p => p.RoleId == role.Id)
                .ToListAsync();
            
            // 添加新权限关联
            foreach (var permission in role.Permissions)
            {
                if (!existingPermissions.Any(p => p.Id == permission.Id))
                {
                    _dbContext.RolePermissionAssignments.Add(permission);
                }
            }
            
            // 移除不再需要的权限关联
            foreach (var existingPermission in existingPermissions)
            {
                if (!role.Permissions.Any(p => p.Id == existingPermission.Id))
                {
                    _dbContext.RolePermissionAssignments.Remove(existingPermission);
                }
            }
        }
        
        await _dbContext.SaveChangesAsync();
        return role;
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var role = await _dbContext.Roles.FindAsync(id);
        if (role != null)
        {
            _dbContext.Roles.Remove(role);
            await _dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 检查应用中是否存在指定名称的角色
    /// </summary>
    public async Task<bool> ExistsByNameAsync(Guid applicationId, string name)
    {
        return await _dbContext.Roles
            .AnyAsync(r => r.ApplicationId == applicationId && r.Name == name);
    }

    /// <summary>
    /// 根据ID列表获取多个角色
    /// </summary>
    public async Task<IEnumerable<DomainEntities.Role>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _dbContext.Roles
            .Include(r => r.Application)
            .Where(r => ids.Contains(r.Id))
            .ToListAsync();
    }
}