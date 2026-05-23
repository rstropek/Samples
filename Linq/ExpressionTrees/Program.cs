using System.Linq.Expressions;
using Data;
using Microsoft.EntityFrameworkCore;

// Simple
{
    Func<int, int> mulFunc = x => x * 2;
    Console.WriteLine($"Func: mulFunc(5)  = {mulFunc(5)}"); // 10

    Expression<Func<int, int>> expr = x => x * 2;
    var mulShort = expr.Compile();
    Console.WriteLine($"Short form: mulShort(5) = {mulShort(5)}");

    var x = Expression.Parameter(typeof(int), name: "x");
    var two = Expression.Constant(2, typeof(int));
    var multiply = Expression.Multiply(x, two);
    var lambda = Expression.Lambda<Func<int, int>>(multiply, x);
    var mulLong = lambda.Compile();
    Console.WriteLine($"Long form: mulLong(5)  = {mulLong(5)}"); // 10
}

// More advanced
{
    Expression<Func<int>> expr = () => 2 + 3 * 15;
    var func = expr.Compile();
    Console.WriteLine($"Expression: {expr} = {func()}");

    var c2 = Expression.Constant(2, typeof(int));
    var c3 = Expression.Constant(3, typeof(int));
    var c15 = Expression.Constant(15, typeof(int));
    var mul = Expression.Multiply(c3, c15);
    var add = Expression.Add(c2, mul);
    var lambda = Expression.Lambda<Func<int>>(add);
    var exprFunc = lambda.Compile();
    Console.WriteLine($"Manual: {lambda} = {exprFunc()}");
}

// With Entity Framework
{
    var dataContextFactory = new ApplicationDataContextFactory();
    using var context = dataContextFactory.CreateDbContext(args);

    var filterName = "Alice";

    Expression<Func<Person, bool>> filter = p => p.Name.Contains(filterName);
    var result = await context.People.Where(filter).ToArrayAsync();

    var p = Expression.Parameter(typeof(Person), "p");
    var member = Expression.Property(p, nameof(Person.Name));
    var constant = Expression.Constant(filterName, typeof(string));
    var body = Expression.Call(member, nameof(string.Contains), null, constant);
    var builtFilter = Expression.Lambda<Func<Person, bool>>(body, p);
    var builtResult = await context.People.Where(builtFilter).ToArrayAsync();
}