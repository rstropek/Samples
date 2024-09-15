using Ref;
/*
Parameters.RefParameters();
ReturnValues.RefReturns();
VariablesAndExpressions.RefVariables();
VariablesAndExpressions.RefExpressions();

Span.TheProblem();
Span.SpanBasics();
Span.BadIdeas();
Span.MemoryBasics();
Span.UnmanagedBasics();
Span.SpanVsList();

RefStructs.RefStruct();
*/
Span<int> values = stackalloc int[] { 1, 1, 2, 2, 3, 3 };
var window = new SlidingWindow(values, 2);
Console.WriteLine(window.Average());
while (window.Slide()) { Console.WriteLine(window.Average()); }

window.Values = stackalloc int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
Console.WriteLine(window.Average());
while (window.Slide()) { Console.WriteLine(window.Average()); }

var roWindow = new ReadonlySlidingWindow(values, 2);
Console.WriteLine(roWindow.Average());
while (roWindow.TrySlide(out roWindow)) { Console.WriteLine(roWindow.Average()); }
