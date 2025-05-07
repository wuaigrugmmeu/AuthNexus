using AuthNexus.Application.Common;
using AuthNexus.Domain.Repositories;
using System.Security.Cryptography;
using System.Text;
using DomainEntities = AuthNexus.Domain.Entities;

namespace AuthNexus.Application.Applications;

/// <summary>
/// 应用管理服务实现
/// </summary>
public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _applicationRepository;

    public ApplicationService(IApplicationRepository applicationRepository)
    {
        _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
    }

    /// <summary>
    /// 注册新应用
    /// </summary>
    public async Task<ResultDto<ApplicationRegistrationResultDto>> RegisterApplicationAsync(RegisterApplicationRequest request)
    {
        try
        {
            // 检查应用标识是否已存在
            if (await _applicationRepository.ExistsByAppUidAsync(request.AppUID))
            {
                return ResultDto<ApplicationRegistrationResultDto>.Failure($"应用标识 '{request.AppUID}' 已存在");
            }

            // 生成API密钥和客户端密钥
            string apiKey = GenerateRandomKey();
            string clientSecret = GenerateRandomKey();

            // 创建应用实体
            var application = new DomainEntities.Application(
                request.AppUID,
                request.Name,
                HashString(apiKey),
                HashString(clientSecret),
                request.Description
            );

            // 保存应用
            var createdApplication = await _applicationRepository.CreateAsync(application);

            // 返回结果，包括生成的密钥（仅显示一次）
            return ResultDto<ApplicationRegistrationResultDto>.Success(new ApplicationRegistrationResultDto
            {
                Id = createdApplication.Id,
                AppUID = createdApplication.AppUID,
                Name = createdApplication.Name,
                ApiKey = apiKey,
                ClientSecret = clientSecret
            });
        }
        catch (Exception ex)
        {
            return ResultDto<ApplicationRegistrationResultDto>.Failure($"注册应用失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取应用信息
    /// </summary>
    public async Task<ResultDto<ApplicationDto>> GetApplicationAsync(string idOrAppUid)
    {
        try
        {
            DomainEntities.Application application = null;
            
            // 尝试将传入的字符串解析为GUID，以确定是按ID还是按AppUID查询
            if (Guid.TryParse(idOrAppUid, out Guid id))
            {
                // 按ID查询
                application = await _applicationRepository.GetByIdAsync(id);
            }
            else
            {
                // 按AppUID查询
                application = await _applicationRepository.GetByAppUidAsync(idOrAppUid);
            }
            
            if (application == null)
            {
                return ResultDto<ApplicationDto>.Failure($"应用 '{idOrAppUid}' 不存在");
            }

            return ResultDto<ApplicationDto>.Success(MapToDto(application));
        }
        catch (Exception ex)
        {
            return ResultDto<ApplicationDto>.Failure($"获取应用信息失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 更新应用信息
    /// </summary>
    public async Task<ResultDto<ApplicationDto>> UpdateApplicationAsync(string idOrAppUid, UpdateApplicationRequest request)
    {
        try
        {
            DomainEntities.Application application = null;
            
            // 尝试将传入的字符串解析为GUID，以确定是按ID还是按AppUID查询
            if (Guid.TryParse(idOrAppUid, out Guid id))
            {
                // 按ID查询
                application = await _applicationRepository.GetByIdAsync(id);
            }
            else
            {
                // 按AppUID查询
                application = await _applicationRepository.GetByAppUidAsync(idOrAppUid);
            }
            
            if (application == null)
            {
                return ResultDto<ApplicationDto>.Failure($"应用 '{idOrAppUid}' 不存在");
            }

            // 更新应用信息
            application.Update(request.Name, request.Description);
            
            // 保存更新
            var updatedApplication = await _applicationRepository.UpdateAsync(application);

            return ResultDto<ApplicationDto>.Success(MapToDto(updatedApplication));
        }
        catch (Exception ex)
        {
            return ResultDto<ApplicationDto>.Failure($"更新应用失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 启用应用
    /// </summary>
    public async Task<ResultDto> EnableApplicationAsync(string idOrAppUid)
    {
        try
        {
            DomainEntities.Application application = null;
            
            // 尝试将传入的字符串解析为GUID，以确定是按ID还是按AppUID查询
            if (Guid.TryParse(idOrAppUid, out Guid id))
            {
                // 按ID查询
                application = await _applicationRepository.GetByIdAsync(id);
            }
            else
            {
                // 按AppUID查询
                application = await _applicationRepository.GetByAppUidAsync(idOrAppUid);
            }
            
            if (application == null)
            {
                return ResultDto.Failure($"应用 '{idOrAppUid}' 不存在");
            }

            application.Enable();
            await _applicationRepository.UpdateAsync(application);

            return ResultDto.Success();
        }
        catch (Exception ex)
        {
            return ResultDto.Failure($"启用应用失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 禁用应用
    /// </summary>
    public async Task<ResultDto> DisableApplicationAsync(string idOrAppUid)
    {
        try
        {
            DomainEntities.Application application = null;
            
            // 尝试将传入的字符串解析为GUID，以确定是按ID还是按AppUID查询
            if (Guid.TryParse(idOrAppUid, out Guid id))
            {
                // 按ID查询
                application = await _applicationRepository.GetByIdAsync(id);
            }
            else
            {
                // 按AppUID查询
                application = await _applicationRepository.GetByAppUidAsync(idOrAppUid);
            }
            
            if (application == null)
            {
                return ResultDto.Failure($"应用 '{idOrAppUid}' 不存在");
            }

            application.Disable();
            await _applicationRepository.UpdateAsync(application);

            return ResultDto.Success();
        }
        catch (Exception ex)
        {
            return ResultDto.Failure($"禁用应用失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取所有应用
    /// </summary>
    public async Task<ResultDto<IEnumerable<ApplicationDto>>> GetAllApplicationsAsync()
    {
        try
        {
            var applications = await _applicationRepository.GetAllAsync();
            var applicationDtos = applications.Select(MapToDto);

            return ResultDto<IEnumerable<ApplicationDto>>.Success(applicationDtos);
        }
        catch (Exception ex)
        {
            return ResultDto<IEnumerable<ApplicationDto>>.Failure($"获取应用列表失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 验证应用凭据
    /// </summary>
    public async Task<ResultDto<bool>> ValidateCredentialsAsync(string appUid, string apiKey, string clientSecret)
    {
        try
        {
            var isValid = await _applicationRepository.ValidateCredentialsAsync(
                appUid, 
                HashString(apiKey), 
                HashString(clientSecret)
            );

            return ResultDto<bool>.Success(isValid);
        }
        catch (Exception ex)
        {
            return ResultDto<bool>.Failure($"验证应用凭据失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 重新生成应用密钥
    /// </summary>
    public async Task<ResultDto<ApplicationRegistrationResultDto>> RegenerateKeysAsync(Guid id)
    {
        try
        {
            var application = await _applicationRepository.GetByIdAsync(id);
            if (application == null)
            {
                return ResultDto<ApplicationRegistrationResultDto>.Failure($"应用ID '{id}' 不存在");
            }

            // 生成新的API密钥和客户端密钥
            string apiKey = GenerateRandomKey();
            string clientSecret = GenerateRandomKey();

            // 更新密钥哈希值
            application.UpdateKeys(
                HashString(apiKey),
                HashString(clientSecret)
            );

            // 保存更新
            var updatedApplication = await _applicationRepository.UpdateAsync(application);

            // 返回结果，包括新生成的密钥（仅显示一次）
            return ResultDto<ApplicationRegistrationResultDto>.Success(new ApplicationRegistrationResultDto
            {
                Id = updatedApplication.Id,
                AppUID = updatedApplication.AppUID,
                Name = updatedApplication.Name,
                ApiKey = apiKey,
                ClientSecret = clientSecret
            });
        }
        catch (Exception ex)
        {
            return ResultDto<ApplicationRegistrationResultDto>.Failure($"重新生成密钥失败: {ex.Message}");
        }
    }

    // 辅助方法

    /// <summary>
    /// 生成随机密钥
    /// </summary>
    private string GenerateRandomKey()
    {
        byte[] key = new byte[32]; // 256 bits
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(key);
        }
        return Convert.ToBase64String(key);
    }

    /// <summary>
    /// 对字符串进行哈希处理
    /// </summary>
    private string HashString(string input)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

    /// <summary>
    /// 将应用实体映射为DTO
    /// </summary>
    private ApplicationDto MapToDto(DomainEntities.Application application)
    {
        return new ApplicationDto
        {
            Id = application.Id,
            AppUID = application.AppUID,
            Name = application.Name,
            Description = application.Description,
            IsEnabled = application.IsEnabled,
            CreatedAt = application.CreatedAt,
            UpdatedAt = application.UpdatedAt
        };
    }
}