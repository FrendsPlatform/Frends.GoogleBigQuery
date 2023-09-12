﻿namespace Frends.GoogleBigQuery.GetResource.Definitions;

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
    /// Data as JToken.
    /// </summary>
    /// <example>{{	"access": [ { "dataset": null, "domain": null, "groupByEmail": null ...	}}</example>
    public dynamic Data { get; private set; }

    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>Error occured...</example>
    public string ErrorMessage { get; private set; }

    internal Result(bool success, dynamic data, string errorMessage)
    {
        Success = success;
        Data = data;
        ErrorMessage = errorMessage;
    }
}