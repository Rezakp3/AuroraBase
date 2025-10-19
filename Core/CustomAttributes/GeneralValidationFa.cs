using System.ComponentModel.DataAnnotations;
using Core.Const;

namespace Core.CustomAttributes;

public class RangeFaAttribute(double minimum, double maximum) : RangeAttribute(minimum, maximum)
{
    public override string FormatErrorMessage(string name)
        => $"مقدار {name} باید بین {Minimum} و {Maximum} باشد";
}

[AttributeUsage(AttributeTargets.Parameter)]
public class NumberRangeFaAttribute(double minimum, double maximum) : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return value is null || value.ToString()?.Length >= minimum && value.ToString()?.Length <= maximum;
    }
    public override string FormatErrorMessage(string name)
        => $"تعداد ارقام {name} باید بین {minimum} و {maximum} باشد";
}


public class StringLengthFaAttribute(int maximumLength) : StringLengthAttribute(maximumLength)
{
    public override string FormatErrorMessage(string name)
        => $"حداکثر تعداد کارکتر {name} {MaximumLength} حرف میباشد.";
}

public class MaxLengthFaAttribute(int maximumLength) : StringLengthFaAttribute(maximumLength);

public class RequiredFaAttribute : RequiredAttribute
{
    public RequiredFaAttribute()
    {
        ErrorMessage = Message.Error.RequiredAttribute;
    }
}

