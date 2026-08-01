# Security Policy

## Supported versions

| Version | Supported |
| --- | --- |
| 2.0.x | Yes |
| 1.1.x and earlier | No — please upgrade; see [CHANGELOG.md](CHANGELOG.md) for the migration table |

## Reporting a vulnerability

Please report security issues privately rather than in a public issue:

1. Open a [private security advisory](https://github.com/i7aket/NPKTools/security/advisories/new) on
   this repository, or
2. contact the maintainer via [LinkedIn](https://www.linkedin.com/in/anatoliyyermakov).

Please include the affected package and version, what an attacker can achieve, and a reproduction if
you have one. You can expect an acknowledgement within a week.

## Scope

NPKTools is a set of computational libraries. They perform no I/O, open no network connections, read
no files, and execute no external processes. The realistic threat surface is therefore small and
consists mostly of input handling:

- `PpmTargetParser.Parse` accepts arbitrary strings. It is the only component that parses untrusted
  text. Malformed input is expected to raise `FormatException` or `ArgumentException`, never to hang
  or crash the process.
- The optimizer accepts caller-supplied fertilizer catalogues and nutrient targets. Solving is
  bounded: `SimplexOptimizationSolver` caps itself at 10,000 pivots, so a pathological input returns
  no solution rather than looping forever.
- Reports are plain strings. If you render them as HTML, escape them like any other data — the
  library does not do it for you, since fertilizer names are caller-supplied.

Reports of denial of service through deliberately enormous catalogues are in scope; the library is
intended to fail gracefully rather than exhaust the host.

## Dependencies

`SYT.NPKTools` depends only on `Microsoft.Extensions.DependencyInjection.Abstractions`, and carries no
native binaries. `Google.OrTools` is a test-only dependency — it backs the differential oracle the
shipped solver is validated against and is never distributed to consumers. Dependency updates are
tracked automatically by Dependabot.
