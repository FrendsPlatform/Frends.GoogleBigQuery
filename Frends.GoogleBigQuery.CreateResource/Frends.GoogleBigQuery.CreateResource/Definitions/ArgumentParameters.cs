namespace Frends.GoogleBigQuery.CreateResource.Definitions;

/// <summary>
/// ArgumentParameters.
/// </summary>
public class ArgumentParameters
{
    /// <summary>
    /// The name of this argument.
    /// Can be absent for function return argument.
    /// </summary>
    /// <example>y</example>
    public string Name { get; set; }

    /// <summary>
    /// Mode (Optional)
    /// Specifies whether the argument is input or output.
    /// Can be set for procedures only.
    /// </summary>
    /// <example>OUT</example>
    public string Mode { get; set; }

    /// <summary>
    /// DataType.
    /// Required unless ArgumentKind = ANY_TYPE.
    /// </summary>
    /// <example>INT64</example>
    public string DataType { get; set; }

    /// <summary>
    /// ArgumentKind. (Optional)
    /// </summary>
    /// <example>ArgumentKindOptions.FIXED_TYPE</example>
    public ArgumentKindOptions ArgumentKind { get; set; }
}
