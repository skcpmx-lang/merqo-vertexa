namespace Merqo.Vertexa.Core.Models;

/// <summary>
/// A single diagnostic value with full provenance. This is the ONLY way data
/// leaves the provider layer — every field carries source, method, timestamp,
/// status, confidence and error information.
/// </summary>
public sealed class DiagnosticResult<T>
{
    public required T? Value { get; init; }
    public string? Unit { get; init; }
    public required string Source { get; init; }
    public required string Method { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    public DiagnosticStatus Status { get; init; } = DiagnosticStatus.Unavailable;
    /// <summary>0..1 confidence in the value given the source.</summary>
    public double Confidence { get; init; }
    public string? Error { get; init; }

    public bool IsAvailable => Status is DiagnosticStatus.Verified or DiagnosticStatus.Partial or DiagnosticStatus.Estimated
        && Value is not null;

    public static DiagnosticResult<T> Ok(T value, string source, string method, string? unit = null, double confidence = 0.95)
        => new() { Value = value, Source = source, Method = method, Unit = unit, Status = DiagnosticStatus.Verified, Confidence = confidence };

    public static DiagnosticResult<T> PartialOk(T value, string source, string method, string? reason = null)
        => new() { Value = value, Source = source, Method = method, Status = DiagnosticStatus.Partial, Confidence = 0.6, Error = reason };

    public static DiagnosticResult<T> Unsupported(string source, string method, string reason)
        => new() { Value = default, Source = source, Method = method, Status = DiagnosticStatus.Unsupported, Confidence = 0, Error = reason };

    public static DiagnosticResult<T> Unavailable(string source, string method, string reason)
        => new() { Value = default, Source = source, Method = method, Status = DiagnosticStatus.Unavailable, Confidence = 0, Error = reason };

    public static DiagnosticResult<T> Failed(string source, string method, string error)
        => new() { Value = default, Source = source, Method = method, Status = DiagnosticStatus.Failed, Confidence = 0, Error = error };

    public static DiagnosticResult<T> PermissionRequired(string source, string method, string reason)
        => new() { Value = default, Source = source, Method = method, Status = DiagnosticStatus.PermissionRequired, Confidence = 0, Error = reason };

    /// <summary>Estimated is only valid with a documented calculation method.</summary>
    public static DiagnosticResult<T> EstimatedOk(T value, string source, string method, string calculationMethod)
        => new() { Value = value, Source = source, Method = method, Status = DiagnosticStatus.Estimated, Confidence = 0.5, Error = $"Calculated: {calculationMethod}" };
}
