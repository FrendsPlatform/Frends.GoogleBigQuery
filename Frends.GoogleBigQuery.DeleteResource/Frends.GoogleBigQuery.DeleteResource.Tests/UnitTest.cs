using System.Text;
using Frends.GoogleBigQuery.DeleteResource.Definitions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Bigquery.v2;
using Google.Apis.Bigquery.v2.Data;
using Google.Apis.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.GoogleBigQuery.DeleteResource.Tests;

[TestClass]
public class UnitTest
{
    private readonly string? secretJson = Environment.GetEnvironmentVariable("BigQuery_SecretJson");
    private Connection connection = new();
    private Input input = new();
    private Options options = new();

    [TestInitialize]
    public void Init()
    {
        var datasetId = $"tasktest_dataset_{DateTime.Now:yyyy_MM_dd_hh_mm_ss}";
        var tableId = $"tasktest_tableid_{DateTime.Now:yyyy_MM_dd_hh_mm_ss}";
        var routineId = $"tasktest_routineid_{DateTime.Now:yyyy_MM_dd_hh_mm_ss}";
        string json;

        // This one is to handle CI secret problems.
        if (File.Exists(secretJson))
        {
            json = File.ReadAllText(secretJson);
        }
        else
        {
            var encodedJson = !string.IsNullOrEmpty(secretJson) ? secretJson : string.Empty;
            byte[] bytes = Convert.FromBase64String(encodedJson);
            json = Encoding.UTF8.GetString(bytes);
        }

        connection = new()
        {
            ReadJsonMethod = ReadJsonMethods.JSON,
            SecretJson = json,
            ProjectId = "instant-stone-387712",
            BaseUri = null,
            CredentialsFilePath = null,
        };

        input = new()
        {
            Resource = Resources.Dataset,
            DatasetId = datasetId,
            TableId = tableId,
            RoutineId = routineId,
            ModelId = null,
            JobId = null,
        };

        options = new()
        {
            ThrowErrorOnFailure = true,
        };
    }

    [TestMethod]
    public async Task Delete_Dataset()
    {
        await CreateDatasetResource(connection.ProjectId, input.DatasetId, connection.SecretJson);
        var result = await GoogleBigQuery.DeleteResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Delete_Routine()
    {
        input.Resource = Resources.Routine;
        await CreateDatasetResource(connection.ProjectId, input.DatasetId, connection.SecretJson);
        await CreateRoutineResource(connection.ProjectId, input.DatasetId, input.RoutineId, connection.SecretJson);
        var result = await GoogleBigQuery.DeleteResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);

        input.Resource = Resources.Dataset;
        await GoogleBigQuery.DeleteResource(input, connection, options, default);
    }

    [TestMethod]
    public async Task Delete_Table()
    {
        input.Resource = Resources.Table;
        await CreateDatasetResource(connection.ProjectId, input.DatasetId, connection.SecretJson);
        await CreateTableResource(connection.ProjectId, input.DatasetId, input.TableId, connection.SecretJson);
        var result = await GoogleBigQuery.DeleteResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);

        input.Resource = Resources.Dataset;
        await GoogleBigQuery.DeleteResource(input, connection, options, default);
    }

    [TestMethod]
    public async Task Delete_Invalid_SecretJson_Throw()
    {
        connection.SecretJson = @"{ 
""Foo"": ""Bar""
}";
        var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => GoogleBigQuery.DeleteResource(input, connection, options, default));
        Assert.IsNotNull(ex);
        Assert.AreEqual("Error creating credential from JSON or JSON parameters. Unrecognized credential type .", ex.Message);
    }

    [TestMethod]
    public async Task Delete_Invalid_SecretJson_ReturnError()
    {
        connection.SecretJson = @"{ 
""Foo"": ""Bar""
}";

        options.ThrowErrorOnFailure = false;

        var result = await GoogleBigQuery.DeleteResource(input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.AreEqual("Error creating credential from JSON or JSON parameters. Unrecognized credential type .", result.Error.Message);
    }

    [TestMethod]
    public async Task Delete_Success_BaseUri()
    {
        await CreateDatasetResource(connection.ProjectId, input.DatasetId, connection.SecretJson);
        connection.BaseUri = "https://bigquery.googleapis.com/bigquery/v2/";
        var result = await GoogleBigQuery.DeleteResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task ErrorHandler_Uses_Custom_ErrorMessageOnFailure()
    {
        connection.SecretJson = @"{ ""Foo"": ""Bar"" }";
        options.ThrowErrorOnFailure = true;
        options.ErrorMessageOnFailure = "CustomErrorMessage";

        var ex = await Assert.ThrowsExceptionAsync<Exception>(() => GoogleBigQuery.DeleteResource(input, connection, options, default));
        StringAssert.Contains(ex.Message, "CustomErrorMessage");
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
            },
        };
        var bigqueryService = GetBigqueryService(secretJson);
        await bigqueryService.Routines.Insert(routine, projectId, datasetId).ExecuteAsync();
    }

    private static async Task CreateTableResource(string projectId, string datasetId, string tableId, string secretJson)
    {
        List<TableFieldSchema> tableFieldSchema = new() { new TableFieldSchema() { Name = "AGE", Type = "INTEGER" } };
        TableSchema tableSchema = new() { Fields = tableFieldSchema };
        Table table = new()
        {
            TableReference = new TableReference
            {
                ProjectId = projectId,
                DatasetId = datasetId,
                TableId = tableId,
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
                },
            },
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
            ApplicationName = "BigQueryApp",
        };
        return new BigqueryService(serviceInitializer);
    }
}
