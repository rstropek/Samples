public enum Images
{
    BaseImage, Eye1, Eye2, Eye3, Hammer, Mouth1, Mouth2,
    Mouth3, RightHand1, RightHand2, Tail,
}

public interface IImageComponentCache
{
    SKBitmap this[Images image] { get; }
}

public class ImageComponentCache : IImageComponentCache
{
    private readonly Dictionary<Images, SKBitmap> _images = new();

    public ImageComponentCache() {}

    public IImageComponentCache Fill()
    {
        void AddImage(Images image, string name)
        {
            using (var imageStream = File.OpenRead(Path.Combine("Images", $"{name}.png")))
            {
                _images[image] = SKBitmap.Decode(imageStream);
            }
        }

        AddImage(Images.BaseImage, "base");
        AddImage(Images.Eye1, "eye1");
        AddImage(Images.Eye2, "eye2");
        AddImage(Images.Eye3, "eye3");
        AddImage(Images.Hammer, "hammer");
        AddImage(Images.Mouth1, "mouth1");
        AddImage(Images.Mouth2, "mouth2");
        AddImage(Images.Mouth3, "mouth3");
        AddImage(Images.RightHand1, "rightHand1");
        AddImage(Images.RightHand2, "rightHand2");
        AddImage(Images.Tail, "tail");

        return this;
    }

    /// <summary>
    /// Indexer that gets the image component with the specified <paramref name="image"/>.
    /// </summary>
    public SKBitmap this[Images image] => _images[image];
}