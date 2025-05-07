using AuthNexus.Domain.Entities;
using AuthNexus.Domain.Repositories;
using AuthNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthNexus.Infrastructure.Repositories;

/// <summary>
/// 权限定义仓储实现
/// </summary>
public class PermissionRepository : IPermissionRepository
{
    private readonly AuthNexusDbContext _dbContext;

    public PermissionRepository(AuthNexusDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// 根据ID获取权限定义
    /// </summary>
    public async Task<PermissionDefinition> GetByIdAsync(Guid id)
    {
        return await _dbContext.PermissionDefinitions
            .Include(p => p.Application)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// 根据应用ID和权限代码获取权限定义
    /// </summary>
    public async Task<PermissionDefinition> GetByCodeAsync(Guid applicationId, string code)
    {
        return await _dbContext.PermissionDefinitions
            .Include(p => p.Application)
            .FirstOrDefaultAsync(p => p.ApplicationId == applicationId && p.Code == code);
    }

    /// <summary>
    /// 获取应用的所有权限定义
    /// </summary>
    public async Task<IEnumerable<PermissionDefinition>> GetByApplicationIdAsync(Guid applicationId)
    {
        return await _dbContext.PermissionDefinitions
            .Include(p => p.Application)
            .Where(p => p.ApplicationId == applicationId)
            .ToListAsync();
    }

    /// <summary>
    /// 创建新权限定义
    /// </summary>
    public async Task<PermissionDefinition> CreateAsync(PermissionDefinition permission)
    {
        _dbContext.PermissionDefinitions.Add(permission);
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
            .Include(p => p.Application)
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();
    }
}