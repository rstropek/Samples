# OthelloSharp - AI Coding Agent Guide

## Project Overview

OthelloSharp is a complete implementation of the classic Othello (Reversi) board game in C#/.NET. This project serves as a demonstration of coding with AI assistance, showcasing best practices in software architecture, test-driven development, and clean code principles.

Othello is a two-player strategy board game played on an 8×8 grid. Players place discs with their color (black or white) on the board. When a player places a disc, all opponent discs that are in a straight line and bounded by the newly placed disc and another disc of the current player's color are flipped to the current player's color. The game ends when neither player can make a valid move, and the winner is the player with the most discs of their color on the board.

## Solution Structure

```
OthelloSharp/
├── OthelloSharp.sln              # Solution file
├── Othello.GameLogic/            # Core game logic library
│   ├── Othello.GameLogic.csproj
│   └── ... (game logic classes)
├── Othello.Tests/                # Unit tests
│   ├── Othello.Tests.csproj
│   └── ... (test classes)
├── Othello.ConsoleApp/           # Console UI
│   ├── Othello.ConsoleApp.csproj
│   └── Program.cs
├── AGENTS.md                     # This file
└── storyboard.md                 # AI-assisted development guide
```

### Project Dependencies

- **Othello.GameLogic**: No external dependencies (pure logic)
- **Othello.Tests**: References `Othello.GameLogic`, uses xUnit
- **Othello.ConsoleApp**: References `Othello.GameLogic`

## Architecture Guidelines

### Separation of Concerns

1. **Game Logic Layer** (`Othello.GameLogic`)
   - Contains all game rules and state management
   - No dependencies on UI or external libraries
   - Pure business logic that can be reused across different UIs
   - Should be thoroughly tested

2. **Console UI Layer** (`Othello.ConsoleApp`)
   - Handles user input/output
   - Depends only on the game logic layer
   - Responsible for game visualization and user interaction

3. **Test Layer** (`Othello.Tests`)
   - Comprehensive unit tests for game logic
   - Test coverage for edge cases and game rules
   - Uses xUnit framework

### Recommended Class Structure for Game Logic

Consider implementing the following key classes:

- **Board**: Represents the 8×8 game board state
- **Player**: Enum or class representing Black/White players
- **Position**: Represents coordinates on the board (row, column)
- **Move**: Represents a move with position and which discs to flip
- **GameState**: Manages current game state, turn order, and game status
- **GameEngine**: Implements game rules (valid moves, disc flipping, win conditions)
- **MoveValidator**: Validates if a move is legal according to Othello rules

## Coding Guidelines

### General Principles

1. **Test-Driven Development (TDD)**
   - Write tests before implementing functionality
   - Aim for high test coverage (>80%)
   - Test edge cases and invalid inputs

2. **Clean Code**
   - Use meaningful variable and method names
   - Keep methods small and focused (single responsibility)
   - Add XML documentation comments for public APIs
   - Follow C# naming conventions (PascalCase for public members, camelCase for private)

3. **Immutability**
   - Prefer immutable data structures where appropriate
   - Use `readonly` fields and properties
   - Consider using records for simple data structures

4. **Error Handling**
   - Use exceptions for exceptional cases
   - Validate inputs and provide clear error messages
   - Consider using Result/Option types for operations that can fail

### Code Style

```csharp
// Good: Clear, descriptive names and XML comments
/// <summary>
/// Validates whether a move is legal according to Othello rules.
/// </summary>
/// <param name="board">The current board state.</param>
/// <param name="position">The position where the player wants to place a disc.</param>
/// <param name="player">The player making the move.</param>
/// <returns>True if the move is valid; otherwise, false.</returns>
public bool IsValidMove(Board board, Position position, Player player)
{
    if (board.IsOccupied(position))
        return false;
    
    return GetFlippableDiscs(board, position, player).Any();
}

// Prefer expression-bodied members for simple properties
public bool IsGameOver => !HasValidMoves(Player.Black) && !HasValidMoves(Player.White);

// Use pattern matching and modern C# features
public string GetWinner() => (BlackCount, WhiteCount) switch
{
    var (b, w) when b > w => "Black",
    var (b, w) when w > b => "White",
    _ => "Draw"
};
```

### Testing Guidelines

1. **Naming Convention**: `MethodName_Scenario_ExpectedBehavior`
   ```csharp
   [Fact]
   public void IsValidMove_EmptyPosition_ReturnsTrueIfFlipsOpponentDiscs()
   ```

2. **Arrange-Act-Assert Pattern**
   ```csharp
   [Fact]
   public void PlaceDisc_ValidMove_FlipsOpponentDiscs()
   {
       // Arrange
       var board = new Board();
       board.Initialize();
       var position = new Position(2, 3);
       
       // Act
       var result = board.PlaceDisc(position, Player.Black);
       
       // Assert
       Assert.True(result);
       Assert.Equal(Player.Black, board.GetDisc(position));
   }
   ```

3. **Test Categories**
   - Unit tests for individual methods
   - Integration tests for game flows
   - Edge case tests (full board, no valid moves, etc.)

### Git Commit Guidelines

- Use conventional commits: `feat:`, `fix:`, `test:`, `refactor:`, `docs:`
- Keep commits atomic and focused
- Write descriptive commit messages

Example:
```
feat: implement move validation logic
test: add comprehensive tests for edge cases
refactor: extract board initialization into separate method
```

## Development Workflow

1. **Setup**: Restore NuGet packages (`dotnet restore`)
2. **Build**: `dotnet build`
3. **Test**: `dotnet test`
4. **Run**: `dotnet run --project Othello.ConsoleApp`

## Target Framework

This project uses .NET 10 RC2. Ensure you have the appropriate SDK installed before building.

## Additional Resources

- [Othello/Reversi Rules](https://en.wikipedia.org/wiki/Reversi)
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [xUnit Documentation](https://xunit.net/)
- [Clean Code Principles](https://www.amazon.com/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882)

