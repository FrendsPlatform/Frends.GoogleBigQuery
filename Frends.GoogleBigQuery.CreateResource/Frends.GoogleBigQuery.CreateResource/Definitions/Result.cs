namespace Frends.GoogleBigQuery.CreateResource.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    internal Result(bool success, string url, Error error = null)
    {
        Success = success;
        Url = url;
        Error = error;
    }

    /// <summary>
    /// Operation complete without errors.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// URL to new dataset or table resource.
    /// New routine return an empty string.
    /// </summary>
    /// <example>https://bigquery.googleapis.com/bigquery/v2/projects/my-project/datasets/my_dataset</example>
    public string Url { get; private set; }

    /// <summary>
    /// Error details. Null when Success is true.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; private set; }
}
