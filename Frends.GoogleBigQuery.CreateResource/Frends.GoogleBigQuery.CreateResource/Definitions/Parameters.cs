namespace Frends.GoogleBigQuery.CreateResource.Definitions;

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
    public string Name { get; set; }

    /// <summary>
    /// Type.
    /// </summary>
    /// <example>STRING</example>
    public string Type { get; set; }
}

/// <summary>
/// TableReferenceParameters.
/// </summary>
public class TableReferenceParameters
{
    /// <summary>
    /// Project ID.
    /// </summary>
    /// <example>global-env-397309</example>
    public string ProjectId { get; set; }

    /// <summary>
    /// Dataset ID.
    /// </summary>
    /// <example>bigquery-public-data.baseball</example>
    public string DatasetId { get; set; }

    /// <summary>
    /// Table ID.
    /// </summary>
    /// <example>bigquery-public-data.baseball.games_post_wide</example>
    public string TableId { get; set; }
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
/// RemoteFunctionParameters.
/// </summary>
public class RemoteFunctionParameters
{
    /// <summary>
    /// Fully qualified name of the user-provided connection object which holds the authentication information to send requests to the remote service.
    /// </summary>
    /// <example>projects/{projectId}/locations/{locationId}/connections/{connectionId}</example>
    public string Connection { get; set; }

    /// <summary>
    /// Endpoint of the user-provided remote service.
    /// </summary>
    /// <example>https://us-east1-my_gcf_project.cloudfunctions.net/remote_add</example>
    public string Endpoint { get; set; }

    /// <summary>
    /// Max number of rows in each batch sent to the remote service. 
    /// If 0, BigQuery dynamically decides the number of rows in a batch.
    /// </summary>
    /// <example>0</example>
    public long MaxBatchingRows { get; set; }

    /// <summary>
    /// User-defined context as a set of key/value pairs, which will be sent as function invocation context together with batched arguments in the requests to the remote service. 
    /// The total number of bytes of keys and values must be less than 8KB.
    /// </summary>
    /// <example>[ { foo, bar }, { foo2, bar2 } ]</example>
    public UserDefinedContext[] UserDefinedContext { get; set; }
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