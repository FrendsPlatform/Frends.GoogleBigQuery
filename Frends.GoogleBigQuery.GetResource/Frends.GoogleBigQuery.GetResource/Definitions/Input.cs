using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.GoogleBigQuery.GetResource.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Resource.
    /// </summary>
    /// <example>Resources.Tables</example>
    [DefaultValue(Resources.Tables)]
    public Resources Resource { get; set; }

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
    /// Job ID.
    /// </summary>
    /// <example>my_job</example>
    public string JobId { get; set; }

    /// <summary>
    /// Model ID.
    /// </summary>
    /// <example>my_model</example>
    public string ModelId { get; set; }

    /// <summary>
    /// Routine ID.
    /// </summary>
    /// <example>my_routine</example>
    public string RoutineId { get; set; }

    /// <summary>
    /// If set, only the Routine fields in the field mask are returned in the response.
    /// If unset, all Routine fields are returned.
    /// This is a comma-separated list of fully qualified names of fields.
    /// </summary>
    /// <example>user.displayName,photos</example>
    [UIHint(nameof(Resource), "", Resources.Routines)]
    public string ReadMask { get; set; }

    /// <summary>
    /// Table schema fields to return (comma-separated).
    /// If unspecified, all fields are returned.
    /// A fieldMask cannot be used here because the fields will automatically be converted from camelCase to snake_case and the conversion will fail if there are underscores.
    /// Since these are fields in BigQuery table schemas, underscores are allowed.
    /// </summary>
    /// <example>foo,bar</example>
    [UIHint(nameof(Resource), "", Resources.Tables)]
    public string SelectedFields { get; set; }

    /// <summary>
    /// Specifies the view that determines which table information is returned.
    /// By default, basic table information and storage statistics (STORAGE_STATS) are returned.
    /// </summary>
    /// <example>Views.Storagestats</example>
    [UIHint(nameof(Resource), "", Resources.Tables)]
    [DefaultValue(Views.Storagestats)]
    public Views View { get; set; }
}
