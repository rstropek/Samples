// ============================================================================
//  Two smaller C# 15 features:
//    (A) Extension INDEXERS — `this[...]` inside an `extension(...)` block.
//    (B) Labeled `break` / `continue` — target an outer loop (or switch) by name.
// ============================================================================
using IndexersAndLabels;

// ---------------------------------------------------------------------------
// (A) Extension indexers
// ---------------------------------------------------------------------------
Console.WriteLine("== (A) Extension indexers ==");

// 1. Index ANY sequence, including from the end — even a lazy iterator that has
//    no Count and no indexer of its own.
IEnumerable<int> squares = Squares(10);
Console.WriteLine($"squares[2] = {squares[2]}, squares[^1] = {squares[^1]}");

// 2. Domain-flavoured indexer on a type you don't own (DateOnly).
var today = new DateOnly(2026, 11, 15);   // a Sunday
Console.WriteLine($"{today:ddd yyyy-MM-dd}: Monday of that week = {today[DayOfWeek.Monday]:ddd yyyy-MM-dd}");

// 3. Indexer with a getter AND setter, keyed by a custom type.
List<char> board = [.. "..........".ToCharArray()];
var cell = new Pos(0, 3);
board[cell] = 'X';                        // extension setter
Console.WriteLine($"board = {new string([.. board])}, board[{cell}] = '{board[cell]}'");

// GOTCHA: element access on an ARRAY never falls back to an extension indexer.
//   char[] arr = ...; arr[cell]   -> error CS0029: Cannot implicitly convert type 'Pos' to 'int'
// Declare the extension on an interface (IReadOnlyList<T>, IList<T>) or a class instead.

// ---------------------------------------------------------------------------
// (B) Labeled break / continue
// ---------------------------------------------------------------------------
Console.WriteLine();
Console.WriteLine("== (B) Labeled break / continue ==");

IReadOnlyList<string> maze =
[
    "..#..",
    ".....",
    "#....",
    "...G.",
];

Console.WriteLine($"C# 15 labeled loops : {FindGoal(maze)}");
Console.WriteLine($"pre-C# 15 (flag)    : {FindGoalWithFlag(maze)}");

// `break label` also leaves a loop from INSIDE a switch — before C# 15 a plain
// `break` there only left the switch.
var commands = new Queue<string>(["step", "step", "skip", "step", "quit", "step"]);
int steps = 0;
loop: while (commands.TryDequeue(out var cmd))
{
    switch (cmd)
    {
        case "step": steps++; break;              // ordinary break: leaves the switch
        case "skip": continue loop;
        case "quit": break loop;                  // leaves the WHILE loop
    }
}
Console.WriteLine($"processed {steps} steps, {commands.Count} command(s) left after 'quit'");

// Scan row by row. Skip the rest of a row as soon as it contains a wall (`#`)
// to the left of the current column; stop everything at the goal.
static string FindGoal(IReadOnlyList<string> maze)
{
    int visited = 0;
    rows: for (int r = 0; r < maze.Count; r++)
    {
        for (int c = 0; c < maze[r].Length; c++)
        {
            var p = new Pos(r, c);
            if (maze[p] == '#') continue rows;    // next ROW, not next column
            visited++;
            if (maze[p] == 'G') return $"goal at {p} after {visited} cells";
        }
    }
    return $"no goal ({visited} cells)";
}

// The same algorithm before C# 15: a flag (or a `goto`). IDE0410 suggests
// replacing such patterns with labeled break/continue.
static string FindGoalWithFlag(IReadOnlyList<string> maze)
{
    int visited = 0;
    for (int r = 0; r < maze.Count; r++)
    {
        bool skipRow = false;
        for (int c = 0; c < maze[r].Length && !skipRow; c++)
        {
            var p = new Pos(r, c);
            if (maze[p] == '#') { skipRow = true; continue; }
            visited++;
            if (maze[p] == 'G') return $"goal at {p} after {visited} cells";
        }
    }
    return $"no goal ({visited} cells)";
}

static IEnumerable<int> Squares(int n)
{
    for (int i = 1; i <= n; i++) yield return i * i;
}

namespace IndexersAndLabels
{
    public readonly record struct Pos(int Row, int Col)
    {
        public override string ToString() => $"({Row},{Col})";
    }

    public static class Indexers
    {
        // Indexers are instance members, so the extension block needs a NAMED receiver.
        extension<T>(IEnumerable<T> source)
        {
            public T this[Index index] => index.IsFromEnd
                ? source.Reverse().ElementAt(index.Value - 1)   // fine for a demo; O(n)
                : source.ElementAt(index.Value);
        }

        extension(DateOnly date)
        {
            // ISO week: Monday is the first day.
            public DateOnly this[DayOfWeek day]
            {
                get
                {
                    DateOnly monday = date.AddDays(-(((int)date.DayOfWeek + 6) % 7));
                    return monday.AddDays(((int)day + 6) % 7);
                }
            }
        }

        // A grid stored row by row in a list of strings.
        extension(IReadOnlyList<string> rows)
        {
            public char this[Pos p] => rows[p.Row][p.Col];
        }

        // Get + set: a flat 10-column board.
        extension(List<char> board)
        {
            public char this[Pos p]
            {
                get => board[p.Row * 10 + p.Col];
                set => board[p.Row * 10 + p.Col] = value;
            }
        }
    }
}
