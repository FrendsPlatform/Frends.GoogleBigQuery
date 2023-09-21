namespace Frends.GoogleBigQuery.PatchResource.Definitions;
#pragma warning disable CS1591 // Self explanatory

/// <summary>
/// Resources.
/// </summary>
public enum Resources
{
    Dataset,
    Table
}

/// <summary>
/// Method to read JSON credentials.
/// </summary>
public enum ReadJsonMethods
{
    JSON,
    File,
}

/// <summary>
/// TablePartitionOptions
/// </summary>
public enum TablePartitionOptions
{
    /// <summary>
    /// Hide options from UI.
    /// </summary>
    None,

    /// <summary>
    /// Range partitioning specification for this table.
    /// </summary>
    RangePartitioning,

    /// <summary>
    /// Time-based partitioning specification for this table.
    /// </summary>
    TimePartitioning
}

public enum TimeTypeOptions
{
    DAY,
    HOUR,
    MONTH,
    YEAR
}