using FluentValidation;
using MediatR;
using AuthNexus.Application.Common;

namespace AuthNexus.Application.Behaviors
{
    /// <summary>
    /// 验证行为 - 用于自动验证请求数据
    /// </summary>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                // 执行所有验证器并聚合结果
                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                // 收集验证错误
                var failures = validationResults
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .ToList();

                // 如果存在验证错误
                if (failures.Count > 0)
                {
                    // 根据 TResponse 类型处理验证失败
                    // 判断 TResponse 是否为 ResultDto<T> 类型
                    if (typeof(TResponse).IsGenericType && 
                        typeof(TResponse).GetGenericTypeDefinition() == typeof(ResultDto<>))
                    {
                        // 构建错误消息
                        var errorMessage = string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));
                        
                        // 创建一个失败的 ResultDto<T> 实例
                        var resultType = typeof(TResponse);
                        var resultGenericArg = resultType.GetGenericArguments()[0];
                        var failureMethod = typeof(ResultDto<>)
                            .MakeGenericType(resultGenericArg)
                            .GetMethod("Failure", new[] { typeof(string) });
                        
                        if (failureMethod == null)
                        {
                            throw new InvalidOperationException($"无法找到 ResultDto<{resultGenericArg.Name}>.Failure 方法");
                        }

                        var result = failureMethod.Invoke(null, new object[] { errorMessage });
                        if (result == null)
                        {
                            throw new InvalidOperationException($"ResultDto<{resultGenericArg.Name}>.Failure 方法返回了 null");
                        }
                        
                        return (TResponse)result;
                    }
                    else if (typeof(TResponse) == typeof(ResultDto))
                    {
                        // 处理非泛型 ResultDto
                        var errorMessage = string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));
                        var result = ResultDto.Failure(errorMessage);
                        if (result == null)
                        {
                            throw new InvalidOperationException("ResultDto.Failure 方法返回了 null");
                        }
                        return (TResponse)(object)result;
                    }
                    else
                    {
                        // 对于不是 ResultDto 类型的响应，仍然抛出异常
                        var failuresDict = failures
                            .GroupBy(x => x.PropertyName)
                            .ToDictionary(
                                g => g.Key, 
                                g => g.Select(x => x.ErrorMessage).ToArray()
                            );
                        
                        var errorMessage = string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));
                        throw new ValidationException(errorMessage);
                    }
                }
            }

            // 执行下一个管道行为或处理程序
            return await next();
        }
    }
}