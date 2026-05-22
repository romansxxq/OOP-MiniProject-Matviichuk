# Changelog
All notable changes to this project will be documented in this file.

The format is based on Keep a Changelog, and this project follows Semantic Versioning.

## [Unreleased]

## [1.0.0] - 2026-05-22
### Added
- Patient registration, doctor/department management, appointment lifecycle.
- Strategy + Composite validation rules.
- JSON persistence with async load/save.
- LINQ queries and analytics.
- Console UI with separated input/output helpers.
- Test strategy, matrix, and CI coverage.

### Changed
- Repositories generalized with `IRepository<T, TId>`.
- Appointment validation uses multiple strategies.

### Fixed
- Data validation on load (duplicate IDs, missing links).
- Validation ignores cancelled appointments.
