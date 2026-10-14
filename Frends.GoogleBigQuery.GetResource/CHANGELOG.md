# Changelog

## [2.0.0] - 2026-10-13
### Changed
- Task now targets .NET 8.
- Renamed the Options.ThrowOnError setting to ThrowErrorOnFailure.
- Failures are now reported in Result.Error (with Message and AdditionalInfo) instead of Result.ErrorMessage.

### Added
- Added Options.ErrorMessageOnFailure for overriding the error message on failure.
- [Breaking Change] Added the Input class and moved resource-specific parameters from Connection to Input.

## [1.0.0] - 2023-09-12
### Added
- Initial implementation