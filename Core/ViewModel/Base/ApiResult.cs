namespace Core.ViewModel.Base;

public class ApiResult(int code = 200, bool isSuccess = true)
{
    public bool IsSuccess { get; set; } = isSuccess;

    private string? _message;
    public string? Message
    {
        set { _message = value; }
        get
        {
            if (!string.IsNullOrEmpty(_message))
                return _message;
            else
                return ResourceConfig.ResourcesFa?.GetString(Code.ToString());
        }
    }

    public IEnumerable<ValidateProperty>? ValidationErrors { get; set; }

    public int Code
    {
        get => code;
        set { code = value; }
    }

    #region static methods

    public static ApiResult Success(string? message = null)
        => new(200, true) { Message = message };
    public static ApiResult Fail(int code = 400, string? message = null)
        => new(code, false) { Message = message };
    public static ApiResult NotFound(string? fieldName = null)
        => new(404, false) { Message = fieldName is null ? null : $"یافت نشد {fieldName}" };

    #endregion

    #region Navigation

    public ApiResult<T> ToGeneric<T>()
        => (ApiResult<T>)this;

    #endregion
}
public class ApiResult<T>(int code = 200, bool isSuccess = true) : ApiResult(code, isSuccess)
{
    public T? Data { get; set; }

    #region static methods

    public static ApiResult<T> Success(T? data = default, string? message = null)
        => new(200, true) { Data = data, Message = message };

    public static ApiResult<T> Fail(T? data = default, int code = 400, string? message = null)
        => new(code, false) { Data = data, Message = message };


    #endregion

    #region navigation

    public ApiResult ToResult()
        => this;

    #endregion
}
public class ValidateProperty
{
    public string? PropertyName { get; set; }
    public List<string>? Errors { get; set; }
}