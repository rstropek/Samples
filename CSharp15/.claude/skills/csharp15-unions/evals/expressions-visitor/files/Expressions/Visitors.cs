namespace Expressions;

public sealed class Evaluator(IReadOnlyDictionary<string, double> variables) : IExprVisitor<double>
{
    public double VisitNum(Num num) => num.Value;
    public double VisitVar(Var var) => variables.TryGetValue(var.Name, out var v)
        ? v : throw new KeyNotFoundException($"Unknown variable {var.Name}");
    public double VisitNeg(Neg neg) => -neg.Operand.Accept(this);
    public double VisitBinary(Binary b)
    {
        double l = b.Left.Accept(this), r = b.Right.Accept(this);
        return b.Op switch
        {
            '+' => l + r,
            '-' => l - r,
            '*' => l * r,
            '/' => l / r,
            _ => throw new InvalidOperationException($"Unknown operator {b.Op}"),
        };
    }
}

public sealed class Printer : IExprVisitor<string>
{
    public string VisitNum(Num num) => num.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
    public string VisitVar(Var var) => var.Name;
    public string VisitNeg(Neg neg) => $"-{neg.Operand.Accept(this)}";
    public string VisitBinary(Binary b) => $"({b.Left.Accept(this)} {b.Op} {b.Right.Accept(this)})";
}

/// <summary>Constant folding and x*1 / x+0 elimination.</summary>
public sealed class Simplifier : IExprVisitor<Expr>
{
    public Expr VisitNum(Num num) => num;
    public Expr VisitVar(Var var) => var;
    public Expr VisitNeg(Neg neg) => neg.Operand.Accept(this) is Num n ? new Num(-n.Value) : new Neg(neg.Operand.Accept(this));
    public Expr VisitBinary(Binary b)
    {
        var l = b.Left.Accept(this);
        var r = b.Right.Accept(this);
        if (l is Num ln && r is Num rn)
            return new Num(new Evaluator(new Dictionary<string, double>()).VisitBinary(new Binary(b.Op, ln, rn)));
        if (b.Op == '*' && r is Num { Value: 1 }) return l;
        if (b.Op == '+' && r is Num { Value: 0 }) return l;
        return new Binary(b.Op, l, r);
    }
}
