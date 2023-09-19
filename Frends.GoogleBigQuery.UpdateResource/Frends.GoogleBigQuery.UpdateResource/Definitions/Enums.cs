namespace Frends.GoogleBigQuery.UpdateResource.Definitions;
#pragma warning disable CS1591 // Self explanatory

/// <summary>
/// Resources.
/// </summary>
public enum Resources
{
    Dataset,
    Routine,
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
/// DeterminismLevels.
/// </summary>
public enum DeterminismLevels
{
    /// <summary>
    /// The determinism of the UDF is unspecified.
    /// </summary>
    DETERMINISM_LEVEL_UNSPECIFIED,

    /// <summary>
    /// The UDF is deterministic, meaning that 2 function calls with the same inputs always produce the same result, even across 2 query runs.
    /// </summary>
    DETERMINISTIC,

    /// <summary>
    /// The UDF is not deterministic.
    /// </summary>
    NOT_DETERMINISTIC
}

/// <summary>
/// Routine types.
/// </summary>
public enum RoutineTypes
{
    SCALAR_FUNCTION,
    PROCEDURE,
    TABLE_FUNCTION
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

public enum ArgumentKindOptions
{
    /// <summary>
    /// Default value.
    /// </summary>
    ARGUMENT_KIND_UNSPECIFIED,

    /// <summary>
    /// The argument is a variable with fully specified type, which can be a struct or an array, but not a table.
    /// </summary>
    FIXED_TYPE,

    /// <summary>
    /// The argument is any type, including struct or array, but not a table. To be added: FIXED_TABLE, ANY_TABLE.
    /// </summary>
    ANY_TYPE
}