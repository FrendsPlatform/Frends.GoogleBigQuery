using Frends.GoogleBigQuery.UpdateResource.Definitions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Bigquery.v2;
using Google.Apis.Bigquery.v2.Data;
using Google.Apis.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Frends.GoogleBigQuery.UpdateResource.Tests;

[TestClass]
public class UnitTests
{
    private Input _input = new();
    private Connection _connection = new();
    private Options _options = new();
    private readonly string? _secretJson = Environment.GetEnvironmentVariable("BigQuery_SecretJson");
    private readonly string? _testuserEmail = Environment.GetEnvironmentVariable("Frends_TestUser_Email");
    private readonly string _projectId = "instant-stone-387712";

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

        _input = new()
        {
            Resource = Resources.Dataset,
            Description = "This is Description.",
            FriendlyName = "This is FriendlyName.",
            Label = null,
            SetEncryptionConfiguration = default,
            KmsKeyName = null,

            //Dataset
            Location = "EU",
            StorageBillingModel = "LOGICAL",
            IsCaseInsensitive = true,
            MaxTimeTravelHours = 48,
            Access = new[] { new AccessParameters() { Role = "READER", UserByEmail = _testuserEmail } },
            Tag = new[] { new TagParameters() { Key = "key", Value = "value" } },

            //Routines
            Argument = new[] { new ArgumentParameters() { DataType = "Int64", Name = "x", ArgumentKind = ArgumentKindOptions.FIXED_TYPE } },
            DefinitionBody = "(x * 3)",
            SetRemoteFunctionParameters = false,
            Language = "SQL",
            DeterminismLevel = DeterminismLevels.DETERMINISM_LEVEL_UNSPECIFIED,
            RoutineType = RoutineTypes.SCALAR_FUNCTION,
            StrictMode = true,
            ImportedLibrary = null,
            RemoteConnection = null,
            RemoteEndpoint = null,
            RemoteMaxBatchingRows = 1,
            RemoteUserDefinedContext = null,
            ReturnType = "Int64",

            //Tables
            RequirePartitionFilter = false,
            TableSchema = new[] {
                new TableSchemaParameters()
                {
                    Name = "Name",
                    Type = "STRING"
                },
                new TableSchemaParameters() {
                    Name = "AGE",
                    Type = "INTEGER"
                },
                new TableSchemaParameters() {
                    Name = "BDAY",
                    Type = "DATE"
                },
                new TableSchemaParameters() {
                    Name = "Country",
                    Type = "STRING"
                }
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

        _connection = new()
        {
            ReadJsonMethod = ReadJsonMethods.JSON,
            SecretJson = json,
            ProjectId = _projectId,
            DatasetId = _datasetId,
            TableId = _tableId,
            RoutineId = _routineId,
            BaseUri = null,
            CredentialsFilePath = null,
        };

        _options = new()
        {
            ThrowOnError = true,
        };
    }

    [TestMethod]
    public async Task Update_Dataset()
    {
        await CreateDatasetResource(_connection.ProjectId, _connection.DatasetId, _connection.SecretJson);

        _input = new Input()
        {
            Resource = Resources.Dataset,
            Description = "This is updated Description.",
            FriendlyName = "This is updated FriendlyName.",
            Label = new[] { new LabelParameters() { Key = "updatedkey", Value = "updatedvalue" } },
            Location = null, //EU
            StorageBillingModel = "LOGICAL",
            IsCaseInsensitive = false, //was true
            MaxTimeTravelHours = 0, //was 48
            Tag = new[] { new TagParameters() { Key = "key2", Value = "value2" } } // was key:value 
        };

        var result = await GoogleBigQuery.UpdateResource(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorMessage);

        await DeleteDataset(_connection.ProjectId, _connection.DatasetId, null, null, _connection.SecretJson);
    }

    [TestMethod]
    public async Task Update_Routine()
    {
        await CreateDatasetResource(_connection.ProjectId, _connection.DatasetId, _connection.SecretJson);
        await CreateRoutineResource(_connection.ProjectId, _connection.DatasetId, _connection.RoutineId, _connection.SecretJson);

        _input = new()
        {
            Resource = Resources.Routine,
            Argument = new[] { new ArgumentParameters() { DataType = "Int64", Name = "y", ArgumentKind = ArgumentKindOptions.FIXED_TYPE } }, //Name = "y"
            DefinitionBody = "(y * 3)", //"(x * 3)"
            SetRemoteFunctionParameters = false,
            Language = "SQL",
            DeterminismLevel = DeterminismLevels.DETERMINISM_LEVEL_UNSPECIFIED,
            RoutineType = RoutineTypes.SCALAR_FUNCTION,
            StrictMode = false, //true
            ImportedLibrary = null,
            RemoteConnection = null,
            RemoteEndpoint = null,
            RemoteMaxBatchingRows = 2, //1
            RemoteUserDefinedContext = null,
            ReturnType = "Int64",
        };
        var result = await GoogleBigQuery.UpdateResource(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorMessage);

        await DeleteDataset(_connection.ProjectId, _connection.DatasetId, null, _connection.RoutineId, _connection.SecretJson);
    }

    [TestMethod]
    public async Task Update_Table()
    {
        await CreateDatasetResource(_connection.ProjectId, _connection.DatasetId, _connection.SecretJson);
        await CreateTableResource(_connection.ProjectId, _connection.DatasetId, _connection.TableId, _connection.SecretJson);

        _input = new()
        {
            Resource = Resources.Table,
            RequirePartitionFilter = true, //false
            TableSchema = new[] {
                new TableSchemaParameters() {
                    Name = "AGE",
                    Type = "INTEGER"
                },
                new TableSchemaParameters() {
                    Name = "DAY",
                    Type = "INTEGER"
                },
                new TableSchemaParameters() {
                    Name = "NEWCOL",
                    Type = "INTEGER"
                }
            },
            Partition = TablePartitionOptions.RangePartitioning,
            RangeField = "AGE",
            RangeStart = 1,
            RangeEnd = 2,
            RangeInterval = 3,
            Description = null, //Should remove existing desc
            Location = null, //EU
            FriendlyName = "UpdateFriendlyName",
            Label = new[] { new LabelParameters() { Key = "updatekey", Value = "updatevalue" } },
        };

        var result = await GoogleBigQuery.UpdateResource(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorMessage);

        await DeleteDataset(_connection.ProjectId, _connection.DatasetId, _connection.TableId, null, _connection.SecretJson);
    }

    [TestMethod]
    public async Task Update_Invalid_SecretJson_Throw()
    {
        _connection.SecretJson = @"{ 
""Foo"": ""Bar""
}";
        var ex = await Assert.ThrowsExceptionAsync<Exception>(() => GoogleBigQuery.UpdateResource(_connection, _input, _options, default));
        Assert.IsNotNull(ex);
        Assert.AreEqual("Error occured: Error creating credential from JSON or JSON parameters. Unrecognized credential type .", ex.Message);
    }

    [TestMethod]
    public async Task Update_Invalid_SecretJson_ReturnError()
    {
        _connection.SecretJson = @"{ 
""Foo"": ""Bar""
}";

        _options.ThrowOnError = false;

        var result = await GoogleBigQuery.UpdateResource(_connection, _input, _options, default);
        Assert.IsFalse(result.Success);
        Assert.AreEqual("Error occured: Error creating credential from JSON or JSON parameters. Unrecognized credential type .", result.ErrorMessage);
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
        List<TableFieldSchema> tableFieldSchema = new() { new TableFieldSchema() { Name = "AGE", Type = "INTEGER" }, new TableFieldSchema() { Name = "DAY", Type = "INTEGER" } };
        TableSchema tableSchema = new() { Fields = tableFieldSchema };
        Table table = new()
        {
            TableReference = new TableReference
            {
                ProjectId = projectId,
                DatasetId = datasetId,
                TableId = TableId
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
                }
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
            ApplicationName = "BigQueryApp"
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