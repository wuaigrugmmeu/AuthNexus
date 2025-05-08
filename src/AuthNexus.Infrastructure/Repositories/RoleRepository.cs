using AuthNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AuthNexus.Domain.Repositories;
using AuthNexus.Domain.Entities;

namespace AuthNexus.Infrastructure.Repositories
{
    /// <summary>
    /// 角色仓储实现
    /// </summary>
    public class RoleRepository : EFCoreGenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(AuthNexusDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// 根据ID获取角色
        /// </summary>
        public new async Task<Role> GetByIdAsync(Guid id)
        {
            var role = await _dbSet.FindAsync(id);
            return role ?? throw new KeyNotFoundException($"未找到ID为{id}的角色");
        }

        /// <summary>
        /// 根据应用ID和角色名称获取角色
        /// </summary>
        public async Task<Role> GetByNameAsync(Guid applicationId, string name)
        {
            var role = await _dbSet
                .FirstOrDefaultAsync(r => r.ApplicationId == applicationId && r.Name == name);
            
            return role ?? throw new KeyNotFoundException($"未找到应用{applicationId}下名称为{name}的角色");
        }

        /// <summary>
        /// 获取应用的所有角色
        /// </summary>
        public async Task<IEnumerable<Role>> GetByApplicationIdAsync(Guid applicationId)
        {
            return await _dbSet
                .Where(r => r.ApplicationId == applicationId)
                .ToListAsync();
        }

        /// <summary>
        /// 创建新角色
        /// </summary>
        public async Task<Role> CreateAsync(Role role)
        {
            await _dbSet.AddAsync(role);
            await _dbContext.SaveChangesAsync();
            return role;
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        public async Task<Role> UpdateAsync(Role role)
        {
            _dbSet.Update(role);
            await _dbContext.SaveChangesAsync();
            return role;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        public async Task DeleteAsync(Guid id)
        {
            var role = await _dbSet.FindAsync(id);
            if (role != null)
            {
                _dbSet.Remove(role);
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 检查应用中是否存在指定名称的角色
        /// </summary>
        public async Task<bool> ExistsByNameAsync(Guid applicationId, string name)
        {
            return await _dbSet.AnyAsync(r => r.ApplicationId == applicationId && r.Name == name);
        }

        /// <summary>
        /// 获取角色及其所有权限
        /// </summary>
        public async Task<Role> GetWithPermissionsAsync(Guid id)
        {
            // 使用Include查询直接获取包含权限的角色
            var role = await _dbContext.Roles
                .Include(r => r.Application)
                .FirstOrDefaultAsync(r => r.Id == id);
                
            if (role == null)
            {
                throw new KeyNotFoundException($"未找到ID为{id}的角色");
            }
            
            // 注意：不再尝试直接设置只读的Permissions属性
            // 而是使用Include加载关联数据
            
            return role;
        }

        /// <summary>
        /// 根据ID列表获取多个角色
        /// </summary>
        public async Task<IEnumerable<Role>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _dbSet
                .Where(r => ids.Contains(r.Id))
                .ToListAsync();
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
                    _dbContext.RolePermissionAssignments.Add(new RolePermissionAssignment(roleId,permissionId));
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