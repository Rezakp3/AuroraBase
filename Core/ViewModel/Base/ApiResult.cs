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
}

public class ValidateProperty
{
    public string? PropertyName { get; set; }
    public List<string>? Errors { get; set; }
}
public class ApiResult<T>(int code = 200, bool isSuccess = true) : ApiResult(code, isSuccess)
{
    public T? Data { get; set; }
}