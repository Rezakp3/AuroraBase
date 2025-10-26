using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

namespace Utils.Helpers;

public static class CaptchaHelper
{
    private static readonly Random Random = new();
    private static readonly char[] Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

    public static string GenerateCode(int length = 5)
    {
        // Generate a random string
        var captchaString = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            captchaString.Append(Characters[Random.Next(Characters.Length)]);
        }

        return captchaString.ToString();
    }

    public static string GenerateCaptcha(string? code = null,int linesCount = 3, int width = 150, int height = 50)
    {
        code ??= GenerateCode(5);

        // Create an image with noise and distortion
        using var bitmap = new Bitmap(width, height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.FromArgb(Random.Next(220, 255), Random.Next(220, 255), Random.Next(220, 255)));
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Add noise
        for (int i = 0; i < width * height / 30; i++)
        {
            bitmap.SetPixel(Random.Next(width), Random.Next(height), Color.FromArgb(Random.Next(256), Random.Next(256), Random.Next(256)));
        }

        // Draw the text with random rotation for each character
        var font = new Font("Segoe UI Light", 24, FontStyle.Regular);
        var brush = new SolidBrush(Color.Black);
        var textSize = graphics.MeasureString(code, font);
        var x = (width - textSize.Width) / 4;
        var y = (height - textSize.Height) / 2;

        foreach (var character in code)
        {
            var characterSize = graphics.MeasureString(character.ToString(), font);
            var characterX = x + characterSize.Width * Random.Next(-10, 10) / 100;
            var characterY = y + characterSize.Height * Random.Next(-10, 10) / 100;
            brush.Color = GetRandomColor();
            graphics.DrawString(character.ToString(), font, brush, characterX, characterY);
            x += (float)(characterSize.Width / 1.5);
        }

        // Add lines
        using (var pen = new Pen(Color.Black))
        {
            for (int i = 0; i < linesCount; i++)
            {
                pen.Color = GetRandomColor();
                graphics.DrawLine(pen, Random.Next(width), Random.Next(height), Random.Next(width), Random.Next(height));
            }
        }

        // Convert the image to Base64
        using var memoryStream = new MemoryStream();
        bitmap.Save(memoryStream, ImageFormat.Jpeg);
        return Convert.ToBase64String(memoryStream.ToArray());
    }

    private static Color GetRandomColor()
        => Color.FromArgb(Random.Next(80, 150), Random.Next(80, 150), Random.Next(80, 150));
}