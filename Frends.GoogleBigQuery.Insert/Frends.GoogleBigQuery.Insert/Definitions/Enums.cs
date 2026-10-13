namespace Frends.GoogleBigQuery.Insert.Definitions;
#pragma warning disable CS1591 // Self explanatory

/// <summary>
/// Method to read JSON credentials.
/// </summary>
public enum ReadJsonMethods
{
    /// <summary>Read credentials from JSON text.</summary>
    JSON,

    /// <summary>Read credentials from a file.</summary>
    File,
}

public enum SourceFormats
{
    /// <summary>Comma-separated values.</summary>
    CSV,

    /// <summary>JSON data.</summary>
    JSON,

    /// <summary>Individual row data.</summary>
    RowData,
}
