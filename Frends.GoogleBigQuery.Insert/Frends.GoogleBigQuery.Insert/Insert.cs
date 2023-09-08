﻿using Frends.GoogleBigQuery.Insert.Definitions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Bigquery.v2;
using Google.Apis.Bigquery.v2.Data;
using Google.Apis.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.GoogleBigQuery.Insert;

/// <summary>
/// Google BigQuery Task.
/// </summary>
public class GoogleBigQuery
{
    /// <summary>
    /// Insert to BigQuery table.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.GoogleBigQuery.Insert)
    /// </summary>
    /// <param name="input">Input parameters.</param>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="options">Optional parameters.</param>
    /// <param name="cancellationToken">Token generated by Frends to stop this Task.</param>
    /// <returns>Object { bool Success, string ErrorMessage }</returns>
    public static async Task<Result> Insert([PropertyTab] Input input, [PropertyTab] Connection connection, [PropertyTab] Options options, CancellationToken cancellationToken)
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
                return new Result(await InsertStream(new BigqueryService(serviceInitializer), connection, input, cancellationToken), null);
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

    private static async Task<bool> InsertStream(BigqueryService service, Connection connection, Input input, CancellationToken cancellationToken)
    {
        TableDataInsertAllRequest request;
        switch (input.SourceFormat)
        {
            case SourceFormats.CSV:
                var csvData = input.Csv;
                var csvRows = csvData.Split('\n').Select(line => line.Trim()).ToList();

                var headerRow = csvRows.Skip(input.SkipTopRows).FirstOrDefault() ?? throw new Exception("Invalid header row");
                var headers = headerRow.Split(input.Delimiter);
                var csvRowsData = new List<TableDataInsertAllRequest.RowsData>();

                foreach (var line in csvRows.Skip(input.SkipTopRows + 1))
                {
                    var rowData = new TableDataInsertAllRequest.RowsData { Json = new Dictionary<string, object>() };
                    var values = line.Split(input.Delimiter);

                    if (values.Length != headers.Length)
                        throw new Exception("Number of values doesn't match the number of headers.");
                    
                    for (var i = 0; i < headers.Length; i++)
                        rowData.Json[headers[i]] = values[i];

                    csvRowsData.Add(rowData);
                }

                request = new TableDataInsertAllRequest { Rows = csvRowsData };
                var result = await service.Tabledata.InsertAll(request, connection.ProjectId, connection.DatasetId, connection.TableId).ExecuteAsync(cancellationToken);

                if (result.InsertErrors != null)
                    throw new Exception(result.InsertErrors.FirstOrDefault().Errors.First().Message);

                return true;
            case SourceFormats.JSON:
                var jsonRows = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(input.Json);
                var jsonRowsData = jsonRows.Select(row => new TableDataInsertAllRequest.RowsData { Json = row }).ToList();
                request = new TableDataInsertAllRequest { Rows = jsonRowsData };
                await service.Tabledata.InsertAll(request, connection.ProjectId, connection.DatasetId, connection.TableId).ExecuteAsync(cancellationToken);
                return true;
            case SourceFormats.RowData:
                var rows = new List<TableDataInsertAllRequest.RowsData>();

                foreach (var param in input.RowData)
                {
                    var rowData = new TableDataInsertAllRequest.RowsData { Json = new Dictionary<string, object>() };

                    foreach (var data in param.Row)
                        rowData.Json[data.ColumnName] = data.Value;

                    rows.Add(rowData);
                }

                request = new TableDataInsertAllRequest { Rows = rows };
                await service.Tabledata.InsertAll(request, connection.ProjectId, connection.DatasetId, connection.TableId).ExecuteAsync(cancellationToken);
                return true;
            default:
                return false;
        }
    }
}