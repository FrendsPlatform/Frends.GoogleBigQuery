using System;

namespace Frends.GoogleBigQuery.Insert.Definitions;

/// <summary>
/// Error details.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Error creating credential from JSON or JSON parameters.</example>
    public string Message { get; set; }

    /// <summary>
    /// The exception that caused the error.
    /// </summary>
    /// <example>System.InvalidOperationException: Error creating credential from JSON or JSON parameters.</example>
    public Exception AdditionalInfo { get; set; }
}
