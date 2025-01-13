using System.Collections.ObjectModel;
using System.Numerics;

namespace Snake.Game;

public readonly record struct Position(int X, int Y)
    : IAdditionOperators<Position, Direction, Position>
{
    public static Position operator +(Position a, Direction b)
        => new(a.X + b.X, a.Y + b.Y);
}

public readonly record struct Direction(int X, int Y)
{
    public static readonly Direction Left = new(-1, 0);
    public static readonly Direction Right = new(1, 0);
    public static readonly Direction Up = new(0, -1);
    public static readonly Direction Down = new(0, 1);

    public static readonly Direction[] Directions = [
        Left,
        Right,
        Up,
        Down
    ];

    public static Direction GetRandom() => Random.Shared.GetItems(Directions, 1)[0];
}

public class Game
{
    public Position Apple { get; private set; }
    private List<Position> SnakeBodyParts { get; set; }
    public ReadOnlyCollection<Position> SnakeBody => new(SnakeBodyParts);
    public Direction SnakeDirection { get; private set; }
    public short Width { get; }
    public short Height { get; }
    private int DesiredSnakeLength { get; set; }

    public Game(short width, short height, short snakeLength)
    {
        Width = width;
        Height = height;
        DesiredSnakeLength = snakeLength;

        SnakeBodyParts = new(snakeLength)
        {
            new(Random.Shared.Next(width), Random.Shared.Next(height))
        };

        while (SnakeBodyParts.Count < snakeLength)
        {
            var direction = Direction.GetRandom();
            var newPos = SnakeBodyParts[^1] + direction;
            if (IsValidPosition(newPos))
            {
                SnakeBodyParts.Add(newPos);
            }
        }

        PlaceNewApple();

        // Choose a direction that doesn't lead to immediate loss
        var head = SnakeBodyParts[0];
        var validDirections = Direction.Directions.Where(dir => IsValidPosition(head + dir)).ToArray();

        SnakeDirection = validDirections.Length > 0
            ? Random.Shared.GetItems(validDirections, 1)[0]
            : Direction.GetRandom(); // Fallback in case no valid direction (shouldn't happen with normal game dimensions)
    }

    public event EventHandler? OnFoundApple;
    public event EventHandler? OnDied;

    private bool IsValidPosition(Position pos) =>
        pos.X >= 0 && pos.X < Width &&
        pos.Y >= 0 && pos.Y < Height &&
        !SnakeBodyParts.Contains(pos);

    public void Move()
    {
        var head = SnakeBodyParts[0];
        var nextPos = head + SnakeDirection;
        if (IsValidPosition(nextPos))
        {
            SnakeBodyParts.Insert(0, nextPos);
            if (SnakeBodyParts.Count > DesiredSnakeLength)
            {
                SnakeBodyParts.RemoveAt(SnakeBodyParts.Count - 1);
            }

            if (nextPos == Apple)
            {
                OnFoundApple?.Invoke(this, EventArgs.Empty);
                DesiredSnakeLength++;
                PlaceNewApple();
            }
        }
        else
        {
            OnDied?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool TrySetSnakeDirection(Direction direction)
    {
        // Ignore if new direction is opposite to current direction
        if (direction.X != -SnakeDirection.X || direction.Y != -SnakeDirection.Y)
        {
            SnakeDirection = direction;
            return true;
        }

        return false;
    }

    private void PlaceNewApple()
    {
        do
        {
            Apple = new(Random.Shared.Next(Width), Random.Shared.Next(Height));
        }
        while (SnakeBodyParts.Contains(Apple));
    }
}