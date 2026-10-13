namespace Frends.GoogleBigQuery.CreateResource.Definitions;

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
