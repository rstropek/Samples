# OthelloSharp Development Storyboard

This document provides a step-by-step guide for building the OthelloSharp game using AI-assisted development. Each step includes suggested prompts, expected outcomes, and validation steps.

## Prerequisites

- .NET 10 RC2 SDK installed
- Code editor with AI assistant capabilities (e.g., Cursor, GitHub Copilot, etc.)
- Basic understanding of C# and Othello game rules

## Phase 1: Foundation - Core Data Structures

### Step 1.1: Create Player Enum and Position Record

**Suggested Prompt:**
```
Create a Player enum in the Othello.GameLogic project representing Black and White players.
Also create a Position record with Row and Column properties to represent board coordinates (0-7).
Add appropriate validation to Position.
```

**Expected Outcome:**
- `Player.cs` with Black and White values
- `Position.cs` as a record with Row and Column properties
- Validation ensuring Row and Column are between 0 and 7

**Validation:**
```bash
dotnet build
# Should compile without errors
```

**Test Prompt:**
```
Write xUnit tests for the Position record in Othello.Tests project:
- Test that valid positions are created correctly
- Test that invalid positions (row/column outside 0-7) throw ArgumentOutOfRangeException
- Test Position equality
```

---

### Step 1.2: Create Board Class

**Suggested Prompt:**
```
Create a Board class in Othello.GameLogic that:
- Uses an 8x8 array to represent the game board
- Has a GetDisc method to retrieve the player at a position (or null if empty)
- Has a SetDisc method to place a disc at a position
- Has an IsOccupied method to check if a position has a disc
- Has an Initialize method that sets up the standard Othello starting position:
  - Position (3,3): White, Position (3,4): Black
  - Position (4,3): Black, Position (4,4): White
```

**Expected Outcome:**
- `Board.cs` with appropriate data structure and methods
- Proper encapsulation of the board array
- Initialization to standard Othello starting position

**Test Prompt:**
```
Write comprehensive xUnit tests for the Board class:
- Test board initialization with correct starting positions
- Test GetDisc returns correct values
- Test SetDisc places discs correctly
- Test IsOccupied returns true/false appropriately
- Test accessing invalid positions throws appropriate exceptions
```

**Validation:**
```bash
dotnet test
# All tests should pass
```

---

## Phase 2: Game Rules - Move Validation and Disc Flipping

### Step 2.1: Implement Direction Logic

**Suggested Prompt:**
```
Create a Direction class or enum representing the 8 directions on the board:
North, South, East, West, NorthEast, NorthWest, SouthEast, SouthWest.
Add a method to get the next position in a given direction from a starting position.
```

**Expected Outcome:**
- `Direction.cs` with 8 direction constants/values
- Method to calculate next position in a direction
- Proper boundary checking

**Test Prompt:**
```
Write tests for Direction functionality:
- Test getting next position in each of the 8 directions
- Test that moving outside board boundaries is handled correctly
```

---

### Step 2.2: Implement Move Validation

**Suggested Prompt:**
```
Create a MoveValidator class in Othello.GameLogic that validates moves according to Othello rules.
Implement a method IsValidMove(Board board, Position position, Player player) that:
- Returns false if the position is occupied
- Returns false if the position is outside the board
- Returns true only if placing a disc there would flip at least one opponent disc
- Checks all 8 directions from the position
- A valid flip requires: opponent disc(s) in a direction, followed by a player's own disc

Also implement GetFlippableDiscs(Board board, Position position, Player player) that returns
all positions that would be flipped if a disc is placed at the given position.
```

**Expected Outcome:**
- `MoveValidator.cs` with move validation logic
- Methods that check all 8 directions for valid flips
- Clear logic following Othello rules

**Test Prompt:**
```
Write extensive tests for MoveValidator:
- Test that occupied positions are invalid
- Test that empty positions with no flips are invalid
- Test valid moves in each of the 8 directions
- Test moves that flip multiple discs
- Test moves that flip discs in multiple directions
- Test edge cases (board edges, corners)
- Use the initial board configuration and various mid-game scenarios
```

**Validation:**
```bash
dotnet test --filter "MoveValidator"
# All move validation tests should pass
```

---

### Step 2.3: Implement Move Execution

**Suggested Prompt:**
```
Add a PlaceDisc method to the Board class that:
- Places a disc at the specified position
- Flips all appropriate opponent discs
- Uses the MoveValidator to get the list of discs to flip
- Returns true if the move was successful, false otherwise
```

**Expected Outcome:**
- Updated `Board.cs` with PlaceDisc method
- Integration with MoveValidator
- Proper disc flipping logic

