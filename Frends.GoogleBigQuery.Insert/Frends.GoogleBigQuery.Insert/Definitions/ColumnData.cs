using System.ComponentModel.DataAnnotations;

namespace Frends.GoogleBigQuery.Insert.Definitions;

/// <summary>
/// ColumnData.
/// </summary>
public class ColumnData
{
    /// <summary>
    /// Column.
    /// </summary>
    /// <example>foo</example>
    public string ColumnName { get; set; }

    /// <summary>
    /// Value.
    /// </summary>
    /// <example>bar123</example>
    [DisplayFormat(DataFormatString = "Text")]
    public dynamic Value { get; set; }
}
