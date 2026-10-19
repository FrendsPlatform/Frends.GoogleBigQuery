namespace Frends.GoogleBigQuery.DeleteResource.Definitions;
#pragma warning disable CS1591 // Self explanatory

/// <summary>
/// Resources.
/// </summary>
public enum Resources
{
    /// <summary>The dataset resource.</summary>
    Dataset,

    /// <summary>The job resource.</summary>
    Job,

    /// <summary>The model resource.</summary>
    Model,

    /// <summary>The routine resource.</summary>
    Routine,

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
