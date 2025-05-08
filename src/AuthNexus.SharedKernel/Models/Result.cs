namespace AuthNexus.SharedKernel.Models
{
    /// <summary>
    /// 表示通用操作结果封装
    /// </summary>
    /// <typeparam name="T">结果数据类型</typeparam>
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public string? Error { get; private set; }
        public int? ErrorCode { get; private set; }

        private Result(bool isSuccess, T? data, string? error, int? errorCode)
        {
            IsSuccess = isSuccess;
            Data = data;
            Error = error;
            ErrorCode = errorCode;
        }

        public static Result<T> Success(T data) => new Result<T>(true, data, null, null);
        public static Result<T> Failure(string error, int errorCode = 400) => new Result<T>(false, default, error, errorCode);
    }

    /// <summary>
    /// 无数据返回的结果封装
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string? Error { get; private set; }
        public int? ErrorCode { get; private set; }

        private Result(bool isSuccess, string? error, int? errorCode)
        {
            IsSuccess = isSuccess;
            Error = error;
            ErrorCode = errorCode;
        }

        public static Result Success() => new Result(true, null, null);
        public static Result Failure(string error, int errorCode = 400) => new Result(false, error, errorCode);
        public static Result<T> Success<T>(T data) => Result<T>.Success(data);
        public static Result<T> Failure<T>(string error, int errorCode = 400) => Result<T>.Failure(error, errorCode);
    }
}