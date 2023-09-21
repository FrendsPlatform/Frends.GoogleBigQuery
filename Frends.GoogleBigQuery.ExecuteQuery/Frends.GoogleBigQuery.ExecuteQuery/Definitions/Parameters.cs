using System.ComponentModel;

namespace Frends.GoogleBigQuery.ExecuteQuery.Definitions;

/// <summary>
/// BigQueryParameters.
/// </summary>
public class BigQueryParameters
{
    /// <summary>
    /// Name.
    /// </summary>
    /// <example>foo</example>
    public string Name { get; set; }

    /// <summary>
    /// BigQueryDbType.
    /// </summary>
    /// <example>BigQueryDbTypes.Int64</example>
    [DefaultValue(BigQueryDbTypes.Empty)]
    public BigQueryDbTypes BigQueryDbType { get; set; }

    /// <summary>
    /// Value.
    /// </summary>
    /// <example>bar</example>
    public object Value { get; set; }
}