using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.GoogleBigQuery.ExecuteQuery.Definitions;

/// <summary>
/// Options parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Whether to throw an error on failure.
    /// If set to false, the error can be found in Result.Error.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Maximum number of retry attempts before throwing an exception or ending this Task.
    /// </summary>
    /// <example>1</example>
    public int MaxRetryAttempts { get; set; }

    /// <summary>
    /// Delay in seconds between retry attempts.
    /// </summary>
    /// <example>5</example>
    public int Delay { get; set; }

    /// <summary>
    /// Overrides the error message on failure. If `ThrowErrorOnFailure` is set to `true`, then the original exception will be wrapped in a new Exception with this error message.
    /// </summary>
    /// <example>Executing the BigQuery query failed</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = string.Empty;
}
