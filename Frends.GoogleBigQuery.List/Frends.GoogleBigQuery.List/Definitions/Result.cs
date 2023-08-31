﻿using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;

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
    /// Request content as List of DataSets.
    /// </summary>
    /// <example>{{ "id": "151959", "key": "TT-87", "self": "https://test.jira.fi/jira/rest/api/latest/issue/151959" }}</example>
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