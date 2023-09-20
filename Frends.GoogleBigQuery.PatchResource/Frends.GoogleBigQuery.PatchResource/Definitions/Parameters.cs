namespace Frends.GoogleBigQuery.PatchResource.Definitions;

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