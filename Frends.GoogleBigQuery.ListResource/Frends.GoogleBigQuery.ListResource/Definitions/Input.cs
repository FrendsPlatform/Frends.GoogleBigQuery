using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.GoogleBigQuery.ListResource.Definitions;

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
}
