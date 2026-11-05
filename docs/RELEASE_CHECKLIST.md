# Release Checklist — MERQO VERTEXA

- [ ] All P1 requirements in `FEATURE_CHECKLIST.md` are VERIFIED or documented PARTIAL/BLOCKED
- [ ] CI green: build, unit + integration tests, static analysis, vulnerability scan
- [ ] Clean build on windows-latest; EXE launches; dashboard shows real data
- [ ] Settings persist; logs written; reports export (JSON/HTML/CSV) and open correctly
- [ ] No telemetry / no secrets in repo; SEC requirements verified
- [ ] Artifacts: portable EXE, installer layout, SHA256SUMS.txt, release notes
- [ ] Unsigned-release disclosure included (SmartScreen "Unknown publisher" note)
- [ ] Git tag `vX.Y.Z` created; GitHub Release published; checksums match artifacts
- [ ] Honest verification report attached (verified / partial / VM-only / unsupported)

Never publish a release with a known critical defect.
