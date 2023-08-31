namespace Frends.GoogleBigQuery.List.Definitions;

/// <summary>
/// Resource.
/// </summary>
public enum Resource
{
    Datasets,
    Jobs,
    Models,
    Projects,
    Routines,
    RowAccessPolicies,
    TableData,
    Tables
}

public enum AuthenticationMethods
{
    ServiceAccount,
    UserAccount
}

/// <summary>
/// OAuthScopes.
/// </summary>
public enum OAuthScopes
{
    BigQuery,
    CloudPlatform,
    BigQueryReadonly,
    CloudPlatformReadOnly
}

/// <summary>
/// Method to read JSON credentials.
/// </summary>
public enum ReadJsonMethods
{
    JSON,
    File,
}
