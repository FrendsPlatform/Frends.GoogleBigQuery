using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.GoogleBigQuery.DeleteResource.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Resource.
    /// </summary>
    /// <example>Resources.Tables</example>
    [DefaultValue(Resources.Table)]
    public Resources Resource { get; set; }

    /// <summary>
    /// Dataset ID.
    /// </summary>
    /// <example>datasetid</example>
    public string DatasetId { get; set; }

    /// <summary>
    /// Table ID.
    /// </summary>
    /// <example>tableid</example>
    [UIHint(nameof(Resource), "", Resources.Table)]
    public string TableId { get; set; }

    /// <summary>
    /// Job ID
    /// </summary>
    /// <example>jobid</example>
    [UIHint(nameof(Resource), "", Resources.Job)]
    public string JobId { get; set; }

    /// <summary>
    /// Model ID.
    /// </summary>
    /// <example>modeid</example>
    [UIHint(nameof(Resource), "", Resources.Model)]
    public string ModelId { get; set; }

    /// <summary>
    /// Routine ID.
    /// </summary>
    /// <example>routineid</example>
    [UIHint(nameof(Resource), "", Resources.Routine)]
    public string RoutineId { get; set; }
}