// ============================================================================
//  Runtime Async demo — a 3-level async call chain that prints the LIVE stack
//  trace from the innermost method. Using `Task.CompletedTask` (synchronous
//  completion) keeps the whole call chain on the stack so the frame count is
//  directly comparable between the classic and runtime-async builds.
//
//  This file is IDENTICAL in both 08_RuntimeAsync_Classic and
//  09_RuntimeAsync_Enabled. The ONLY difference is the .csproj:
//      <Features>$(Features);runtime-async=on</Features>
// ============================================================================
using System.Diagnostics;

Console.WriteLine($"Runtime-async enabled: {IsRuntimeAsyncOn()}");
Console.WriteLine(new string('-', 60));
await OuterAsync();

static async Task OuterAsync()
{
    await Task.CompletedTask;
    await MiddleAsync();
}

static async Task MiddleAsync()
{
    await Task.CompletedTask;
    await InnerAsync();
}

static async Task InnerAsync()
{
    await Task.CompletedTask;
    var trace = new StackTrace(fNeedFileInfo: false);
    Console.WriteLine(trace);
    Console.WriteLine(new string('-', 60));
    Console.WriteLine($"Visible frames: {trace.FrameCount}");
}

// Heuristic: classic async emits a nested IAsyncStateMachine type for InnerAsync;
// runtime-async does not. We detect it by reflecting over the declaring type.
static bool IsRuntimeAsyncOn()
{
    var nested = typeof(Program).Assembly
        .GetTypes()
        .Any(t => typeof(System.Runtime.CompilerServices.IAsyncStateMachine).IsAssignableFrom(t));
    return !nested;
}

// Breadth check: common async patterns all run under runtime-async.
await AsyncPatterns.RunAllAsync();
