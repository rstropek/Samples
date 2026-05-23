using SkiaSharp;

namespace Shooter;

public class Game
{
    private const float SPACESHIP_HEIGHT = 50;
    private const float SPACESHIP_WIDTH = 40;
    private const float SPACESHIP_SPEED = 5;
    private SKPoint? SpaceshipPosition = null;

    private const float LASER_LENGTH = 10;
    private const float LASER_SPEED = 10;
    private List<SKPoint> Lasers = [];

    private const float METEOR_WIDTH = 40;
    private const float METEOR_SPEED = 2;
    private const double METEORS_TIME_INTERVAL = 0.5;
    private List<SKPoint> Meteors = [];
    private DateTime lastMeteorCreationTime = DateTime.Now.AddSeconds(-10);

    public bool Paint(SKCanvas canvas, SKImageInfo info, KeyboardStatus keyboard)
    {
        // Initialize the spaceship position if it has never been set before.
        // Spaceship should be centered in x axis, and at 80% of the height in y axis.
        if (SpaceshipPosition == null) { SpaceshipPosition = new(info.Width / 2, info.Height * 0.8f); }

        // Black background
        canvas.Clear(SKColors.Black);

        // A spaceship is a triangle with a base of SPACESHIP_WIDTH and a height of SPACESHIP_HEIGHT.
        // The SpaceshipPosition is the center of the base of the triangle.
        var spaceshipVertices = new SKPoint[]
        {
            new(SpaceshipPosition.Value.X, SpaceshipPosition.Value.Y - SPACESHIP_HEIGHT),
            new(SpaceshipPosition.Value.X - SPACESHIP_WIDTH / 2, SpaceshipPosition.Value.Y),
            new(SpaceshipPosition.Value.X + SPACESHIP_WIDTH / 2, SpaceshipPosition.Value.Y)
        };
        MoveSpaceship(info, keyboard, ref SpaceshipPosition);
        DrawSpaceship(canvas, spaceshipVertices);

        if (keyboard.Shooting)
        {
            Lasers.Add(new(SpaceshipPosition!.Value.X, SpaceshipPosition.Value.Y - SPACESHIP_HEIGHT));
            keyboard.Shooting = false;
        }

        Move(Lasers, LASER_SPEED);
        Move(Meteors, -METEOR_SPEED);

        if (Meteors.Count < 10 && (DateTime.Now - lastMeteorCreationTime).TotalSeconds >= METEORS_TIME_INTERVAL)
        {
            Meteors.Add(new(Random.Shared.Next(0, info.Width), 0));
            lastMeteorCreationTime = DateTime.Now;
        }

        if (!HandleCollisions(info, spaceshipVertices))
        {
            // Game over
            return false;
        }

        DrawLasers(canvas, Lasers);
        DrawMeteors(canvas, Meteors);

        return true;
    }

    private static void DrawMeteors(SKCanvas canvas, IEnumerable<SKPoint> meteors)
    {
        foreach (var meteor in meteors)
        {
            DrawGlowingCircle(canvas, meteor, METEOR_WIDTH / 2, SKColors.Orange);
        }
    }

    private bool HandleCollisions(SKImageInfo info, SKPoint[] spaceshipVertices)
    {
        var hitLasers = new HashSet<SKPoint>();
        var hitMeteors = new HashSet<SKPoint>();
        foreach (var meteor in Meteors)
        {
            if (IsColliding(meteor, METEOR_WIDTH / 2, spaceshipVertices)) { return false; }

            foreach (var laser in Lasers)
            {
                laser.Offset(0, -LASER_LENGTH);
                if (IsColliding(meteor, METEOR_WIDTH / 2, laser))
                {
                    hitLasers.Add(laser);
                    hitMeteors.Add(meteor);
                }
            }

            if (meteor.Y - METEOR_WIDTH / 2 > info.Height)
            {
                hitMeteors.Add(meteor);
            }
        }

        Meteors = Meteors.Except(hitMeteors).ToList();
        Lasers = Lasers.Except(hitLasers).ToList();

        return true;
    }

    private static void DrawLasers(SKCanvas canvas, IEnumerable<SKPoint> lasers)
    {
        foreach (var laser in lasers)
        {
            using var laserPath = new SKPath();
            laserPath.MoveTo(laser.X, laser.Y);
            laserPath.LineTo(laser.X, laser.Y - LASER_LENGTH);
            DrawGlowingPath(canvas, laserPath, SKColors.Red);
        }
    }

