using System.Security.Cryptography;
using System.Text;

namespace Utils.Helpers;

public static class CryptoHelper
{
    // کلید رمزنگاری باید دقیقاً 32 بایت (256 بیت) باشد.
    // حتماً این کلید را از کانفیگ (AppSettings یا UserSecrets) بخوانید و اینجا هاردکد نکنید.
    private static byte[] Key;

    public static void SetKey(string key)
        => Key = Encoding.UTF8.GetBytes(key);

    /// <summary>
    /// رمزنگاری یک متن ساده
    /// </summary>
    public static string Encrypt(this string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return string.Empty;

        using Aes aes = Aes.Create();
        aes.Key = Key;
        // تولید یک IV (Initialization Vector) رندوم برای امنیت بیشتر
        aes.GenerateIV();
        byte[] iv = aes.IV;

        using var encryptor = aes.CreateEncryptor(aes.Key, iv);
        using var ms = new MemoryStream();
        // ابتدا IV را در ابتدای استریم می‌نویسیم تا موقع دکریپت به آن دسترسی داشته باشیم
        ms.Write(iv, 0, iv.Length);

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }

        // تبدیل کل پکیج (IV + متن رمزنگاری شده) به یک رشته Base64
        return Convert.ToBase64String(ms.ToArray());
    }

    /// <summary>
    /// رمزگشایی یک متن رمزگذاری شده
    /// </summary>
    public static string Decrypt(this string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) return string.Empty;

        try
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);

            using Aes aes = Aes.Create();
            aes.Key = Key;

            // طول IV در الگوریتم AES همیشه 16 بایت است
            byte[] iv = new byte[16];
            byte[] cipher = new byte[fullCipher.Length - iv.Length];

            // جدا کردن IV و متن رمزنگاری شده اصلی از آرایه بایت‌ها
            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(cipher);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
        catch (CryptographicException)
        {
            // در صورتی که کلید اشتباه باشد یا متن دستکاری شده باشد
            return "خطا در رمزگشایی (احتمالاً کلید نامعتبر است)";
        }
    }
}