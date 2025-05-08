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