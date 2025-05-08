using AuthNexus.Domain.Entities;
using AuthNexus.Domain.Repositories;
using AuthNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthNexus.Infrastructure.Repositories
{
    /// <summary>
    /// 权限仓储实现
    /// </summary>
    public class PermissionRepository : EFCoreGenericRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(AuthNexusDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// 根据ID获取权限定义
        /// </summary>
        public new async Task<PermissionDefinition> GetByIdAsync(Guid id)
        {
            var permission = await _dbContext.PermissionDefinitions
                .FindAsync(id);
            
            return permission ?? throw new KeyNotFoundException($"未找到ID为{id}的权限定义");
        }
        
        /// <summary>
        /// 根据应用ID和权限代码获取权限定义
        /// </summary>
        public async Task<PermissionDefinition> GetByCodeAsync(Guid applicationId, string code)
        {
            var permission = await _dbContext.PermissionDefinitions
                .FirstOrDefaultAsync(p => p.ApplicationId == applicationId && p.Code == code);
            
            return permission ?? throw new KeyNotFoundException($"未找到应用{applicationId}下代码为{code}的权限定义");
        }
        
        /// <summary>
        /// 获取应用的所有权限定义
        /// </summary>
        public async Task<IEnumerable<PermissionDefinition>> GetByApplicationIdAsync(Guid applicationId)
        {
            return await _dbContext.PermissionDefinitions
                .Where(p => p.ApplicationId == applicationId)
                .ToListAsync();
        }
        
        /// <summary>
        /// 创建新权限定义
        /// </summary>
        public async Task<PermissionDefinition> CreateAsync(PermissionDefinition permission)
        {
            await _dbContext.PermissionDefinitions.AddAsync(permission);
            await _dbContext.SaveChangesAsync();
            return permission;
        }
        
        /// <summary>
        /// 更新权限定义
        /// </summary>
        public async Task<PermissionDefinition> UpdateAsync(PermissionDefinition permission)
        {
            _dbContext.PermissionDefinitions.Update(permission);
            await _dbContext.SaveChangesAsync();
            return permission;
        }
        
        /// <summary>
        /// 删除权限定义
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            var permission = await _dbContext.PermissionDefinitions.FindAsync(id);
            if (permission != null)
            {
                _dbContext.PermissionDefinitions.Remove(permission);
                await _dbContext.SaveChangesAsync();
            }
        }
        
        /// <summary>
        /// 检查应用中是否存在指定代码的权限
        /// </summary>
        public async Task<bool> ExistsByCodeAsync(Guid applicationId, string code)
        {
            return await _dbContext.PermissionDefinitions
                .AnyAsync(p => p.ApplicationId == applicationId && p.Code == code);
        }
        
        /// <summary>
        /// 根据ID列表获取多个权限定义
        /// </summary>
        public async Task<IEnumerable<PermissionDefinition>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _dbContext.PermissionDefinitions
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();
        }

        /// <summary>
        /// 根据名称获取权限
        /// </summary>
        public async Task<Permission?> GetByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Name == name);
        }

        /// <summary>
        /// 检查权限名是否已存在
        /// </summary>
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbSet.AnyAsync(p => p.Name == name);
        }

        /// <summary>
        /// 获取拥有此权限的角色
        /// </summary>
        public async Task<IReadOnlyList<Role>> GetRolesWithPermissionAsync(Guid permissionId)
        {
            var roleIds = await _dbContext.RolePermissionAssignments
                .Where(rp => rp.PermissionDefinitionId == permissionId)
                .Select(rp => rp.RoleId)
                .ToListAsync();

            return await _dbContext.Roles
                .Where(r => roleIds.Contains(r.Id))
                .ToListAsync();
        }

        /// <summary>
        /// 获取拥有此权限的用户
        /// </summary>
        public async Task<IReadOnlyList<User>> GetUsersWithPermissionAsync(Guid permissionId)
        {
            // 获取直接拥有该权限的用户
            var directUserIds = await _dbContext.UserDirectPermissionAssignments
                .Where(up => up.PermissionDefinitionId == permissionId)
                .Select(up => up.UserIdentityId)
                .ToListAsync();

            // 获取通过角色拥有该权限的用户
            var roleIds = await _dbContext.RolePermissionAssignments
                .Where(rp => rp.PermissionDefinitionId == permissionId)
                .Select(rp => rp.RoleId)
                .ToListAsync();

            var userIdsFromRoles = await _dbContext.UserRoleAssignments
                .Where(ur => roleIds.Contains(ur.RoleId))
                .Select(ur => ur.UserIdentityId)
                .ToListAsync();

            // 合并两组用户ID并去重
            var allUserIds = directUserIds.Union(userIdsFromRoles).Distinct();

            // 获取用户信息
            return await _dbContext.UserIdentities
                .Where(u => allUserIds.Contains(u.Id))
                .Select(u => new User
                {
                    Id = u.Id,
                    Username = u.ExternalUserId, // 假设ExternalUserId是用户名
                    Email = string.Empty // 假设不需要填充邮箱
                })
                .ToListAsync();
        }
    }
}