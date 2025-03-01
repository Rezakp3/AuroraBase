namespace Core.Const;


public static class Message
{
    public static class Error
    {
        public const string Common = "هنگام انجام عملیات خطایی رخ داده است !";
        public const string FailedCommon = "عملیات انجام نشد";
        public const string RequiredAttribute = "لطفا {0} را وارد کنید !";
        public const string MaxLength = "حداکثر تعداد کارکتر {0} 255 عدد میباشد.";

        public static string NotFound(string value = "")
            => $" {value} یافت نشد "; 

        public static string Required(string value)
            => $"{value} اجباری است";
    }

    public static class Success
    {
        public const string Common = "عملیات با موفقبت انجام شد";
    }
}