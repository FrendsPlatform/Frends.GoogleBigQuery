﻿using Frends.GoogleBigQuery.PatchResource.Definitions;
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

namespace Frends.GoogleBigQuery.PatchResource;

/// <summary>
/// Google BigQuery Task.
/// </summary>
public class GoogleBigQuery
{
    /// <summary>
    /// Updates information in an existing resource. Patch method only replaces fields that are provided in the submitted resource.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.GoogleBigQuery.PatchResource)
    /// </summary>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="input">Input parameters.</param>
    /// <param name="options">Optional parameters.</param>
    /// <param name="cancellationToken">Token generated by Frends to stop this Task.</param>
    /// <returns>Object { bool Success, string ErrorMessage }</returns>
    public static async Task<Result> PatchResource([PropertyTab] Connection connection, [PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
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
        switch (input.Resource)
        {
            case Resources.Dataset:
                List<Dataset.AccessData> accessList = new();
                List<Dataset.TagsData> tags = new();
                Dataset dataset = new();

                if (input.Access is not null)
                {
                    foreach (var param in input.Access)
                        accessList.Add(new()
                        {
                            Role = param.Role,
                            UserByEmail = param.UserByEmail
                        });
                    dataset.Access = accessList;
                }

                if (input.Label is not null)
                {
                    foreach (var param in input.Label)
                        labels.Add(param.Key, param.Value);
                    dataset.Labels = labels;
                }

                if (input.Tag is not null)
                {
                    foreach (var param in input.Tag)
                        tags.Add(new Dataset.TagsData { TagKey = param.Key, TagValue = param.Value });
                    dataset.Tags = tags;
                }

                if (input.SetEncryptionConfiguration)
                    dataset.DefaultEncryptionConfiguration = new EncryptionConfiguration { KmsKeyName = input.KmsKeyName };

                if (!string.IsNullOrWhiteSpace(input.Description))
                    dataset.Description = input.Description;

                if (!string.IsNullOrWhiteSpace(input.FriendlyName))
                    dataset.FriendlyName = input.FriendlyName;

                if (!string.IsNullOrWhiteSpace(input.FriendlyName))
                    dataset.FriendlyName = input.FriendlyName;

                if (!string.IsNullOrWhiteSpace(input.Location))
                    dataset.Location = input.Location;

                if (!string.IsNullOrWhiteSpace(input.StorageBillingModel))
                    dataset.StorageBillingModel = input.StorageBillingModel;

                if (!string.IsNullOrWhiteSpace(input.StorageBillingModel))
                    dataset.StorageBillingModel = input.StorageBillingModel;

                dataset.IsCaseInsensitive = input.IsCaseInsensitive;
                dataset.DatasetReference = new DatasetReference { DatasetId = connection.DatasetId };
                dataset.MaxTimeTravelHours = input.MaxTimeTravelHours > 0 ? input.MaxTimeTravelHours : null;

                await service.Datasets.Patch(dataset, connection.ProjectId, connection.DatasetId).ExecuteAsync(cancellationToken);
                return true;
            case Resources.Table:
                Table table = new();
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
                    table.Schema = tableSchema;
                }

                if (input.Label is not null)
                {
                    foreach (var param in input.Label)
                        labels.Add(param.Key, param.Value);
                    table.Labels = labels;
                }

                table.TableReference = new TableReference
                {
                    ProjectId = connection.ProjectId,
                    DatasetId = connection.DatasetId,
                    TableId = connection.TableId
                };

                if (input.SetEncryptionConfiguration)
                    table.EncryptionConfiguration = !input.SetEncryptionConfiguration ? null : new EncryptionConfiguration
                    {
                        KmsKeyName = input.KmsKeyName
                    };

                if (input.RequirePartitionFilter)
                    table.RequirePartitionFilter = input.RequirePartitionFilter;

                if (!string.IsNullOrWhiteSpace(input.Description))
                    table.Description = input.Description;

                if (!string.IsNullOrWhiteSpace(input.FriendlyName))
                    table.FriendlyName = input.FriendlyName;

                if (!string.IsNullOrWhiteSpace(input.Location))
                    table.Location = input.Location;

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

                await service.Tables.Patch(table, connection.ProjectId, connection.DatasetId, connection.TableId).ExecuteAsync(cancellationToken);
                return true;
            default:
                throw new Exception("Resource not supported.");
        }
    }
}