using Frends.GoogleBigQuery.ExecuteQuery.Definitions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Bigquery.v2;
using Google.Apis.Bigquery.v2.Data;
using Google.Apis.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Text;

namespace Frends.GoogleBigQuery.ExecuteQuery.Tests;

[TestClass]
public class UnitTests
{
    private Input _input = new();
    private Connection _connection = new();
    private Options _options = new();
    private readonly string? _secretJson = Environment.GetEnvironmentVariable("BigQuery_SecretJson");
    private readonly string _projectId = "instant-stone-387712";

    [TestInitialize]
    public async Task Init()
    {
        var _datasetId = $"tasktest_dataset_{DateTime.Now:yyyy_MM_dd_hh_mm_ss}";
        var _tableId = $"tasktest_tableid_{DateTime.Now:yyyy_MM_dd_hh_mm_ss}";
        string json;

        // This one is to handle CI secret problems.
        if (File.Exists(_secretJson))
            json = File.ReadAllText(_secretJson);
        else
        {
            var _json = !string.IsNullOrEmpty(_secretJson) ? _secretJson : "";
            byte[] bytes = Convert.FromBase64String(_json);
            json = Encoding.UTF8.GetString(bytes);
        }

        _connection = new()
        {
            ReadJsonMethod = ReadJsonMethods.JSON,
            SecretJson = json,
            ProjectId = _projectId,
            DatasetId = _datasetId,
            TableId = _tableId,
            CredentialsFilePath = null,
        };

        _input = new()
        {
            BigQueryParameter = default,
            Query = $"SELECT * FROM {_connection.DatasetId}.{_connection.TableId}"
        };

        _options = new()
        {
            ThrowOnError = true,
            MaxRetryAttempts = 5,
            Delay = 5
        };

        await CreateData(_connection.ProjectId, _connection.DatasetId, _connection.TableId, _connection.SecretJson);
    }

    [TestCleanup]
    public void Cleanup()
    {
        DeleteDataset(_connection.ProjectId, _connection.DatasetId, _connection.TableId, _connection.SecretJson);
    }

