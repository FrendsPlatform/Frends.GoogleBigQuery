﻿using Frends.GoogleBigQuery.ListResource.Definitions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Bigquery.v2;
using Google.Apis.Services;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.GoogleBigQuery.ListResource;

/// <summary>
/// Google BigQuery Task.
/// </summary>
public class GoogleBigQuery
{
    /// <summary>
    /// List resource.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.GoogleBigQuery.ListResource)
    /// </summary>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="options">Optional parameters.</param>
    /// <param name="cancellationToken">Token generated by Frends to stop this Task.</param>
    /// <returns>Object { bool Success, JToken Data, string ErrorMessage }</returns>
    public static async Task<Result> ListResource([PropertyTab] Connection connection, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        try
        {
            var serviceInitializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = await GetServiceAccountCredential(connection, cancellationToken),
                ApplicationName = "BigQueryApp",
                BaseUri = string.IsNullOrWhiteSpace(connection.BaseUri) ? null : connection.BaseUri,
            };
            var bigqueryService = new BigqueryService(serviceInitializer);

            if (bigqueryService is not null)
                return new Result(true, await ListRequest(new BigqueryService(serviceInitializer), connection, cancellationToken), null);
            else
            {
                if (options.ThrowOnError)
                    throw new Exception("Failed to initialize BigqueryService.");

                return new Result(false, null, "Failed to initialize BigqueryService.");
            }
        }
        catch (Exception ex)
        {
            if (options.ThrowOnError)
                throw new Exception($"Error occured: {ex.Message}");

            return new Result(false, null, $"Error occured: {ex.Message}");
        }
    }

    private static async Task<ServiceAccountCredential> GetServiceAccountCredential(Connection connection, CancellationToken cancellationToken)
    {
        var jsonCredentials = connection.ReadJsonMethod == ReadJsonMethods.File ? await File.ReadAllTextAsync(connection.CredentialsFilePath, cancellationToken) : connection.SecretJson;


        ServiceAccountCredential credential;
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonCredentials));
        credential = GoogleCredential.FromStream(stream)
                                     .CreateScoped(BigqueryService.Scope.Bigquery)
                                     .UnderlyingCredential as ServiceAccountCredential;

        return credential;
    }

    private static async Task<JToken> ListRequest(BigqueryService service, Connection connection, CancellationToken cancellationToken)
    {
        return connection.Resource switch
        {
            Resources.Datasets => JToken.FromObject(await service.Datasets.List(connection.ProjectId).ExecuteAsync(cancellationToken)),
            Resources.Jobs => JToken.FromObject(await service.Jobs.List(connection.ProjectId).ExecuteAsync(cancellationToken)),
            Resources.Models => JToken.FromObject(await service.Models.List(connection.ProjectId, connection.DatasetId).ExecuteAsync(cancellationToken)),
            Resources.Projects => JToken.FromObject(await service.Projects.List().ExecuteAsync(cancellationToken)),
            Resources.Routines => JToken.FromObject(await service.Routines.List(connection.ProjectId, connection.DatasetId).ExecuteAsync(cancellationToken)),
            Resources.RowAccessPolicies => JToken.FromObject(await service.RowAccessPolicies.List(connection.ProjectId, connection.DatasetId, connection.TableId).ExecuteAsync(cancellationToken)),
            Resources.TableData => JToken.FromObject(await service.Tabledata.List(connection.ProjectId, connection.DatasetId, connection.TableId).ExecuteAsync(cancellationToken)),
            Resources.Tables => JToken.FromObject(await service.Tables.List(connection.ProjectId, connection.DatasetId).ExecuteAsync(cancellationToken)),
            _ => throw new Exception("Resource not supported."),
        };
    }
}