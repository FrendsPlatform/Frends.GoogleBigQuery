namespace Frends.GoogleBigQuery.ExecuteQuery.Definitions;
#pragma warning disable CS1591 // Self explanatory

/// <summary>
/// Method to read JSON credentials.
/// </summary>
public enum ReadJsonMethods
{
    JSON,
    File,
}

public enum BigQueryDbTypes
{
    Empty,
    Int64,
    Float64,
    Bool,
    String,
    Bytes,
    Date,
    DateTime,
    Time,
    Timestamp,
    Array,
    Struct,
    Numeric,
    Geography,
    BigNumeric,
    Json,
}