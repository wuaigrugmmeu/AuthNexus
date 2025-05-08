using AuthNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DomainEntities = AuthNexus.Domain.Entities;
using AuthNexus.Domain.Repositories;
using AuthNexus.Domain.Entities;

namespace AuthNexus.Infrastructure.Repositories
{
    /// <summary>
    /// 角色仓储实现
    /// </summary>
    public class RoleRepository : EFCoreGenericRepository<DomainEntities.Role>, IRoleRepository
    {
        public RoleRepository(AuthNexusDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// 根据名称获取角色
        /// </summary>
        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.Name == name);
        }

        /// <summary>
        /// 检查角色名是否已存在
        /// </summary>
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbSet.AnyAsync(r => r.Name == name);
        }

        /// <summary>
        /// 获取角色的权限
        /// </summary>
        public async Task<IReadOnlyList<Permission>> GetRolePermissionsAsync(Guid roleId)
        {
            var permissionIds = await _dbContext.RolePermissionAssignments
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionDefinitionId)
                .ToListAsync();

            return await _dbContext.PermissionDefinitions
                .Where(p => permissionIds.Contains(p.Id))
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
        /// 为角色分配权限
        /// </summary>
        public async Task AssignPermissionsToRoleAsync(Guid roleId, List<Guid> permissionIds)
        {
            var existingAssignments = await _dbContext.RolePermissionAssignments
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            var existingPermissionIds = existingAssignments.Select(rp => rp.PermissionDefinitionId).ToList();
            
            // 添加新的权限
            foreach (var permissionId in permissionIds)
            {
                if (!existingPermissionIds.Contains(permissionId))
                {
                    _dbContext.RolePermissionAssignments.Add(new DomainEntities.RolePermissionAssignment
                    {
                        Id = Guid.NewGuid(),
                        RoleId = roleId,
                        PermissionDefinitionId = permissionId
                    });
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 移除角色的权限
        /// </summary>
        public async Task RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId)
        {
            var assignment = await _dbContext.RolePermissionAssignments
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionDefinitionId == permissionId);

            if (assignment != null)
            {
                _dbContext.RolePermissionAssignments.Remove(assignment);
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 获取拥有此角色的用户
        /// </summary>
        public async Task<IReadOnlyList<User>> GetUsersInRoleAsync(Guid roleId)
        {
            var userIds = await _dbContext.UserRoleAssignments
                .Where(ur => ur.RoleId == roleId)
                .Select(ur => ur.UserIdentityId)
                .ToListAsync();

            return await _dbContext.UserIdentities
                .Where(u => userIds.Contains(u.Id))
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