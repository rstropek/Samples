namespace Billing;

/// <summary>Result of a refund request.</summary>
/// <remarks>
/// Exactly one of the following is true:
///  * Succeeded == true  and RefundId is set
///  * Succeeded == false and ErrorCode is set
///  * Succeeded == false and PendingUntil is set (refund queued by the provider)
/// </remarks>
public sealed class RefundOutcome
{
    public bool Succeeded { get; init; }
    public string? RefundId { get; init; }
    public string? ErrorCode { get; init; }
    public DateTimeOffset? PendingUntil { get; init; }

    public static RefundOutcome Success(string refundId) => new() { Succeeded = true, RefundId = refundId };
    public static RefundOutcome Failure(string errorCode) => new() { ErrorCode = errorCode };
    public static RefundOutcome Pending(DateTimeOffset until) => new() { PendingUntil = until };
}
