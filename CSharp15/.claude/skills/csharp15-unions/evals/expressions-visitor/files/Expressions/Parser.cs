using System.Globalization;

namespace Expressions;

/// <summary>Tokenizer + recursive-descent parser for expressions like "-(x + 2) * 3".</summary>
public static class Parser
{
    /// <summary>Yields a <see cref="double"/> for numbers, a <see cref="char"/> for
    /// operators and parentheses, and a <see cref="string"/> for identifiers.</summary>
    public static IEnumerable<object> Tokenize(string text)
    {
        int i = 0;
        while (i < text.Length)
        {
            char c = text[i];
            if (char.IsWhiteSpace(c)) { i++; continue; }
            if (char.IsDigit(c) || c == '.')
            {
                int start = i;
                while (i < text.Length && (char.IsDigit(text[i]) || text[i] == '.')) i++;
                yield return double.Parse(text.AsSpan(start, i - start), CultureInfo.InvariantCulture);
            }
            else if (char.IsLetter(c))
            {
                int start = i;
                while (i < text.Length && char.IsLetterOrDigit(text[i])) i++;
                yield return text[start..i];
            }
            else if ("+-*/()".Contains(c)) { yield return c; i++; }
            else throw new FormatException($"Unexpected character '{c}' at {i}");
        }
    }

    public static Expr Parse(string text)
    {
        var tokens = Tokenize(text).ToList();
        int pos = 0;
        var expr = ParseSum(tokens, ref pos);
        if (pos != tokens.Count) throw new FormatException($"Unexpected token {tokens[pos]}");
        return expr;
    }

    private static Expr ParseSum(List<object> t, ref int pos)
    {
        var left = ParseProduct(t, ref pos);
        while (pos < t.Count && t[pos] is char op && (op == '+' || op == '-'))
        {
            pos++;
            left = new Binary(op, left, ParseProduct(t, ref pos));
        }
        return left;
    }

    private static Expr ParseProduct(List<object> t, ref int pos)
    {
        var left = ParseUnary(t, ref pos);
        while (pos < t.Count && t[pos] is char op && (op == '*' || op == '/'))
        {
            pos++;
            left = new Binary(op, left, ParseUnary(t, ref pos));
        }
        return left;
    }

    private static Expr ParseUnary(List<object> t, ref int pos)
    {
        if (pos >= t.Count) throw new FormatException("Unexpected end of input");
        var token = t[pos++];
        if (token is double d) return new Num(d);
        if (token is string name) return new Var(name);
        if (token is char c)
        {
            if (c == '-') return new Neg(ParseUnary(t, ref pos));
            if (c == '(')
            {
                var inner = ParseSum(t, ref pos);
                if (pos >= t.Count || t[pos] is not ')') throw new FormatException("Missing ')'");
                pos++;
                return inner;
            }
        }
        throw new FormatException($"Unexpected token {token}");
    }
}
