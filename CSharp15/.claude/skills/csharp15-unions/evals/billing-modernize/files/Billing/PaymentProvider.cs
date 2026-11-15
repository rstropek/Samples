namespace Billing;

/// <summary>
/// Base class for payment provider adapters. The adapters ship as separate NuGet
/// packages (Billing.Stripe, Billing.Adyen, Billing.PayPal, ...) maintained by
/// other teams, and each of them derives from this class.
/// </summary>
public abstract class PaymentProvider
{
    public abstract string Name { get; }
    public abstract Task<PaymentResult> ChargeAsync(string customerId, decimal amount, CancellationToken ct);
    public abstract Task<RefundOutcome> RefundAsync(string transactionId, CancellationToken ct);
}

/// <summary>In-memory provider for tests.</summary>
public sealed class FakeProvider : PaymentProvider
{
    public override string Name => "fake";

    public override Task<PaymentResult> ChargeAsync(string customerId, decimal amount, CancellationToken ct) =>
        Task.FromResult<PaymentResult>(amount switch
        {
            <= 0 => new Declined("Amount must be positive", Retryable: false),
            > 10_000 => new RequiresAction(new Uri("https://example.test/3ds")),
            _ => new Approved($"tx-{Guid.NewGuid():N}", amount),
        });

    public override Task<RefundOutcome> RefundAsync(string transactionId, CancellationToken ct) =>
        Task.FromResult(transactionId.StartsWith("tx-")
            ? RefundOutcome.Success($"rf-{transactionId[3..]}")
            : RefundOutcome.Failure("unknown_transaction"));
}
