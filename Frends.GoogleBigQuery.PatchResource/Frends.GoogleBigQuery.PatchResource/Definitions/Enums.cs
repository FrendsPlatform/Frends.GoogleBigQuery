namespace Frends.GoogleBigQuery.PatchResource.Definitions;
#pragma warning disable CS1591 // Self explanatory

/// <summary>
/// Resources.
/// </summary>
public enum Resources
{
    /// <summary>The dataset resource.</summary>
    Dataset,

    /// <summary>The table resource.</summary>
    Table,
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
    TimePartitioning,
}

public enum TimeTypeOptions
{
    /// <summary>Partition by day.</summary>
    DAY,

    /// <summary>Partition by hour.</summary>
    HOUR,

    /// <summary>Partition by month.</summary>
    MONTH,

    /// <summary>Partition by year.</summary>
    YEAR,
}