**Test Prompt:**
```
Write integration tests for PlaceDisc:
- Test that valid moves place the disc and flip opponents
- Test that invalid moves don't modify the board
- Test multiple consecutive moves
- Test disc flipping in multiple directions simultaneously
```

---

## Phase 3: Game State Management

### Step 3.1: Create GameState Class

**Suggested Prompt:**
```
Create a GameState class in Othello.GameLogic that:
- Contains a Board instance
- Tracks the current player (whose turn it is)
- Has a property IsGameOver that checks if neither player has valid moves
- Has a GetValidMoves(Player player) method that returns all valid positions for a player
- Has a GetDiscCount(Player player) method that counts discs for each player
- Has a MakeMove(Position position) method that executes a move for the current player
- Switches to the next player after a move (or stays with same player if opponent has no valid moves)
```

**Expected Outcome:**
- `GameState.cs` managing game flow
- Turn management logic
- Game over detection
- Move execution with turn switching

**Test Prompt:**
```
Write tests for GameState:
- Test initial game state (Black goes first)
- Test getting valid moves for a player
- Test making valid moves
- Test turn switching after moves
- Test that player keeps turn if opponent has no valid moves
- Test game over detection
- Test disc counting
```

**Validation:**
```bash
dotnet test --filter "GameState"
```

---

### Step 3.2: Implement Winner Determination

**Suggested Prompt:**
```
Add winner determination logic to GameState:
- Add a GetWinner() method that returns the player with the most discs (or null for a draw)
- This should only be called when IsGameOver is true
- Add a GameResult property that returns "Black Wins", "White Wins", or "Draw"
```

**Expected Outcome:**
- Winner determination methods in GameState
- Proper handling of draws

**Test Prompt:**
```
Write tests for winner determination:
- Test scenarios where Black wins
- Test scenarios where White wins
- Test draw scenarios
- Test that GetWinner throws or returns null when game is not over
```

---

## Phase 4: Console User Interface

### Step 4.1: Create Board Display

**Suggested Prompt:**
```
In the Othello.ConsoleApp project, create a BoardRenderer class that:
- Takes a Board instance and renders it to the console
- Uses different characters/colors for Black discs (B or ●), White discs (W or ○), and empty spaces (.)
- Shows row and column numbers for easy reference
- Makes the board visually appealing and easy to read
```

**Expected Outcome:**
- `BoardRenderer.cs` with console rendering logic
- Clear visual representation of the board
- Color coding if supported by terminal

**Example Output:**
```
  0 1 2 3 4 5 6 7
0 . . . . . . . .
1 . . . . . . . .
2 . . . . . . . .
3 . . . ○ ● . . .
4 . . . ● ○ . . .
5 . . . . . . . .
6 . . . . . . . .
7 . . . . . . . .
```

---

### Step 4.2: Implement User Input Handling

**Suggested Prompt:**
```
Create an InputHandler class in Othello.ConsoleApp that:
- Prompts the user for a move (row and column)
- Validates that the input is numeric and in range
- Returns a Position object
- Handles invalid input gracefully with error messages
- Allows the user to type 'quit' to exit
```

**Expected Outcome:**
- `InputHandler.cs` with robust input parsing
- Error handling and user-friendly messages
- Exit functionality

---

### Step 4.3: Create Main Game Loop

**Suggested Prompt:**
```
Update Program.cs in Othello.ConsoleApp to create a complete game loop:
1. Initialize a new GameState
2. Display welcome message and game rules
3. Main game loop:
   - Display the current board
   - Show whose turn it is
   - Show valid moves for the current player
   - If current player has no valid moves, display message and pass turn
   - Prompt for move input
   - Validate and execute the move
   - Display what discs were flipped
   - Check if game is over
4. When game ends:
   - Display final board
   - Show final scores
   - Announce the winner
5. Ask if players want to play again
```

**Expected Outcome:**
- Complete, playable console application
- Clear user experience with helpful messages
- Smooth game flow from start to finish

**Test Run:**
```bash
dotnet run --project Othello.ConsoleApp
# Play a complete game to verify functionality
```

---

## Phase 5: Enhancements and Polish

### Step 5.1: Add Game Statistics

**Suggested Prompt:**
```
Add a GameStatistics class that tracks:
- Number of moves made by each player
- Total game duration
- Move history (all moves made during the game)
- Add a method to export game history
```

**Optional Enhancement:**
```
Add the ability to save and load games to/from JSON files.
```

---

### Step 5.2: Implement Move Hints

**Suggested Prompt:**
```
Add a 'hint' feature in the console app that:
- Shows the number of discs that would be flipped for each valid move
- Highlights the best move (most discs flipped)
- Can be toggled on/off by the user
```

---

