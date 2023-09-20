using System.ComponentModel;

namespace Frends.GoogleBigQuery.ExecuteQuery.Definitions;

/// <summary>
/// Options parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Throw an error on exception.
    /// If set to false, exception message can be found in Result.ErrorMessage.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowOnError { get; set; }

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
}