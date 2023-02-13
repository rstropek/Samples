public interface IImageOptionBuilder
{
    IImageOptionBuilder WithEye(EyeType eye);
    IImageOptionBuilder WithMouth(MouthType mouth);
    IImageOptionBuilder WithRightHand(RightHandType rightHand);
    IImageOptionBuilder WithTail();
    IImageOptionBuilder WithoutTail();
    IImageOptionBuilder WithHammer();
    IImageOptionBuilder WithoutHammer();
    ImageOptions Build();
}

public class ImageOptionBuilder : IImageOptionBuilder
{
    private EyeType withEye = EyeType.NoEye;
    private MouthType withMouth = MouthType.NoMouth;
    private RightHandType withRightHand = RightHandType.NoHand;
    private bool withTail = false;
    private bool withHammer = false;

    public ImageOptionBuilder() { }

    public IImageOptionBuilder WithEye(EyeType eye) { withEye = eye; return this; }
    public IImageOptionBuilder WithHammer() { withHammer = true; return this; }
    public IImageOptionBuilder WithoutHammer() { withHammer = false; return this; }
    public IImageOptionBuilder WithMouth(MouthType mouth) { withMouth = mouth; return this; }
    public IImageOptionBuilder WithRightHand(RightHandType rightHand) { withRightHand = rightHand; return this; }
    public IImageOptionBuilder WithTail() { withTail = true; return this; }
    public IImageOptionBuilder WithoutTail() { withTail = false; return this; }
    public ImageOptions Build() => new ImageOptions(withEye, withHammer, withMouth, withRightHand, withTail);
}

static class IImageOptionBuilderExtensions
{
    public static IImageOptionBuilder Random(this IImageOptionBuilder builder)
    {
        static T PickRandom<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(System.Random.Shared.Next(values.Length))!;
        }

        bool PickRandomBool() => System.Random.Shared.Next(2) == 0;

        var b = builder
            .WithEye(PickRandom<EyeType>())
            .WithMouth(PickRandom<MouthType>())
            .WithRightHand(PickRandom<RightHandType>());
        if (PickRandomBool()) { b = b.WithTail(); }
        if (PickRandomBool()) { b = b.WithHammer(); }
        return b;
    }

    public static IImageOptionBuilder Happy(this IImageOptionBuilder builder)
        => builder
            .WithEye(EyeType.Open)
            .WithMouth(MouthType.Happy)
            .WithRightHand(RightHandType.Victory);
}
