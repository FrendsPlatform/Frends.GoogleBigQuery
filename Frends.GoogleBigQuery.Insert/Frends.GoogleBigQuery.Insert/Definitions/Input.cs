using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.GoogleBigQuery.Insert.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Source format.
    /// </summary>
    /// <example>SourceFormats.CSV</example>
    [DefaultValue(SourceFormats.CSV)]
    public SourceFormats SourceFormat { get; set; }

    /// <summary>
    /// CSV.
    /// </summary>
    /// <example>
    /// Col1;Col2;Col3
    /// Foo Csv;1;Fooland
    /// Bar Csv;2;Barland
    /// </example>
    [UIHint(nameof(SourceFormat), "", SourceFormats.CSV)]
    public string Csv { get; set; }

    /// <summary>
    /// CSV delimiter.
    /// </summary>
    /// <example>;</example>
    [UIHint(nameof(SourceFormat), "", SourceFormats.CSV)]
    public string Delimiter { get; set; }

    /// <summary>
    /// Number of top rows to skip.
    /// </summary>
    /// <example>0</example>
    [UIHint(nameof(SourceFormat), "", SourceFormats.CSV)]
    [DefaultValue(0)]
    public int SkipTopRows { get; set; }

    /// <summary>
    /// Row data.
    /// </summary>
    /// <example>[ [{ Column1, Value1 }, { Column2, Value1 }], [{ Column1, Value2 }, { Column2, Value2 }] ]</example>
    [UIHint(nameof(SourceFormat), "", SourceFormats.RowData)]
    public RowData[] RowData { get; set; }

    /// <summary>
    /// JSON.
    /// </summary>
    /// <example>[ {"Name": "Foo", "Age": 1, "Country": "Fooland"}, {"Name": "Bar", "Age": 2, "Country": "Barland"} ]"</example>
    [UIHint(nameof(SourceFormat), "", SourceFormats.JSON)]
    public string Json { get; set; }
}