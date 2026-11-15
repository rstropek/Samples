using System.Diagnostics;

namespace Billing;

public sealed class PaymentService(PaymentProvider provider)
{
    public Task<PaymentResult> ChargeAsync(string customerId, decimal amount, CancellationToken ct = default) =>
        provider.ChargeAsync(customerId, amount, ct);

    public static string Describe(PaymentResult result) => result switch
    {
        Approved a       => $"Approved {a.Amount:C} ({a.TransactionId})",
        Declined d       => $"Declined: {d.Reason}{(d.Retryable ? " (retry later)" : "")}",
        RequiresAction r => $"Customer action required at {r.RedirectUrl}",
        _ => throw new UnreachableException($"Unknown payment result {result.GetType().Name}"),
    };

    public static int ToHttpStatus(PaymentResult result)
    {
        if (result is Approved) return 200;
        if (result is Declined d) return d.Retryable ? 503 : 402;
        if (result is RequiresAction) return 202;
        throw new InvalidOperationException("Unhandled payment result");
    }

    public async Task<string> RefundAsync(string transactionId, CancellationToken ct = default)
    {
        var outcome = await provider.RefundAsync(transactionId, ct);
        if (outcome.Succeeded) return $"Refunded ({outcome.RefundId})";
        if (outcome.PendingUntil is { } until) return $"Refund queued until {until:u}";
        return $"Refund failed: {outcome.ErrorCode}";
    }
}
