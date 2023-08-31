namespace Frends.GoogleBigQuery.Get.Definitions;
#pragma warning disable CS1591 // Self explanatory

/// <summary>
/// Resources.
/// </summary>
public enum Resources
{
    Datasets,
    Jobs,
    Models,
    Routines,
    Tables
}

/// <summary>
/// Method to read JSON credentials.
/// </summary>
public enum ReadJsonMethods
{
    JSON,
    File,
}

public enum Views
{
    /// <summary>
    /// Includes all table information, including storage statistics. 
    /// It returns same information as STORAGE_STATS view, but may contain additional information in the future.
    /// </summary>
    Full,

    /// <summary>
    /// Includes basic table information including schema and partitioning specification. 
    /// This view does not include storage statistics such as numRows or numBytes. 
    /// This view is significantly more efficient and should be used to support high query rates.
    /// </summary>
    Basic,

    /// <summary>
    /// Includes all information in the BASIC view as well as storage statistics (numBytes, numLongTermBytes, numRows and lastModifiedTime).
    /// </summary>
    Storagestats,

    /// <summary>
    /// The default value. Default to the STORAGE_STATS view.
    /// </summary>
    Tablemetadataviewunspecified
}