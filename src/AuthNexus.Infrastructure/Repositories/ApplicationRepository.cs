using AuthNexus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DomainEntities = AuthNexus.Domain.Entities;
using AuthNexus.Domain.Repositories;

namespace AuthNexus.Infrastructure.Repositories;

/// <summary>
/// 应用仓储实现
/// </summary>
public class ApplicationRepository : IApplicationRepository
{
    private readonly AuthNexusDbContext _dbContext;

    public ApplicationRepository(AuthNexusDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// 根据ID获取应用
    /// </summary>
    public async Task<DomainEntities.Application> GetByIdAsync(Guid id)
    {
        return await _dbContext.Applications
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    /// <summary>
    /// 根据AppUID获取应用，同时支持通过ID查询
    /// </summary>
    public async Task<DomainEntities.Application> GetByAppUidAsync(string appUid)
    {
        // 尝试将appUid解析为Guid，如果成功则按ID查询
        if (Guid.TryParse(appUid, out Guid id))
        {
            Console.WriteLine($"尝试通过ID查找应用: {appUid}");
            return await _dbContext.Applications
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        
        Console.WriteLine($"尝试通过AppUID查找应用: {appUid}");
        return await _dbContext.Applications
            .FirstOrDefaultAsync(a => a.AppUID == appUid);
    }

    /// <summary>
    /// 验证应用访问凭据
    /// </summary>
    public async Task<bool> ValidateCredentialsAsync(string appUid, string hashedApiKey, string hashedClientSecret)
    {
        var application = await _dbContext.Applications
            .FirstOrDefaultAsync(a => a.AppUID == appUid);

        if (application == null || !application.IsEnabled)
        {
            return false;
        }

        return application.HashedApiKey == hashedApiKey && 
               application.HashedClientSecret == hashedClientSecret;
    }

    /// <summary>
    /// 创建新应用
    /// </summary>
    public async Task<DomainEntities.Application> CreateAsync(DomainEntities.Application application)
    {
        _dbContext.Applications.Add(application);
        await _dbContext.SaveChangesAsync();
        return application;
    }

    /// <summary>
    /// 更新应用
    /// </summary>
    public async Task<DomainEntities.Application> UpdateAsync(DomainEntities.Application application)
    {
        _dbContext.Applications.Update(application);
        await _dbContext.SaveChangesAsync();
        return application;
    }

    /// <summary>
    /// 获取所有应用
    /// </summary>
    public async Task<IEnumerable<DomainEntities.Application>> GetAllAsync()
    {
        return await _dbContext.Applications
            .ToListAsync();
    }

    /// <summary>
    /// 检查应用UID是否已存在
    /// </summary>
    public async Task<bool> ExistsByAppUidAsync(string appUid)
    {
        return await _dbContext.Applications
            .AnyAsync(a => a.AppUID == appUid);
    }
}