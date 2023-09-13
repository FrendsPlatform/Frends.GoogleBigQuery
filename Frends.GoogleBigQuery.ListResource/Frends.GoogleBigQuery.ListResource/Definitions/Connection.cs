﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.GoogleBigQuery.ListResource.Definitions;

/// <summary>
/// Connection parameters.
/// </summary>
public class Connection
{
    /// <summary>
    /// Service base URI.
    /// If empty, the default base URI for the service is used.
    /// </summary>
    /// <example>https://bigquery.googleapis.com</example>
    public string BaseUri { get; set; }

    /// <summary>
    /// Resource.
    /// </summary>
    /// <example>Resources.Tables</example>
    [DefaultValue(Resources.Tables)]
    public Resources Resource { get; set; }

    /// <summary>
    /// Project ID.
    /// </summary>
    /// <example>global-env-397309</example>
    public string ProjectId { get; set; }

    /// <summary>
    /// Dataset ID.
    /// </summary>
    /// <example>baseball</example>
    public string DatasetId { get; set; }

    /// <summary>
    /// Table ID.
    /// </summary>
    /// <example>games_post_wide</example>
    public string TableId { get; set; }

    /// <summary>
    /// Method to read Service account JSON.
    /// </summary>
    /// <example>ReadJsonMethods.File</example>
    [DefaultValue(ReadJsonMethods.File)]
    public ReadJsonMethods ReadJsonMethod { get; set; }

    /// <summary>
    /// Service account key file.
    /// </summary>
    /// <example>
    /// {
    ///   "type": "service_account",
    ///   "project_id": "your-project-id",
    ///   ...
    /// }
    /// </example>
    [UIHint(nameof(ReadJsonMethod), "", ReadJsonMethods.JSON)]
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    public string SecretJson { get; set; }

    /// <summary>
    /// Filepath to service account key file.
    /// </summary>
    /// <example>C:\temp\jsonfile.json</example>
    [UIHint(nameof(ReadJsonMethod), "", ReadJsonMethods.File)]
    public string CredentialsFilePath { get; set; }
}