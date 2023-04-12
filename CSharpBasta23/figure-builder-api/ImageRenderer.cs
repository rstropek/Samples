public interface IImageRenderer
{
    byte[] Render(ImageOptions imageOptions, float scale);
}

public class ImageRenderer : IImageRenderer
{
    private readonly IImageComponentCache images;

    public ImageRenderer(IImageComponentCache images)
    {
        this.images = images;
    }

    public byte[] Render(ImageOptions imageOptions, float scale)
    {
        var width = (int)Math.Ceiling((double)(1024f * scale));
        var height = (int)Math.Ceiling((double)(1124f * scale));
        var origin = new SKPoint(0, 0);

        using var surface = SKSurface.Create(new SKImageInfo(width, height));
        using var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        canvas.Scale(scale);

        if (imageOptions.HasHammer) { canvas.DrawBitmap(images[Images.Hammer], origin); }
        if (imageOptions.HasTail) { canvas.DrawBitmap(images[Images.Tail], origin); }
        canvas.DrawBitmap(images[Images.BaseImage], origin);
        if (imageOptions.Eye != EyeType.NoEye) { canvas.DrawBitmap(images[Images.Eye1 + ((int)imageOptions.Eye - 1)], origin); }
        if (imageOptions.Mouth != MouthType.NoMouth) { canvas.DrawBitmap(images[Images.Mouth1 + ((int)imageOptions.Mouth - 1)], origin); }
        if (imageOptions.RightHand != RightHandType.NoHand) { canvas.DrawBitmap(images[Images.RightHand1 + ((int)imageOptions.RightHand - 1)], origin); }

        using var resultImage = surface.Snapshot();
        using var data = resultImage.Encode(SKEncodedImageFormat.Png, 75);

        return data.ToArray();
    }
}