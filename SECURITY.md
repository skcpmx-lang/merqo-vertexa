# Security & Privacy Policy — MERQO VERTEXA

## Principles

1. **Local-first.** All diagnostics run on the local machine. No mandatory cloud
   service, account, or paid API. Core diagnostics work fully offline.
2. **No telemetry by default.** No hidden data collection. Any optional update
   check is transparent and off by default.
3. **Least privilege.** The app runs unelevated whenever possible. Elevation is
   requested only via UAC for operations that genuinely require it.
4. **No destructive defaults.** Storage tests are non-destructive; no formatting,
   deletion, or overwrite of user data without explicit informed consent.
5. **No security bypass.** The app never disables Defender, SmartScreen, UAC, or
   modifies drivers/Windows configuration without explicit user authorization.
6. **Privacy.** Serial numbers and identifying data are masked by default in
   reports and exports; the user controls privacy settings.

## Safe process execution

- External processes (e.g. `powercfg`, PowerShell) are launched with fixed
  argument lists — no shell string interpolation from untrusted input.
- Strict input validation on all file paths; path-traversal checks on report
  export destinations. External links are validated against an allowlist before
  `Process.Start` with `UseShellExecute`.

## Dependencies

- NuGet dependencies are pinned in `Directory.Packages.props` (central package
  management) and audited by `dotnet list package --vulnerable` in CI.
- No secrets are stored in source; GitHub Actions secrets are used only where
  strictly necessary.

## Reporting a vulnerability

Email merqoonline@gmail.com with details. Do not open public issues for
unpatched vulnerabilities.
