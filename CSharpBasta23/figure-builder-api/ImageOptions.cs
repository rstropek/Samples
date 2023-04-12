public enum EyeType { NoEye = 0, HalfOpen = 1, Closed = 2, Open = 3, }
public enum MouthType { NoMouth = 0, Happy = 1, Normal = 2, Unhappy = 3, }
public enum RightHandType { NoHand = 0, Normal = 1, Victory = 2, }

public record ImageOptions(EyeType Eye, bool HasHammer, MouthType Mouth, RightHandType RightHand, bool HasTail)
{
    public ImageOptions() : this(EyeType.NoEye, false, MouthType.NoMouth, RightHandType.NoHand, false) { }

    public static implicit operator byte(ImageOptions dto) =>
        (byte)((int)dto.Eye
        | (int)(dto.HasHammer ? 1 : 0) << 2
        | (int)dto.Mouth << 3
        | (int)dto.RightHand << 5
        | (int)(dto.HasTail ? 1 : 0) << 7);

    public static implicit operator ImageOptions(byte value)
    {
        var eye = (EyeType)(value & 0b11);
        if (!Enum.IsDefined(typeof(EyeType), eye))
        {
            throw new InvalidImageOptionsException($"Invalid eye type {(int)eye}");
        }

        var hasHammer = (value >> 2 & 0b1) == 1;

        var mouth = (MouthType)(value >> 3 & 0b11);
        if (!Enum.IsDefined(typeof(MouthType), mouth))
        {
            throw new InvalidImageOptionsException($"Invalid mouth type {(int)mouth}");
        }

        var rightHand = (RightHandType)(value >> 5 & 0b11);
        if (!Enum.IsDefined(typeof(RightHandType), rightHand))
        {
            throw new InvalidImageOptionsException($"Invalid right hand type {(int)rightHand}");
        }

        var hasTail = (value >> 7 & 0b1) == 1;

        return new ImageOptions(eye, hasHammer, mouth, rightHand, hasTail);
    }
}

class ImageOptionsValidator : AbstractValidator<ImageOptions>
{
    public ImageOptionsValidator()
    {
        RuleFor(x => x.Eye).IsInEnum();
        RuleFor(x => x.Mouth).IsInEnum();
        RuleFor(x => x.RightHand).IsInEnum();
    }
}

class InvalidImageOptionsException : Exception
{
    public InvalidImageOptionsException(string message) : base(message) { }
    public InvalidImageOptionsException(IEnumerable<ValidationFailure> failures)
        : this(string.Join(", ", failures.Select(e => $"{e.ErrorMessage}"))) { }
}
