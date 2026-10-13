namespace Frends.GoogleBigQuery.GetResource.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    internal Result(bool success, dynamic data, Error error = null)
    {
        Success = success;
        Data = data;
        Error = error;
    }

    /// <summary>
    /// Operation complete without errors.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Data as JToken.
    /// </summary>
    /// <example>{{ "access": [ { "dataset": null, "domain": null, "groupByEmail": null ... }}</example>
    public dynamic Data { get; private set; }

    /// <summary>
    /// Error details. Null when Success is true.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; private set; }
}
