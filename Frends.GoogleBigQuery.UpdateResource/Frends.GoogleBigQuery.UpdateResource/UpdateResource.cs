﻿using Frends.GoogleBigQuery.UpdateResource.Definitions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Bigquery.v2;
using Google.Apis.Bigquery.v2.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.GoogleBigQuery.UpdateResource;

/// <summary>
/// Google BigQuery Task.
/// </summary>
public class GoogleBigQuery
{
    /// <summary>
    /// Updates information in an existing resource. 
    /// The update method replaces the entire resource, whereas the patch method only replaces fields that are provided in the submitted resource.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.GoogleBigQuery.UpdateResource)
    /// </summary>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="input">Input parameters.</param>
    /// <param name="options">Optional parameters.</param>
    /// <param name="cancellationToken">Token generated by Frends to stop this Task.</param>
    /// <returns>Object { bool Success, string ErrorMessage }</returns>
    public static async Task<Result> UpdateResource([PropertyTab] Connection connection, [PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
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
                return new Result(await UpdateRequest(new BigqueryService(serviceInitializer), connection, input, cancellationToken), null);
            else
            {
                if (options.ThrowOnError)
                    throw new Exception("Failed to initialize BigqueryService.");

                return new Result(false, "Failed to initialize BigqueryService.");
            }
        }
        catch (Exception ex)
        {
            if (options.ThrowOnError)
                throw new Exception($"Error occured: {ex.Message}");

            return new Result(false, $"Error occured: {ex.Message}");
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

    private static async Task<bool> UpdateRequest(BigqueryService service, Connection connection, Input input, CancellationToken cancellationToken)
    {
        Dictionary<string, string> labels = new();
        switch (connection.Resource)
        {
            case Resources.Dataset:
                List<Dataset.AccessData> accessList = new();
                List<Dataset.TagsData> tags = new();

                if (input.Access is not null)
                    foreach (var param in input.Access)
                        accessList.Add(new()
                        {
                            Role = param.Role,
                            UserByEmail = param.UserByEmail
                        });

                if (input.Label is not null)
                    foreach (var param in input.Label)
                        labels.Add(param.Key, param.Value);

                if (input.Tag is not null)
                    foreach (var param in input.Tag)
                        tags.Add(new Dataset.TagsData { TagKey = param.Key, TagValue = param.Value });

                Dataset dataset = new()
                {
                    Access = accessList is null ? null : accessList,
                    DefaultEncryptionConfiguration = !input.SetEncryptionConfiguration ? null : new EncryptionConfiguration
                    {
                        KmsKeyName = string.IsNullOrEmpty(input.KmsKeyName) ? null : input.KmsKeyName,
                    },
                    Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description,
                    FriendlyName = string.IsNullOrWhiteSpace(input.FriendlyName) ? null : input.FriendlyName,
                    Labels = labels is null ? null : labels,
                    Location = input.Location,
                    StorageBillingModel = input.StorageBillingModel,
                    Tags = tags is null ? null : tags,
                    DatasetReference = new DatasetReference { DatasetId = connection.DatasetId },
                    IsCaseInsensitive = input.IsCaseInsensitive,
                    MaxTimeTravelHours = input.MaxTimeTravelHours > 0 ? input.MaxTimeTravelHours : null,
                };
                await service.Datasets.Update(dataset, connection.ProjectId, connection.DatasetId).ExecuteAsync(cancellationToken);
                return true;
            case Resources.Routine:
                List<Argument> args = new();
                Dictionary<string, string> userDefinedContext = new();
                List<string> importedLibraries = new();

                if (input.Argument is not null)
                    foreach (var param in input.Argument)
                        args.Add(new Argument()
                        {
                            Name = string.IsNullOrWhiteSpace(param.Name) ? null : param.Name,
                            Mode = string.IsNullOrWhiteSpace(param.Mode) ? null : param.Mode,
                            DataType = new StandardSqlDataType { TypeKind = param.DataType.ToUpper() },
                            ArgumentKind = param.ArgumentKind.ToString().ToUpper(),
                        });

                if (input.RemoteUserDefinedContext is not null)
                    foreach (var param in input.RemoteUserDefinedContext)
                        userDefinedContext.Add(param.Key, param.Value);

                if (input.ImportedLibrary is not null)
                    foreach (var param in input.ImportedLibrary)
                        importedLibraries.Add(param.Value);

                Routine routine = new()
                {
                    Arguments = args,
                    DefinitionBody = input.DefinitionBody,
                    Language = input.Language,
                    ImportedLibraries = importedLibraries is null ? null : importedLibraries,
                    Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description,
                    DeterminismLevel = input.DeterminismLevel.ToString(),
                    RoutineType = input.RoutineType.ToString(),
                    StrictMode = input.StrictMode,
                    ReturnType = new StandardSqlDataType { TypeKind = input.ReturnType.ToUpper() },
                    RoutineReference = new()
                    {
                        DatasetId = connection.DatasetId,
                        ProjectId = connection.ProjectId,
                        RoutineId = connection.RoutineId,
                    },
                };

                if (input.SetRemoteFunctionParameters)
                    routine.RemoteFunctionOptions = new()
                    {
                        Connection = input.RemoteConnection,
                        UserDefinedContext = userDefinedContext is null ? null : userDefinedContext,
                        Endpoint = input.RemoteEndpoint,
                        MaxBatchingRows = input.RemoteMaxBatchingRows,
                    };

                await service.Routines.Update(routine, connection.ProjectId, connection.DatasetId, connection.RoutineId).ExecuteAsync(cancellationToken);
                return true;
            case Resources.Table:
                TableSchema tableSchema = new();
                List<TableFieldSchema> tableFieldSchema = new();

                if (input.TableSchema is not null)
                {
                    foreach (var param in input.TableSchema)
                        tableFieldSchema.Add(
                            new TableFieldSchema
                            {
                                Name = param.Name,
                                Type = param.Type.ToUpper(),
                            });

                    tableSchema.Fields = tableFieldSchema;
                }

                if (input.Label is not null)
                    foreach (var param in input.Label)
                        labels.Add(param.Key, param.Value);

                Table table = new()
                {
                    TableReference = new TableReference
                    {
                        ProjectId = connection.ProjectId,
                        DatasetId = connection.DatasetId,
                        TableId = connection.TableId
                    },
                    EncryptionConfiguration = !input.SetEncryptionConfiguration ? null : new EncryptionConfiguration
                    {
                        KmsKeyName = input.KmsKeyName
                    },
                    RequirePartitionFilter = input.RequirePartitionFilter,
                    Schema = tableSchema is null ? null : tableSchema,
                    Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description,
                    FriendlyName = string.IsNullOrWhiteSpace(input.FriendlyName) ? null : input.FriendlyName,
                    Labels = labels is null ? null : labels,
                    Location = string.IsNullOrWhiteSpace(input.Location) ? null : input.Location,
                };

                if (input.Partition is TablePartitionOptions.RangePartitioning)
                    table.RangePartitioning = new RangePartitioning()
                    {
                        Field = input.RangeField.ToUpper(),
                        Range = new RangePartitioning.RangeData()
                        {
                            Start = input.RangeStart,
                            End = input.RangeEnd,
                            Interval = input.RangeInterval,
                        }
                    };
                else
                    table.TimePartitioning = new TimePartitioning()
                    {
                        RequirePartitionFilter = input.TimeRequirePartitionFilter,
                        ExpirationMs = input.ExpirationMs,
                        Field = input.TimeField,
                        Type = input.Type.ToString()
                    };

                await service.Tables.Update(table, connection.ProjectId, connection.DatasetId, connection.TableId).ExecuteAsync(cancellationToken);
                return true;
            default:
                throw new Exception("Resource not supported.");
        }
    }
}