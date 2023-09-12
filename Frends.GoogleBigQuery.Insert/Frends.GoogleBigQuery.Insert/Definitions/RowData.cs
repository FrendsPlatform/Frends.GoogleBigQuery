using System.ComponentModel.DataAnnotations;

namespace Frends.GoogleBigQuery.Insert.Definitions;

/// <summary>
/// RowData.
/// </summary>
public class RowData
{
    /// <summary>
    /// Row data.
    /// </summary>
    /// <example>[{ Column1, Value1 }, { Column2, Value1 }]</example>
    public ColumnData[] Row { get; set; }
}

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
