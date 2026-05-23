# OthelloSharp

A demonstration project for AI-assisted development: Building the classic Othello (Reversi) game in C#/.NET.

## 🎮 Project Status

**Setup Complete!** The project structure is ready for development with AI assistance.

## 📁 Project Structure

```
OthelloSharp/
├── OthelloSharp.sln              # Solution file
├── Othello.GameLogic/            # Core game logic library (no dependencies)
│   └── Othello.GameLogic.csproj
├── Othello.Tests/                # xUnit test project
│   └── Othello.Tests.csproj      # References Othello.GameLogic
├── Othello.ConsoleApp/           # Console UI application
│   ├── Othello.ConsoleApp.csproj # References Othello.GameLogic
│   └── Program.cs
├── AGENTS.md                     # AI coding agent guidelines
├── storyboard.md                 # Step-by-step development guide
└── README.md                     # This file
```

## 🚀 Getting Started

### Prerequisites

- **.NET 10 RC2 SDK** (already installed on your system)
- Code editor with AI assistant (Cursor, GitHub Copilot, etc.)

### Next Steps

1. **Read the Documentation:**
   - 📖 **[AGENTS.md](AGENTS.md)** - Project overview, architecture, and coding guidelines
   - 📋 **[storyboard.md](storyboard.md)** - Detailed step-by-step guide with AI prompts

2. **Start Development:**
   - Follow the storyboard.md guide to build the game with AI assistance
   - Each phase includes suggested prompts and validation steps
   - Start with Phase 1: Core Data Structures

3. **Build and Test:**
   ```bash
   # Restore dependencies (if not already done)
   dotnet restore
   
   # Build the solution
   dotnet build
   
   # Run tests
   dotnet test
   
   # Run the console app
   dotnet run --project Othello.ConsoleApp
   ```

## 🎯 What You'll Build

An implementation of Othello (Reversi) with:

- ✅ Complete game rules and logic
- ✅ Move validation in all 8 directions
- ✅ Disc flipping mechanics
- ✅ Game state management
- ✅ Win condition detection
- ✅ Console-based user interface
- ✅ Comprehensive unit tests (>80% coverage)

## 📚 Development Phases

The storyboard breaks development into 6 phases:

1. **Phase 1:** Core data structures (Player, Position, Board)
2. **Phase 2:** Game rules (move validation, disc flipping)
3. **Phase 3:** Game state management
4. **Phase 4:** Console user interface
5. **Phase 5:** Enhancements (statistics, hints, AI opponent)
6. **Phase 6:** Testing and documentation

**Estimated time:** 5-8 hours with AI assistance

## 🧪 Testing

The project follows Test-Driven Development (TDD):

- Write tests before implementation
- Aim for >80% code coverage
- Test edge cases and game rules thoroughly
- Use xUnit framework

## 🏗️ Architecture

- **Separation of Concerns:** Game logic is independent of UI
- **Clean Code:** Meaningful names, single responsibility, XML documentation
- **Immutability:** Prefer immutable data structures where appropriate
- **Modern C#:** Uses .NET 10, nullable reference types, pattern matching

## 📖 About Othello

Othello (also known as Reversi) is a strategy board game for two players:

- Played on an 8×8 grid
- Players place discs (Black and White)
- Placing a disc flips opponent's discs that are sandwiched
- Game ends when neither player can move
- Winner has the most discs on the board

[Learn more about Othello rules](https://en.wikipedia.org/wiki/Reversi)

## 🤖 AI-Assisted Development

This project is specifically designed to demonstrate effective AI-assisted coding:

- **Incremental development** with validation at each step
- **Test-driven approach** ensures correctness
- **Clear prompts** for AI assistants
- **Iterative refinement** of generated code

See [AGENTS.md](AGENTS.md) for detailed AI assistant guidelines.

## 🔧 Troubleshooting

### NuGet Restore Issues

If you encounter SSL/certificate errors during `dotnet restore`:

```bash
# Option 1: Restore with network permissions
dotnet restore

# Option 2: If offline, packages may already be cached
dotnet build --no-restore
```

The test project requires these NuGet packages:
- `Microsoft.NET.Test.Sdk`
- `xunit`
- `xunit.runner.visualstudio`
- `coverlet.collector`

### Build Errors

- Ensure .NET 10 RC2 SDK is installed: `dotnet --version`
- Clean and rebuild: `dotnet clean && dotnet build`

## 📈 Future Enhancements

After completing the basic implementation, consider:

- 🎨 **GUI version** (WPF, Avalonia, or Blazor)
- 🌐 **Multiplayer support** (network play)
- 🤖 **Advanced AI** (minimax algorithm)
- 📱 **Mobile app** (.NET MAUI)
- 🏆 **Tournament mode** with statistics

## 📄 License

This is a sample/demonstration project for learning AI-assisted development.

## 🙏 Acknowledgments

Built to demonstrate best practices in:
- Test-Driven Development
- Clean Code principles
- AI-assisted software development
- .NET application architecture

---

**Ready to start?** Open [storyboard.md](storyboard.md) and begin with Phase 1! 🚀

