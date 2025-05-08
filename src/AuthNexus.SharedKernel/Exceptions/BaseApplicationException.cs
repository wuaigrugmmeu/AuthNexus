namespace AuthNexus.SharedKernel.Exceptions
{
    /// <summary>
    /// 应用异常基类
    /// </summary>
    public abstract class BaseApplicationException : Exception
    {
        public int StatusCode { get; }

        protected BaseApplicationException(string message, int statusCode = 500) 
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

    /// <summary>
    /// 实体未找到异常
    /// </summary>
    public class EntityNotFoundException : BaseApplicationException
    {
        public EntityNotFoundException(string entityName, object id)
            : base($"{entityName} with id {id} was not found.", 404)
        {
        }
    }

    /// <summary>
    /// 验证失败异常
    /// </summary>
    public class ValidationException : BaseApplicationException
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; }

        public ValidationException(IReadOnlyDictionary<string, string[]> errors)
            : base("One or more validation errors occurred.", 400)
        {
            Errors = errors;
        }
    }

    /// <summary>
    /// 未授权异常
    /// </summary>
    public class UnauthorizedException : BaseApplicationException
    {
        public UnauthorizedException(string message = "You are not authorized to perform this action.")
            : base(message, 401)
        {
        }
    }

    /// <summary>
    /// 禁止访问异常
    /// </summary>
    public class ForbiddenException : BaseApplicationException
    {
        public ForbiddenException(string message = "You do not have permission to perform this action.")
            : base(message, 403)
        {
        }
    }
}