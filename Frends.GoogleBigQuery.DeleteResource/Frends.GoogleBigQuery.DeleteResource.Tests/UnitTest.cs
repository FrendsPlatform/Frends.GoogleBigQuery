using Frends.GoogleBigQuery.DeleteResource.Definitions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Bigquery.v2;
using Google.Apis.Bigquery.v2.Data;
using Google.Apis.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Frends.GoogleBigQuery.DeleteResource.Tests;

[TestClass]
public class UnitTest
{
    private Connection _connection = new();
    private Options _options = new();
    private readonly string? _secretJson = Environment.GetEnvironmentVariable("BigQuery_SecretJson");

    [TestInitialize]
    public void Init()
    {
        var _datasetId = $"tasktest_dataset_{DateTime.Now:yyyy_MM_dd_hh_mm_ss}";
        var _tableId = $"tasktest_tableid_{DateTime.Now:yyyy_MM_dd_hh_mm_ss}";
        var _routineId = $"tasktest_routineid_{DateTime.Now:yyyy_MM_dd_hh_mm_ss}";
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
            Resource = Resources.Dataset,
            SecretJson = json,
            ProjectId = "instant-stone-387712",
            DatasetId = _datasetId,
            TableId = _tableId,
            RoutineId = _routineId,
            ModelId = null,
            JobId = null,
            BaseUri = null,
            CredentialsFilePath = null,
        };

        _options = new()
        {
            ThrowOnError = true,
        };
    }

    [TestMethod]
    public async Task Delete_Dataset()
    {
        await CreateDatasetResource(_connection.ProjectId, _connection.DatasetId, _connection.SecretJson);
        var result = await GoogleBigQuery.DeleteResource(_connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Delete_Routine()
    {
        _connection.Resource = Resources.Routine;
        await CreateDatasetResource(_connection.ProjectId, _connection.DatasetId, _connection.SecretJson);
        await CreateRoutineResource(_connection.ProjectId, _connection.DatasetId, _connection.RoutineId, _connection.SecretJson);
        var result = await GoogleBigQuery.DeleteResource(_connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorMessage);

        _connection.Resource = Resources.Dataset;
        await GoogleBigQuery.DeleteResource(_connection, _options, default);
    }

    [TestMethod]
    public async Task Delete_Table()
    {
        _connection.Resource = Resources.Table;
        await CreateDatasetResource(_connection.ProjectId, _connection.DatasetId, _connection.SecretJson);
        await CreateTableResource(_connection.ProjectId, _connection.DatasetId, _connection.TableId, _connection.SecretJson);
        var result = await GoogleBigQuery.DeleteResource(_connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorMessage);

        _connection.Resource = Resources.Dataset;
        await GoogleBigQuery.DeleteResource(_connection, _options, default);
    }

    [TestMethod]
    public async Task Delete_Invalid_SecretJson_Throw()
    {
        _connection.SecretJson = @"{ 
""Foo"": ""Bar""
}";
        var ex = await Assert.ThrowsExceptionAsync<Exception>(() => GoogleBigQuery.DeleteResource(_connection, _options, default));
        Assert.IsNotNull(ex);
        Assert.AreEqual("Error occured: Error creating credential from JSON or JSON parameters. Unrecognized credential type .", ex.Message);
    }

    [TestMethod]
    public async Task Delete_Invalid_SecretJson_ReturnError()
    {
        _connection.SecretJson = @"{ 
""Foo"": ""Bar""
}";

        _options.ThrowOnError = false;

        var result = await GoogleBigQuery.DeleteResource(_connection, _options, default);
        Assert.IsFalse(result.Success);
        Assert.AreEqual("Error occured: Error creating credential from JSON or JSON parameters. Unrecognized credential type .", result.ErrorMessage);
    }

    [TestMethod]
    public async Task Delete_Success_BaseUri()
    {
        await CreateDatasetResource(_connection.ProjectId, _connection.DatasetId, _connection.SecretJson);
        _connection.BaseUri = "https://bigquery.googleapis.com/bigquery/v2/";
        var result = await GoogleBigQuery.DeleteResource(_connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorMessage);
    }

    private static async Task CreateDatasetResource(string projectId, string datasetId, string secretJson)
    {
        Dataset dataset = new()
        {
            FriendlyName = datasetId,
            Location = "EU",
            StorageBillingModel = "LOGICAL",
            DatasetReference = new DatasetReference { DatasetId = datasetId },
        };
        var bigqueryService = GetBigqueryService(secretJson);
        await bigqueryService.Datasets.Insert(dataset, projectId).ExecuteAsync();
    }

    private static async Task CreateRoutineResource(string projectId, string datasetId, string routineId, string secretJson)
    {
        Routine routine = new()
        {
            Arguments = new[] { new Argument() { DataType = new StandardSqlDataType { TypeKind = "INT64" }, Name = "x", ArgumentKind = "FIXED_TYPE" } },
            DefinitionBody = "(x * 3)",
            Description = routineId,
            RoutineType = "SCALAR_FUNCTION",
            RoutineReference = new()
            {
                DatasetId = datasetId,
                ProjectId = projectId,
                RoutineId = routineId,
            }
        };
        var bigqueryService = GetBigqueryService(secretJson);
        await bigqueryService.Routines.Insert(routine, projectId, datasetId).ExecuteAsync();
    }

    private static async Task CreateTableResource(string projectId, string datasetId, string TableId, string secretJson)
    {
        List<TableFieldSchema> tableFieldSchema = new() { new TableFieldSchema() { Name = "AGE", Type = "INTEGER" } };
        TableSchema tableSchema = new() { Fields = tableFieldSchema };
        Table table = new()
        {
            TableReference = new TableReference
            {
                ProjectId = projectId,
                DatasetId = datasetId,
                TableId = TableId
            },
            Schema = tableSchema,
            RangePartitioning = new RangePartitioning()
            {
                Field = "AGE",
                Range = new RangePartitioning.RangeData()
                {
                    Start = 1,
                    End = 10,
                    Interval = 10,
                }
            }
        };
        var bigqueryService = GetBigqueryService(secretJson);
        await bigqueryService.Tables.Insert(table, projectId, datasetId).ExecuteAsync();
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
}