namespace Frends.GoogleBigQuery.UpdateResource.Definitions;

/// <summary>
/// TagParameters.
/// </summary>
public class TagParameters
{
    /// <summary>
    /// Key.
    /// </summary>
    /// <example>foo</example>
    public string Key { get; set; }

    /// <summary>
    /// Value.
    /// </summary>
    /// <example>bar</example>
    public string Value { get; set; }
}

/// <summary>
/// TableSchemaParameters.
/// </summary>
public class TableSchemaParameters
{
    /// <summary>
    /// Name.
    /// </summary>
    /// <example>Name</example>
    public string Name { get; set; }

    /// <summary>
    /// Type.
    /// </summary>
    /// <example>STRING</example>
    public string Type { get; set; }
}

/// <summary>
/// LabelParameters.
/// </summary>
public class LabelParameters
{
    /// <summary>
    /// Key.
    /// </summary>
    /// <example>foo</example>
    public string Key { get; set; }

    /// <summary>
    /// Value.
    /// </summary>
    /// <example>bar</example>
    public string Value { get; set; }
}

/// <summary>
/// ImportedLibraryParameters.
/// </summary>
public class ImportedLibraryParameters
{
    /// <summary>
    /// Value.
    /// </summary>
    /// <example>bar</example>
    public string Value { get; set; }
}

/// <summary>
/// AccessParameters.
/// </summary>
public class AccessParameters
{
    /// <summary>
    /// Role.
    /// </summary>
    /// <example>READER</example>
    public string Role { get; set; }

    /// <summary>
    /// User's email.
    /// </summary>
    /// <example>foo@bar.com</example>
    public string UserByEmail { get; set; }
}

/// <summary>
/// UserDefinedContext.
/// </summary>
public class UserDefinedContext
{
    /// <summary>
    /// Key.
    /// </summary>
    /// <example>foo</example>
    public string Key { get; set; }

    /// <summary>
    /// Value.
    /// </summary>
    /// <example>bar</example>
    public string Value { get; set; }
}

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