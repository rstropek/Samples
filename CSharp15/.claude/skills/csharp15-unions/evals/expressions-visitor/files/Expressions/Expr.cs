namespace Expressions;

/// <summary>Arithmetic expression tree. The node types are fixed; new operations are
/// added as visitors.</summary>
public abstract class Expr
{
    public abstract T Accept<T>(IExprVisitor<T> visitor);
}

public sealed class Num(double value) : Expr
{
    public double Value { get; } = value;
    public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitNum(this);
}

public sealed class Var(string name) : Expr
{
    public string Name { get; } = name;
    public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitVar(this);
}

public sealed class Binary(char op, Expr left, Expr right) : Expr
{
    public char Op { get; } = op;
    public Expr Left { get; } = left;
    public Expr Right { get; } = right;
    public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitBinary(this);
}

public sealed class Neg(Expr operand) : Expr
{
    public Expr Operand { get; } = operand;
    public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitNeg(this);
}

public interface IExprVisitor<T>
{
    T VisitNum(Num num);
    T VisitVar(Var var);
    T VisitBinary(Binary binary);
    T VisitNeg(Neg neg);
}
