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
