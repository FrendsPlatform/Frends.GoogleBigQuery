using System.Text.Json;

namespace Frends.GoogleBigQuery.GetResource.Definitions;

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
    /// Request content as Get of DataSets.
    /// </summary>
    /// <example>{{	"access": [ { "dataset": null, "domain": null, "groupByEmail": null ...	}}</example>
    public JsonElement? Data { get; private set; }

    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>Error occured...</example>
    public string ErrorMessage { get; private set; }

    internal Result(bool success, JsonElement? data, string errorMessage)
    {
        Success = success;
        Data = data;
        ErrorMessage = errorMessage;
    }
}