    [TestMethod]
    public async Task Update_SELECT()
    {
        var result = await GoogleBigQuery.ExecuteQuery(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Update_SELECT_NotFound()
    {
        _input.Query = "SELECT * FROM tasktest.testtable";
        var result = await GoogleBigQuery.ExecuteQuery(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Update_SELECT_Parameter()
    {
        var param = new[] { new BigQueryParameters() { Name = "ParameterName", BigQueryDbType = BigQueryDbTypes.String, Value = "Sample String" } };
        _input.Query = $"SELECT * FROM {_connection.DatasetId}.{_connection.TableId} WHERE string_field = @ParameterName";
        _input.BigQueryParameter = param;

        var result = await GoogleBigQuery.ExecuteQuery(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Update_INSERT()
    {
        _input.Query = @$"INSERT INTO {_connection.DatasetId}.{_connection.TableId} (integer_field, string_field, date_field, float_field, boolean_field, timestamp_field, time_field, datetime_field, record_field, array_field, struct_field) VALUES (3, 'New Sample', DATE '2023-09-22', 4.56, TRUE, TIMESTAMP '2023-09-22T14:45:00Z', TIME '14:45:00.123', DATETIME '2023-09-22 14:45:00.123000', STRUCT('Nested New', 55), ['Item 5', 'Item 6'], STRUCT('Jane Doe', 40));";

        var result = await GoogleBigQuery.ExecuteQuery(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Update_UPDATE()
    {
        // Have to use old table here to skip long wait
        _input.Query = $@"UPDATE tasktest.testtable SET Name = 'FooBar' WHERE Name = 'Foo';";

        var result = await GoogleBigQuery.ExecuteQuery(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Update_Delete()
    {
        // Have to use old table here to skip long wait
        _input.Query = $@"DELETE FROM tasktest.testtable WHERE Country = 'USA';";

        var result = await GoogleBigQuery.ExecuteQuery(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Update_Invalid_SecretJson_Throw()
    {
        _connection.SecretJson = @"{ 
""Foo"": ""Bar""
}";
        var ex = await Assert.ThrowsExceptionAsync<Exception>(() => GoogleBigQuery.ExecuteQuery(_connection, _input, _options, default));
        Assert.IsNotNull(ex);
        Assert.AreEqual("Error creating credential from JSON or JSON parameters. Unrecognized credential type .", ex.Message);
    }

    [TestMethod]
    public async Task Update_Invalid_SecretJson_ReturnError()
    {
        _connection.SecretJson = @"{ 
""Foo"": ""Bar""
}";

        _options.ThrowOnError = false;

        var result = await GoogleBigQuery.ExecuteQuery(_connection, _input, _options, default);
        Assert.IsFalse(result.Success);
        Assert.AreEqual("Error creating credential from JSON or JSON parameters. Unrecognized credential type .", result.ErrorMessage);
    }

    private static async Task CreateData(string projectId, string datasetId, string tableId, string secretJson)
    {
        try
        {
            var bigqueryService = GetBigqueryService(secretJson);

            // Dataset
            Dataset dataset = new()
            {
                FriendlyName = datasetId,
                Location = "EU",
                StorageBillingModel = "LOGICAL",
                DatasetReference = new DatasetReference { DatasetId = datasetId },
            };

            await bigqueryService.Datasets.Insert(dataset, projectId).ExecuteAsync();

            TableSchema schema = new()
            {
                Fields = new List<TableFieldSchema>
                {
                    new TableFieldSchema { Name = "integer_field", Type = "INTEGER" },
                    new TableFieldSchema { Name = "string_field", Type = "STRING" },
                    new TableFieldSchema { Name = "date_field", Type = "DATE" },
                    new TableFieldSchema { Name = "float_field", Type = "FLOAT" },
                    new TableFieldSchema { Name = "boolean_field", Type = "BOOLEAN" },
                    new TableFieldSchema { Name = "timestamp_field", Type = "TIMESTAMP" },
                    new TableFieldSchema { Name = "time_field", Type = "TIME" },
                    new TableFieldSchema { Name = "datetime_field", Type = "DATETIME" },
                    new TableFieldSchema
                    {
                        Name = "record_field",
                        Type = "RECORD",
                        Fields = new List<TableFieldSchema>
                        {
                            new TableFieldSchema { Name = "nested_string", Type = "STRING" },
                            new TableFieldSchema { Name = "nested_integer", Type = "INTEGER" }
                        }
                    },
                    new TableFieldSchema { Name = "array_field", Type = "STRING", Mode = "REPEATED" },
                    new TableFieldSchema
                    {
                        Name = "struct_field",
                        Type = "STRUCT",
                        Fields = new List<TableFieldSchema>
                        {
                            new TableFieldSchema { Name = "name", Type = "STRING" },
                            new TableFieldSchema { Name = "age", Type = "INTEGER" }
                        }
                    }
                }
            };

            // Table
            Table table = new()
            {
                TableReference = new TableReference
                {
                    ProjectId = projectId,
                    DatasetId = datasetId,
                    TableId = tableId
                },
                RequirePartitionFilter = false,
                Description = "Table with various data types",
                Schema = schema,
                RangePartitioning = new RangePartitioning()
                {
                    Field = "integer_field",
                    Range = new RangePartitioning.RangeData()
                    {
                        Start = 1,
                        End = 2,
                        Interval = 3,
                    }
                },
            };

            await bigqueryService.Tables.Insert(table, projectId, datasetId).ExecuteAsync();

            // Insert test data
            var jsonData = @"[
            {
                ""integer_field"": 1,
                ""string_field"": ""Sample String"",
                ""date_field"": ""2023-09-20"",
                ""float_field"": 3.14,
                ""boolean_field"": true,
                ""timestamp_field"": ""2023-09-20T12:34:56Z"",
                ""time_field"": ""12:34:56.789"",
                ""datetime_field"": ""2023-09-20 12:34:56.789000"",
                ""record_field"": { ""nested_string"": ""Nested String"", ""nested_integer"": 42 },
                ""array_field"": [""Item 1"", ""Item 2""],
                ""struct_field"": { ""name"": ""John Smith"", ""age"": 30 }
            },
            {
                ""integer_field"": 2,
                ""string_field"": ""Another String"",
                ""date_field"": ""2023-09-21"",
                ""float_field"": 2.718,
                ""boolean_field"": false,
                ""timestamp_field"": ""2023-09-21T10:00:00Z"",
                ""time_field"": ""10:00:00.123"",
                ""datetime_field"": ""2023-09-20 12:34:56.789000"",
                ""record_field"": { ""nested_string"": ""Nested Value"", ""nested_integer"": 99 },
                ""array_field"": [""Item 3"", ""Item 4""],
                ""struct_field"": { ""name"": ""Alice Johnson"", ""age"": 25 }
            }
        ]";

            var jsonRows = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonData);
            var jsonRowsData = jsonRows?.Select(row => new TableDataInsertAllRequest.RowsData { Json = row }).ToList();
            var request = new TableDataInsertAllRequest { Rows = jsonRowsData };
            await bigqueryService.Tabledata.InsertAll(request, projectId, datasetId, tableId).ExecuteAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private static BigqueryService GetBigqueryService(string secretJson)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(secretJson));
        ServiceAccountCredential? credential = GoogleCredential.FromStream(stream)
                                         .CreateScoped(BigqueryService.Scope.Bigquery)
                                         .UnderlyingCredential as ServiceAccountCredential;

        var serviceInitializer = new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "BigQueryApp"
        };
        return new BigqueryService(serviceInitializer);
    }

    private static void DeleteDataset(string projectId, string datasetId, string tableId, string secretJson)
    {
        try
        {
            var bigqueryService = GetBigqueryService(secretJson);
            bigqueryService.Tables.Delete(projectId, datasetId, tableId);
            bigqueryService.Datasets.Delete(projectId, datasetId);
        }
        catch
        {
            //ignore exception
        }
    }
}