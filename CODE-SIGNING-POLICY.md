# Code signing policy

**Status: not active yet.** An application to SignPath Foundation has been prepared but not approved. Releases are currently **unsigned**, so Windows shows an "unknown publisher" warning. This page describes the policy that will apply once signing is approved.

## Attribution (applies once approved)

Free code signing provided by [SignPath.io](https://about.signpath.io), certificate by [SignPath Foundation](https://signpath.org).

## What is signed

Only binaries built from this repository by the GitHub Actions workflow `.github/workflows/psyduck-yellow-build.yml` on a GitHub-hosted runner: `PKHeX.exe`, `PKHeX.dll`, `PKHeX.Core.dll` and `PKHeX.Drawing*.dll`. Third-party libraries (for example QRCoder) are not signed by this project. Every release requires a manual approval of the signing request.

## Team roles

| Role | Member |
|---|---|
| Authors | ThatAnarchyDude |
| Reviewers | ThatAnarchyDude (changes from other contributors are reviewed before they are merged) |
| Approvers | ThatAnarchyDude (approves each signing request) |

All members use multi-factor authentication for GitHub and SignPath.

## Privacy statement

This program does not send any information to other networked systems unless the user explicitly requests it. It opens save files the user chooses and writes the files the user exports; it does not collect or transmit save data, paths or personal information.

## Verifying a download

Each release lists SHA-256 checksums (`SHA256SUMS`). The complete corresponding source is in this repository and attached to each release (PKHeX is GPL-3.0-or-later).
