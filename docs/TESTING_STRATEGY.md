# Testing Strategy — MERQO VERTEXA

| Layer | Tool | Scope |
|---|---|---|
| Unit | xUnit | Provider parsing, health formulas, status resolution, settings migrations, report serialization |
| Integration | xUnit | Orchestrator runs modules end-to-end on CI (VM), DB round-trip |
| Functional | Scripted + manual checklist | Launch, navigate, run diagnostics, export report |
| UI | Manual + screenshots at 100/125/150/175/200% scaling | No clipping/overlap/broken controls |
| Security | `dotnet list package --vulnerable`, secret scan, network monitor | No telemetry, no secrets |
| Performance | Manual profiling | Monitoring polling within budget; no leaks |
| Hardware | Physical-device verification procedures (docs) | Items impossible to verify in a VM are documented, never claimed |

## Acceptance rule
A feature is VERIFIED only when: implemented + error handling works + output
validated + test executed and passed + docs updated. Compilation alone is never
proof of completion.
