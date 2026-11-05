namespace Merqo.Vertexa.Core.Models;

/// <summary>
/// Lifecycle status of a diagnostic value. A value may ONLY be reported as
/// <see cref="Verified"/> when it came from a real executed source and passed
/// validation. Fabricated values are never produced.
/// </summary>
public enum DiagnosticStatus
{
    Verified,
    Partial,
    Unavailable,
    Unsupported,
    PermissionRequired,
    Failed,
    NotTested,
    /// <summary>Only allowed with a documented calculation method.</summary>
    Estimated
}
