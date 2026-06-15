using UnionMembers;

Shape[] shapes = [new Circle(2), new Rectangle(3, 4), new Triangle(6, 8), Shape.Unit];

foreach (var s in shapes)
    Console.WriteLine($"{s,-26} area={s.Area:F2}  degenerate={s.IsDegenerate}");

Console.WriteLine();
Shape big = new Circle(2);
Console.WriteLine($"{big} scaled x3 -> {big.Scaled(3)}");   // custom ToString + method

Console.WriteLine();
Temperature t1 = new Fahrenheit(98.6);
Temperature t2 = new Celsius(100);
Console.WriteLine($"{new Fahrenheit(98.6)} body temp -> {t1}");  // partial-union ToString
Console.WriteLine($"boiling -> {t2}");

// The explicit form: ANY type with [Union] + IUnion works the same way.
// See ExplicitUnion.cs for a hand-written union struct with members.
ExplicitDemo.Run();
