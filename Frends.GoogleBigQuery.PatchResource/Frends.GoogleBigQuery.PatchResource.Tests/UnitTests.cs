using System.Text;
using Frends.GoogleBigQuery.PatchResource.Definitions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Bigquery.v2;
using Google.Apis.Bigquery.v2.Data;
using Google.Apis.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.GoogleBigQuery.PatchResource.Tests;

[TestClass]
public class UnitTests
{
    private readonly string? secretJson = Environment.GetEnvironmentVariable("BigQuery_SecretJson");
    private readonly string? testuserEmail = Environment.GetEnvironmentVariable("Frends_TestUser_Email");
    private readonly string projectId = "instant-stone-387712";
    private Input input = new();
    private Connection connection = new();
    private Options options = new();

    [TestInitialize]
    public void Init()
    {
        var datasetId = $"tasktest_dataset_{DateTime.Now:yyyy_MM_dd_hh_mm_ss}";
        var tableId = $"tasktest_tableid_{DateTime.Now:yyyy_MM_dd_hh_mm_ss}";
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

        input = new()
        {
            Resource = Resources.Dataset,
            Description = "This is Description.",
            FriendlyName = "This is FriendlyName.",
            Label = null,
            SetEncryptionConfiguration = default,
            KmsKeyName = null,

            // Dataset
            Location = "EU",
            StorageBillingModel = "LOGICAL",
            IsCaseInsensitive = true,
            MaxTimeTravelHours = 48,
            Access = new[] { new AccessParameters() { Role = "READER", UserByEmail = testuserEmail } },
            Tag = new[] { new TagParameters() { Key = "key", Value = "value" } },

            // Tables
            RequirePartitionFilter = false,
            TableSchema = new[]
            {
                new TableSchemaParameters()
                {
                    Name = "Name",
                    Type = "STRING",
                },
                new TableSchemaParameters()
                {
                    Name = "AGE",
                    Type = "INTEGER",
                },
                new TableSchemaParameters()
                {
                    Name = "BDAY",
                    Type = "DATE",
                },
                new TableSchemaParameters()
                {
                    Name = "Country",
                    Type = "STRING",
                },
            },
            TimeField = "BDAY",
            Partition = TablePartitionOptions.RangePartitioning,
            TimeRequirePartitionFilter = false,
            Type = TimeTypeOptions.DAY,
            RangeField = "AGE",
            RangeStart = 1,
            RangeEnd = 2,
            RangeInterval = 3,
            ExpirationMs = 1,
        };

        connection = new()
        {
            ReadJsonMethod = ReadJsonMethods.JSON,
            SecretJson = json,
            ProjectId = projectId,
            DatasetId = datasetId,
            TableId = tableId,
            BaseUri = null,
            CredentialsFilePath = null,
        };

        options = new()
        {
            ThrowErrorOnFailure = true,
        };
    }

    [TestMethod]
    public async Task Update_Dataset()
    {
        await CreateDatasetResource(connection.ProjectId, connection.DatasetId, connection.SecretJson);

        input = new Input()
        {
            Resource = Resources.Dataset,
            Description = "This is updated Description.",
            FriendlyName = "This is updated FriendlyName.",
            Label = new[] { new LabelParameters() { Key = "updatedkey", Value = "updatedvalue" } },
            Location = null, // EU
            StorageBillingModel = "LOGICAL",
            IsCaseInsensitive = false, // was true
            MaxTimeTravelHours = 0, // was 48
            Tag = new[] { new TagParameters() { Key = "key2", Value = "value2" } }, // was key:value
        };

        var result = await GoogleBigQuery.PatchResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);

        await DeleteDataset(connection.ProjectId, connection.DatasetId, null, null, connection.SecretJson);
    }

    [TestMethod]
    public async Task Update_Table()
    {
        await CreateDatasetResource(connection.ProjectId, connection.DatasetId, connection.SecretJson);
        await CreateTableResource(connection.ProjectId, connection.DatasetId, connection.TableId, connection.SecretJson);

        input = new()
        {
            Resource = Resources.Table,
            RequirePartitionFilter = true, // false
            TableSchema = new[]
            {
                new TableSchemaParameters()
                {
                    Name = "AGE",
                    Type = "INTEGER",
                },
                new TableSchemaParameters()
                {
                    Name = "DAY",
                    Type = "INTEGER",
                },
                new TableSchemaParameters()
                {
                    Name = "NEWCOL",
                    Type = "INTEGER",
                },
            },
            Partition = TablePartitionOptions.RangePartitioning,
            RangeField = "AGE",
            RangeStart = 1,
            RangeEnd = 2,
            RangeInterval = 3,
            Description = null, // Should remove existing desc
            Location = null, // EU
            FriendlyName = "UpdateFriendlyName",
            Label = new[] { new LabelParameters() { Key = "updatekey", Value = "updatevalue" } },
        };

        var result = await GoogleBigQuery.PatchResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);

        await DeleteDataset(connection.ProjectId, connection.DatasetId, connection.TableId, null, connection.SecretJson);
    }

    [TestMethod]
    public async Task Update_Invalid_SecretJson_Throw()
    {
        connection.SecretJson = @"{ 
""Foo"": ""Bar""
}";
        var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => GoogleBigQuery.PatchResource(input, connection, options, default));
        Assert.IsNotNull(ex);
        Assert.AreEqual("Error creating credential from JSON or JSON parameters. Unrecognized credential type .", ex.Message);
    }

    [TestMethod]
    public async Task Update_Invalid_SecretJson_ReturnError()
    {
        connection.SecretJson = @"{ 
""Foo"": ""Bar""
}";

        options.ThrowErrorOnFailure = false;

        var result = await GoogleBigQuery.PatchResource(input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.AreEqual("Error creating credential from JSON or JSON parameters. Unrecognized credential type .", result.Error.Message);
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

    private static async Task CreateTableResource(string projectId, string datasetId, string tableId, string secretJson)
    {
        List<TableFieldSchema> tableFieldSchema = new() { new TableFieldSchema() { Name = "AGE", Type = "INTEGER" }, new TableFieldSchema() { Name = "DAY", Type = "INTEGER" } };
        TableSchema tableSchema = new() { Fields = tableFieldSchema };
        Table table = new()
        {
            TableReference = new TableReference
            {
                ProjectId = projectId,
                DatasetId = datasetId,
                TableId = tableId,
            },
            RequirePartitionFilter = false,
            Description = "This is desc",
            Schema = tableSchema,
            RangePartitioning = new RangePartitioning()
            {
                Field = "AGE",
                Range = new RangePartitioning.RangeData()
                {
                    Start = 1,
                    End = 2,
                    Interval = 3,
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

    private static async Task DeleteDataset(string projectId, string datasetId, string? tableId, string? routineId, string secretJson)
    {
        try
        {
            var bigqueryService = GetBigqueryService(secretJson);

            if (tableId != null)
                await bigqueryService.Tables.Delete(projectId, datasetId, tableId).ExecuteAsync();
            if (routineId != null)
                await bigqueryService.Routines.Delete(projectId, datasetId, routineId).ExecuteAsync();

            await bigqueryService.Datasets.Delete(projectId, datasetId).ExecuteAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
