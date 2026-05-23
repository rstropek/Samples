using System.Numerics;
using SkiaSharp;

namespace AvaloniaFireworks;

class Particle(float colorHue, Vector2 startPosition, Vector2 launchVelociy, float strokeWidth)
{
    private static readonly Vector2 Gravity = new(0, 0.2f);

    protected const int NumberOfPreviousPositions = 20;

    protected float colorHue = colorHue;
    protected Vector2 position = startPosition;
    protected Vector2 velocity = launchVelociy;
    protected float strokeWidth = strokeWidth;
    protected readonly List<Vector2> previousPositions = [];

    public virtual void Draw(SKCanvas canvas)
    {
        SKPaint CreatePaint(float hue, float lightness) => new SKPaint()
        {
            Color = SKColor.FromHsv(hue, 100, lightness),
            StrokeWidth = strokeWidth,
            StrokeCap = SKStrokeCap.Round
        };

        foreach (var (previousPosition, index) in previousPositions.Select((p, i) => (p, i)))
        {
            using var previousPaint = CreatePaint(colorHue, 100f * (index + 1) / NumberOfPreviousPositions);
            canvas.DrawPoint(previousPosition.X, previousPosition.Y, previousPaint);
        }

        using var paint = CreatePaint(colorHue, 100);
        canvas.DrawPoint(position.X, position.Y, paint);
    }

    public virtual void ApplyForce()
    {
        position += velocity;
        while (previousPositions.Count >= NumberOfPreviousPositions)
        {
            previousPositions.RemoveAt(0);
        }

        previousPositions.Add(position);

        velocity += Gravity;
    }
}

enum RocketState
{
    Launching,
    Exploding,
    Done
}

class Rocket(float? colorHue = null, float? x = null, float? launchVelocityY = null, float? spread = null, float canvasWidth = 800f, float canvasHeight = 600f) : Particle(
        colorHue: colorHue ?? RandomFloat.NextFloat(0, 360f),
        startPosition: new Vector2(x ?? RandomFloat.NextFloat(0, canvasWidth), canvasHeight),
        launchVelociy: new Vector2(0, -Math.Abs(launchVelocityY ?? (float)Math.Floor(RandomFloat.NextFloat(10f, 15f)))),
        strokeWidth: 4f)
{
    private readonly float spread = spread ?? RandomFloat.NextFloat(10f, 30f);

    public RocketState State { get; private set; } = RocketState.Launching;

    private readonly List<ExplosionParticle> particles = [];

    public override void Draw(SKCanvas canvas)
    {
        if (State == RocketState.Done) { return; }

        ApplyForce();

        switch (State)
        {
            case RocketState.Launching:
                base.Draw(canvas);
                break;
            case RocketState.Exploding:
                foreach (var particle in particles) { particle.Draw(canvas); }
                break;
            default:
                break;
        }
    }

    public override void ApplyForce()
    {
        switch (State)
        {
            case RocketState.Launching:
                base.ApplyForce();
                if (velocity.Y >= 0)
                {
                    State = RocketState.Exploding;
                    Explode();
                }
                break;
            case RocketState.Exploding:
                foreach (var particle in particles)
                {
                    particle.ApplyForce();
                }

                if (particles.All(p => p.State == ExplosionParticleState.Done))
                {
                    State = RocketState.Done;
                }

                break;
            default:
                break;
        }
    }

    private void Explode()
    {
        for (int i = 0; i < 300; i++)
        {
            var particle = new ExplosionParticle(
                colorHue: this.colorHue + RandomFloat.NextFloat(-10f, 10f),
                startPosition: position,
                launchVelociy: RandomFloat.NextVector() * RandomFloat.NextFloat(0, spread));
            particles.Add(particle);
        }
    }
}

enum ExplosionParticleState
{
    Flying,
    Done
}

class ExplosionParticle(float colorHue, Vector2 startPosition, Vector2 launchVelociy) : Particle(colorHue, startPosition, launchVelociy, 2.5f)
{
    private float lifespan = 100f;
    private readonly float lifespanDecay = (float)Random.Shared.NextDouble() * 6.5f + 1.5f;
    public ExplosionParticleState State { get; private set; } = ExplosionParticleState.Flying;

    public override void ApplyForce()
    {
        if (State == ExplosionParticleState.Done) { return; }

        base.ApplyForce();

        velocity *= 0.9f;
        lifespan = Math.Max(0, lifespan - lifespanDecay);

        if (lifespan <= 0)
        {
            State = ExplosionParticleState.Done;
        }
    }
}