    private static void Move(List<SKPoint> objects, float speed)
    {
        for (var i = 0; i < objects.Count; i++)
        {
            var obj = objects[i];
            obj.Offset(0, -speed);
            if (obj.Y < 0)
            {
                objects.RemoveAt(i--);
                continue;
            }

            objects[i] = obj;
        }
    }

    private static void DrawSpaceship(SKCanvas canvas, SKPoint[] vertices)
    {
        using var spaceshipPath = new SKPath();
        spaceshipPath.MoveTo(vertices[0]);
        spaceshipPath.LineTo(vertices[1]);
        spaceshipPath.LineTo(vertices[2]);
        spaceshipPath.Close();

        DrawGlowingPath(canvas, spaceshipPath, SKColors.Yellow);
    }

    private static void DrawGlowingPath(SKCanvas canvas, SKPath path, SKColor baseColor)
    {
        var glowColor = new SKColor(baseColor.Red, baseColor.Green, baseColor.Blue, 50);
        for (int i = 5; i > 0; i--)
        {
            using var glowPaint = new SKPaint
            {
                Color = glowColor.WithAlpha((byte)(glowColor.Alpha * i / 5.0)),
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = i * 2
            };

            canvas.DrawPath(path, glowPaint);
        }

        using var pathPaint = new SKPaint
        {
            Color = baseColor,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2
        };
        canvas.DrawPath(path, pathPaint);
    }

    private static void DrawGlowingCircle(SKCanvas canvas, SKPoint center, float radius, SKColor baseColor)
    {
        var glowColor = new SKColor(baseColor.Red, baseColor.Green, baseColor.Blue, 50);
        for (int i = 5; i > 0; i--)
        {
            using var glowPaint = new SKPaint
            {
                Color = glowColor.WithAlpha((byte)(glowColor.Alpha * i / 5.0)),
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = i * 2
            };

            canvas.DrawCircle(center, radius, glowPaint);
        }

        using var circlePaint = new SKPaint
        {
            Color = baseColor,
            IsAntialias = true,
        };
        canvas.DrawCircle(center, radius, circlePaint);
    }

    private static void MoveSpaceship(SKImageInfo info, KeyboardStatus keyboard, ref SKPoint? SpaceshipPosition)
    {
        if (SpaceshipPosition is null) { throw new InvalidOperationException("SpaceshipPosition is null"); }

        if (keyboard.LeftIsDown && SpaceshipPosition.Value.X > SPACESHIP_WIDTH / 2)
        {
            SpaceshipPosition = new(
                Math.Max(SPACESHIP_WIDTH / 2, SpaceshipPosition.Value.X - SPACESHIP_SPEED),
                SpaceshipPosition.Value.Y);
        }

        if (keyboard.RightIsDown && SpaceshipPosition.Value.X < info.Width - SPACESHIP_WIDTH / 2)
        {
            SpaceshipPosition = new(
                Math.Min(info.Width - SPACESHIP_WIDTH / 2, SpaceshipPosition.Value.X + SPACESHIP_SPEED),
                SpaceshipPosition.Value.Y);
        }

        if (keyboard.UpIsDown && SpaceshipPosition.Value.Y > SPACESHIP_HEIGHT)
        {
            SpaceshipPosition = new(
                SpaceshipPosition.Value.X,
                Math.Max(SPACESHIP_HEIGHT, SpaceshipPosition.Value.Y - SPACESHIP_SPEED));
        }

        if (keyboard.DownIsDown && SpaceshipPosition.Value.Y < info.Height)
        {
            SpaceshipPosition = new(
                SpaceshipPosition.Value.X,
                Math.Min(info.Height, SpaceshipPosition.Value.Y + SPACESHIP_SPEED));
        }
    }

    private static bool IsColliding(SKPoint circleCenter, float circleRadius, SKPoint[] triangleVertices)
    {
        foreach (var vertex in triangleVertices)
        {
            var distance = Math.Sqrt(Math.Pow(vertex.X - circleCenter.X, 2) + Math.Pow(vertex.Y - circleCenter.Y, 2));
            if (distance <= circleRadius)
            {
                return true;
            }
        }
        return false;
    }

    private static bool IsColliding(SKPoint circleCenter, float circleRadius, SKPoint laserTopPoint)
    {
        // Calculate the distance between the topmost point of the laser and the center of the circle
        float distance = (float)Math.Sqrt(Math.Pow(laserTopPoint.X - circleCenter.X, 2) + Math.Pow(laserTopPoint.Y - circleCenter.Y, 2));

        // If the distance is less than or equal to the circle's radius, they are colliding
        return distance <= circleRadius;
    }
}
