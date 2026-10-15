namespace Frends.GoogleBigQuery.ListResource.Definitions;
#pragma warning disable CS1591 // Self explanatory

/// <summary>
/// Resources.
/// </summary>
public enum Resources
{
    /// <summary>The datasets resource.</summary>
    Datasets,

    /// <summary>The jobs resource.</summary>
    Jobs,

    /// <summary>The models resource.</summary>
    Models,

    /// <summary>The projects resource.</summary>
    Projects,

    /// <summary>The routines resource.</summary>
    Routines,

    /// <summary>The row access policies resource.</summary>
    RowAccessPolicies,

    /// <summary>The table data resource.</summary>
    TableData,

    /// <summary>The tables resource.</summary>
    Tables,
}

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
