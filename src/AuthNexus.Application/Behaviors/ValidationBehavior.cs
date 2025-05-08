using FluentValidation;
using MediatR;
using AuthNexus.SharedKernel.Exceptions;
using ValidationException = AuthNexus.SharedKernel.Exceptions.ValidationException;

namespace AuthNexus.Application.Behaviors
{
    /// <summary>
    /// 验证行为 - 用于自动验证请求数据
    /// </summary>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
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
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key, 
                        g => g.Select(x => x.ErrorMessage).ToArray()
                    );

                // 如果存在验证错误，则抛出异常
                if (failures.Count > 0)
                {
                    throw new ValidationException(failures);
                }
            }

            // 执行下一个管道行为或处理程序
            return await next();
        }
    }
}