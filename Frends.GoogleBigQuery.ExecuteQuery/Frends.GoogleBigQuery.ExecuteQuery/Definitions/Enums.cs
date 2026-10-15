namespace Frends.GoogleBigQuery.ExecuteQuery.Definitions;
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

public enum BigQueryDbTypes
{
    /// <summary>No explicit database type.</summary>
    Empty,

    /// <summary>A 64-bit integer.</summary>
    Int64,

    /// <summary>A 64-bit floating-point value.</summary>
    Float64,

    /// <summary>A Boolean value.</summary>
    Bool,

    /// <summary>A string value.</summary>
    String,

    /// <summary>A byte sequence.</summary>
    Bytes,

    /// <summary>A date value.</summary>
    Date,

    /// <summary>A date and time value.</summary>
    DateTime,

    /// <summary>A time value.</summary>
    Time,

    /// <summary>A timestamp value.</summary>
    Timestamp,

    /// <summary>An array value.</summary>
    Array,

    /// <summary>A structured value.</summary>
    Struct,

    /// <summary>A numeric value.</summary>
    Numeric,

    /// <summary>A geography value.</summary>
    Geography,

    /// <summary>A large numeric value.</summary>
    BigNumeric,

    /// <summary>A JSON value.</summary>
    Json,
}
