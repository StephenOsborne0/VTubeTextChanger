using SkiaSharp;

namespace VTubeStudioTextSwapper;

public class TextToPngRenderer(string outputPath, string fontName = "Arial", int fontSize = 48, int padding = 20)
{
    public void RenderTextToPng(string text)
    {
        // Ensure the directory exists
        string directory = Path.GetDirectoryName(outputPath) ?? "";
        
        if (!Directory.Exists(directory)) 
            Directory.CreateDirectory(directory);
        
        // Create a paint object for text
        using SKPaint paint = new();
        paint.TextSize = fontSize;
        paint.Typeface = SKTypeface.FromFamilyName(fontName);
        paint.IsAntialias = true;
        paint.Color = SKColors.Black;
        paint.TextAlign = SKTextAlign.Left;

        // Measure text size
        SKRect textBounds = new();
        paint.MeasureText(text, ref textBounds);

        // Apply padding
        int width = (int)Math.Ceiling(textBounds.Width + (padding * 2));
        int height = (int)Math.Ceiling(-textBounds.Top + textBounds.Bottom + (padding * 2));

        // Create a new bitmap with padding
        using (SKBitmap bitmap = new(width, height))
        using (SKCanvas canvas = new(bitmap))
        {
            canvas.Clear(SKColors.Transparent);

            // Draw text with padding applied
            float textX = padding;
            float textY = padding - textBounds.Top; // Adjust for baseline offset
            canvas.DrawText(text, textX, textY, paint);

            // Save the bitmap as a PNG
            using (SKImage image = SKImage.FromBitmap(bitmap))
            using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                File.WriteAllBytes(outputPath, data.ToArray());
            }
        }

        Console.WriteLine($"PNG saved at: {outputPath}");
    }
}