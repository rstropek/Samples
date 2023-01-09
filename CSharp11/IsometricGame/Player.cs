using SkiaSharp;

public class Player
{
    private const float MAX_HEIGHT = 30f;

    private readonly Spritesheet sprites;
    private readonly float translationY;
    private float animationHeight = 0f;
    private float animationDirection = 1f;

    public Player(Spritesheet sprites, string spriteName, float translationY = 0f)
    {
        this.sprites = sprites;
        this.SpriteName = spriteName;
        this.translationY = translationY;
    }

    public void Draw(SKCanvas canvas)
    {
        animationHeight += animationDirection;
        if (animationHeight >= MAX_HEIGHT) { animationDirection = -1f; }
        else if (animationHeight <= 0) { animationDirection = 1; }

        canvas.Save();
        canvas.Translate(0f, translationY + animationHeight);
        canvas.DrawCenteredBitmap(sprites, SpriteName);
        canvas.Restore();
    }

    public SKPointI PlayerPosition { get; set; }
    public string SpriteName { get; set;}

    public void MoveUp() => PlayerPosition.Offset(0, -1);
    public void MoveDown() => PlayerPosition.Offset(0, 1);
    public void MoveLeft() => PlayerPosition.Offset(-1, 0);
    public void MoveRight() => PlayerPosition.Offset(1, 0);
}
