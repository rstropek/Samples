namespace Billing;

/// <summary>Outcome of a charge attempt.</summary>
/// <remarks>
/// The private protected constructor keeps other assemblies from adding their own
/// result types: callers switch over these three cases.
/// </remarks>
public abstract record PaymentResult
{
    private protected PaymentResult() { }
}

public sealed record Approved(string TransactionId, decimal Amount) : PaymentResult;
public sealed record Declined(string Reason, bool Retryable) : PaymentResult;
public sealed record RequiresAction(Uri RedirectUrl) : PaymentResult;