### Step 5.3: Add AI Opponent (Optional)

**Suggested Prompt:**
```
Create a simple AI player in Othello.GameLogic:
- Create an IPlayer interface with a GetMove method
- Create a HumanPlayer class (uses console input)
- Create a SimpleAI class that uses a basic strategy:
  - Option 1: Random valid move
  - Option 2: Greedy (choose move that flips most discs)
  - Option 3: Positional strategy (prefer corners and edges)
- Update the console app to allow human vs AI games
```

---

## Phase 6: Final Testing and Documentation

### Step 6.1: Comprehensive Testing

**Suggested Prompt:**
```
Review test coverage and add any missing tests:
- Test all edge cases (full board, corners, edges)
- Test complete game scenarios from start to finish
- Test pass scenarios (player has no valid moves)
- Add integration tests that play through sample games
```

**Validation:**
```bash
dotnet test --collect:"XPlat Code Coverage"
# Review coverage report
```

---

### Step 6.2: Code Documentation

**Suggested Prompt:**
```
Add XML documentation comments to all public classes and methods in Othello.GameLogic.
Follow C# documentation standards with <summary>, <param>, <returns>, and <exception> tags.
```

---

### Step 6.3: Create README

**Suggested Prompt:**
```
Create a README.md file with:
- Project description
- How to build and run the application
- How to run tests
- Game rules explanation
- Screenshots or examples of gameplay
- Technical architecture overview
- Future enhancement ideas
```

---

## Tips for Working with AI Assistants

### Effective Prompting Strategies

1. **Be Specific**: Include exact class names, namespaces, and expected behavior
2. **Request Tests First**: Ask for tests before implementation (TDD approach)
3. **Iterate**: If the output isn't perfect, ask for specific refinements
4. **Ask for Explanations**: Request comments or explanations for complex logic
5. **Validate Each Step**: Build and test after each major step before moving forward

### Example Refinement Prompts

- "The move validation logic doesn't handle corners correctly. Can you fix the edge case when checking northwest from position (0,0)?"
- "Add more descriptive error messages to the input validation"
- "Refactor the GetFlippableDiscs method to be more readable"
- "Add XML documentation comments to the Board class"

### Debugging with AI

When you encounter issues:
```
I'm getting a NullReferenceException in GameState.MakeMove when I try to execute a move.
Here's the stack trace: [paste stack trace]
Here's the relevant code: [paste code]
Can you help me identify and fix the issue?
```

---

## Estimated Timeline

- **Phase 1**: 30-60 minutes (Core data structures)
- **Phase 2**: 1-2 hours (Game rules and validation)
- **Phase 3**: 1 hour (Game state management)
- **Phase 4**: 1-2 hours (Console UI)
- **Phase 5**: 1-2 hours (Enhancements - optional)
- **Phase 6**: 30-60 minutes (Testing and documentation)

**Total**: 5-8 hours for complete implementation with AI assistance

---

## Success Criteria

✅ All unit tests pass  
✅ Game can be played from start to finish  
✅ All Othello rules are correctly implemented  
✅ User-friendly console interface  
✅ Clean, well-documented code  
✅ High test coverage (>80%)  

---

## Common Pitfalls and Solutions

### Pitfall 1: Invalid Move Detection
**Problem**: Not all 8 directions are checked properly, especially at board edges.  
**Solution**: Write extensive tests for edge and corner positions in all directions.

### Pitfall 2: Turn Switching
**Problem**: Forgetting to handle the case where one player has no valid moves but the game isn't over.  
**Solution**: Check both players for valid moves before declaring game over.

### Pitfall 3: Disc Flipping
**Problem**: Only flipping discs in one direction instead of all valid directions.  
**Solution**: Ensure GetFlippableDiscs checks all 8 directions and returns all positions.

### Pitfall 4: Off-by-One Errors
**Problem**: Array indexing errors when checking board boundaries.  
**Solution**: Use consistent 0-based indexing and test boundary conditions thoroughly.

---

## Next Steps After Completion

1. **Performance Optimization**: Profile and optimize critical game logic
2. **GUI Version**: Create a WPF, Avalonia, or Blazor UI
3. **Multiplayer**: Add network play capabilities
4. **Tournament Mode**: Multiple games with score tracking
5. **Advanced AI**: Implement minimax with alpha-beta pruning
6. **Mobile App**: Port to .NET MAUI for mobile platforms

---

## Resources

- [Othello Strategy Guide](https://www.worldothello.org/)
- [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [xUnit Best Practices](https://xunit.net/docs/getting-started)
- [Test-Driven Development](https://martinfowler.com/bliki/TestDrivenDevelopment.html)

Happy coding with AI! 🎮

