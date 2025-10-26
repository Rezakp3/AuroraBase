namespace Application.Common.Models;

public class ApiResult(int code = 200, bool isSuccess = true)
{
    public bool IsSuccess { get; set; } = isSuccess;

    private string? _message;
    public string? Message
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
    public IEnumerable<ValidationError>? ValidationErrors { get; set; }

    public static ApiResult Success(string? message = null)
        => new(200, true) { Message = message };

    public static ApiResult Fail(int code = 400, string? message = null)
        => new(code, false) { Message = message };

    public static ApiResult NotFound(string? fieldName = null)
        => new(404, false) { Message = fieldName is null ? null : $"{fieldName} یافت نشد" };
}

public class ApiResult<T> : ApiResult
{
    public T? Data { get; set; }

    public ApiResult(int code = 200, bool isSuccess = true) : base(code, isSuccess) { }

    public static ApiResult<T> Success(T? data = default, string? message = null)
        => new(200, true) { Data = data, Message = message };

    new public static ApiResult<T> Fail(int code = 400, string? message = null)
        => new(code, false) { Message = message };

    new public static ApiResult<T> NotFound(string? fieldName = null)
        => new(404, false) { Message = fieldName is null ? null : $"{fieldName} یافت نشد" };
}

public class ValidationError
{
    public string? PropertyName { get; set; }
    public List<string>? Errors { get; set; }
}