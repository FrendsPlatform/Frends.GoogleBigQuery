using Newtonsoft.Json.Linq;

namespace Frends.GoogleBigQuery.List.Definitions;

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
    /// Data.
    /// </summary>
    /// <example>{{  "datasets": [ { "datasetReference": { "datasetId": "tasktest", "projectId": "instant-stone-387712",...</example>
    public JToken Data { get; private set; }

    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>Error occured...</example>
    public string ErrorMessage { get; private set; }

    internal Result(bool success, JToken data, string errorMessage)
    {
        Success = success;
        Data = data;
        ErrorMessage = errorMessage;
    }
}