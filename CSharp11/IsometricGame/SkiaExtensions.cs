using SkiaSharp;

public static class SkiaExtensions
{
    public static void DrawCenteredBitmap(this SKCanvas canvas, Spritesheet spritesheet, string spriteName, SKSize? destSize = null)
    {
        var sprite = spritesheet.Frames[spriteName];
        var bitmapPaint = new SKPaint
        {
            FilterQuality = SKFilterQuality.High,
            IsAntialias = false,
            IsDither = false,
        };

        SKRect destRect;
        if (destSize.HasValue)
        {
            destRect = new SKRect(-destSize.Value.Width / 2, -destSize.Value.Height / 2, destSize.Value.Width / 2, destSize.Value.Height / 2);
        }
        else
        {
            destRect = new SKRect(-sprite.SourceSize.Width / 2, -sprite.SourceSize.Height / 2, sprite.SourceSize.Width / 2, sprite.SourceSize.Height / 2);
        }

        canvas.DrawImage(spritesheet.Image, sprite.FrameCoordinates, destRect, bitmapPaint);
    }
}