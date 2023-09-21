namespace Frends.GoogleBigQuery.ExecuteQuery.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Query.
    /// </summary>
    /// <example>SELECT * FROM table</example>
    public string Query { get; set; }

    /// <summary>
    /// Parameters.
    /// </summary>
    /// <example>parametername</example>
    public BigQueryParameters[] BigQueryParameter { get; set; }
}