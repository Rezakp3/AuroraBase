using System.Security.Cryptography.X509Certificates;

namespace Application.Common.Models;

public class ApiResult(int code = 200, bool isSuccess = true)
{
    public bool IsSuccess { get; set; } = isSuccess;

    private string _message;
    public string Message
    {
        get
        {
            if (!string.IsNullOrEmpty(_message))
                return _message;

            // استفاده از Resource اگر موجود باشد
            return ResourceConfig.ResourcesFa?.GetString(Code.ToString());
        }
        set => _message = value;
    }

    public int Code { get; set; } = code;
    public IEnumerable<ValidationError> ValidationErrors { get; set; }

    public static ApiResult Success(string message = null)
        => new(200, true) { Message = message };

    public static ApiResult Fail(string message = null, int code = 400)
        => new(code, false) { Message = message };

    public static ApiResult NotFound(string fieldName = null)
        => new(404, false) { Message = fieldName is null ? null : $"{fieldName} یافت نشد" };
}

public class ApiResult<T>(int code = 200, bool isSuccess = true) : ApiResult(code, isSuccess)
{
    public T Data { get; set; }

    public static ApiResult<T> Success(T data = default, string message = null)
        => new(200, true) { Data = data, Message = message };

    new public static ApiResult<T> Fail(string message = null, int code = 400)
        => new(code, false) { Message = message };

    new public static ApiResult<T> NotFound(string fieldName = null)
        => new(404, false) { Message = fieldName is null ? null : $"{fieldName} یافت نشد" };
}

public static class ApiResultHelper
{
    extension(ApiResult apiResult)
    {
        public ApiResult Success(int code = 200, string message = null)
        {
            apiResult.IsSuccess = true;
            apiResult.Code = code;
            apiResult.Message = message;
            return apiResult;
        }
        public ApiResult Success(int code = 200)
            => apiResult.Success(code, null);
        public ApiResult Success(string message = null)
            => apiResult.Success(200, message);
        public ApiResult Success()
            => apiResult.Success(200, null);



        public ApiResult Fail(int code = 400, string message = null)
        {
            apiResult.IsSuccess = false;
            apiResult.Code = code;
            apiResult.Message = message;
            return apiResult;
        }
        public ApiResult Fail(int code = 400)
            => apiResult.Fail(code, null);
        public ApiResult Fail(string message = null)
            => apiResult.Fail(400, message);
        public ApiResult Fail()
            => apiResult.Fail(400, null);
    }

    extension<T>(ApiResult<T> apiResult)
    {
        public ApiResult<T> Success(T data = default, int code = 200, string message = null)
        {
            apiResult.IsSuccess = true;
            apiResult.Code = code;
            apiResult.Data = data;
            apiResult.Message = message;
            return apiResult;
        }
        public ApiResult<T> Success(T data = default, int code = 200)
            => apiResult.Success(data, code, null);
        public ApiResult<T> Success(T data = default, string message = null)
            => apiResult.Success(data, 200, message);
        public ApiResult<T> Success(T data = default)
            => apiResult.Success(data, 200, null);

        public ApiResult<T> Fail(int code = 400, string message = null)
        {
            apiResult.IsSuccess = false;
            apiResult.Code = code;
            apiResult.Message = message;
            return apiResult;
        }
        public ApiResult<T> Fail(int code = 400)
            => apiResult.Fail(code, null);
        public ApiResult<T> Fail(string message = null)
            => apiResult.Fail(400, message);
        public ApiResult<T> Fail()
            => apiResult.Fail(400, null);
    }
}
public class ValidationError
{
    public string? PropertyName { get; set; }
    public List<string>? Errors { get; set; }
}
