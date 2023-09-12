namespace Frends.GoogleBigQuery.Insert.Definitions;
#pragma warning disable CS1591 // Self explanatory

/// <summary>
/// Method to read JSON credentials.
/// </summary>
public enum ReadJsonMethods
{
    JSON,
    File,
}

public enum SourceFormats
{
    CSV,
    JSON,
    RowData
}