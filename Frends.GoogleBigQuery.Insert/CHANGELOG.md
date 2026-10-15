# Changelog

## [2.0.0] - 2026-10-13
### Changed
- Task now targets .NET 8.
- Task parameters are now in the order Input, Connection, Options, so existing Task usages need their parameters re-checked.
- Renamed the Options.ThrowOnError setting to ThrowErrorOnFailure.
- Failures are now reported in Result.Error (with Message and AdditionalInfo) instead of Result.ErrorMessage.

### Added
- Added Options.ErrorMessageOnFailure for overriding the error message on failure.

## [1.0.0] - 2023-09-08
### Added
- Initial implementation