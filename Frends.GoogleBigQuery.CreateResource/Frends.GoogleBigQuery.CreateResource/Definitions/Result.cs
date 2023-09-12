namespace Frends.GoogleBigQuery.CreateResource.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// Operation complete without errors.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// URL to new dataset or table resource.
    /// New routine return an empty string.
    /// </summary>
    /// <example>true</example>
    public string Url { get; private set; }

    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>Error occured...</example>
    public string ErrorMessage { get; private set; }

    internal Result(bool success, string url, string errorMessage)
    {
        Success = success;
        Url = url;
        ErrorMessage = errorMessage;
    }
}