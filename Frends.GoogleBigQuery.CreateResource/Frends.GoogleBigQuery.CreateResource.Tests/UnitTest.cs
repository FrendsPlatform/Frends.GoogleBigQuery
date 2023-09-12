using Frends.GoogleBigQuery.CreateResource.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Frends.GoogleBigQuery.CreateResource.Tests;

[TestClass]
public class UnitTest
{
    private Connection _connection = new();
    private Input _input = new();
    private Options _options = new();
    private readonly string? _secretJson = Environment.GetEnvironmentVariable("BigQuery_SecretJson");
    private readonly string? _testuserEmail = Environment.GetEnvironmentVariable("Frends_TestUser_Email");
    private readonly string _projectId = "instant-stone-387712";
    private readonly string _datasetId = "tasktest";

    [TestInitialize]
    public void Init()
    {
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
            BaseUri = null,
            CredentialsFilePath = null,
        };

        _input = new()
        {
            Resource = Resources.Dataset,
            Description = "This is Description.",
            FriendlyName = "This is FriendlyName.",
            Label = null,
            SetEncryptionConfiguration = default,
            KmsKeyName = null,

            //Dataset
            DatasetId = $"Dataset_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}",
            Location = "EU",
            StorageBillingModel = "LOGICAL",
            IsCaseInsensitive = true,
            MaxTimeTravelHours = 48,
            Access = new[] { new AccessParameters() { Role = "READER", UserByEmail = _testuserEmail } },
            Tag = new[] { new TagParameters() { Key = "key", Value = "value" } },

            //Routines
            RoutineId = null,
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
            TableId = null,
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

        _options = new()
        {
            ThrowOnError = true,
        };
    }

    [TestMethod]
    public async Task Create_Dataset()
    {
        var result = await GoogleBigQuery.CreateResource(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Url);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Create_Dataset_NULL_BOOL_Unlimited()
    {
        _input.IsCaseInsensitive = true;
        _input.MaxTimeTravelHours = 0;
        _input.Access = null;
        _input.Tag = null;

        var result = await GoogleBigQuery.CreateResource(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Url);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Create_Table()
    {
        _input.TableId = $"newtable_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}_1";
        _input.Resource = Resources.Table;

        var result = await GoogleBigQuery.CreateResource(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Url);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Create_Table_TimePartitioning_Day()
    {
        _input.TableId = $"newtable_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}_2";
        _input.Resource = Resources.Table;
        _input.Partition = TablePartitionOptions.TimePartitioning;

        var result = await GoogleBigQuery.CreateResource(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Url);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Create_Table_TimePartitioning_MONTH()
    {
        _input.TableId = $"newtable_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}_3";
        _input.Resource = Resources.Table;
        _input.Partition = TablePartitionOptions.TimePartitioning;
        _input.Type = TimeTypeOptions.MONTH;

        var result = await GoogleBigQuery.CreateResource(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Url);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Create_Table_TimePartitioning_YEAR()
    {
        _input.TableId = $"newtable_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}_4";
        _input.Resource = Resources.Table;
        _input.Partition = TablePartitionOptions.TimePartitioning;
        _input.Type = TimeTypeOptions.YEAR;

        var result = await GoogleBigQuery.CreateResource(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Url);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Create_Table_Bool()
    {
        _input.TableId = $"newtable_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}_5";
        _input.RoutineId = $"table_bool{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}";
        _input.Resource = Resources.Table;
        _input.RequirePartitionFilter = true;

        var result = await GoogleBigQuery.CreateResource(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Url);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Create_Routine()
    {
        _input.RoutineId = $"newroutine_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}_1";
        _input.Resource = Resources.Routine;

        var result = await GoogleBigQuery.CreateResource(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Url);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Create_Routine_NOT_DETERMINISTIC()
    {
        _input.RoutineId = $"newroutine_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}_2";
        _input.Resource = Resources.Routine;
        _input.DefinitionBody = "function multiplyByThree(x) { return x * 3; }";
        _input.Language = "JAVASCRIPT";
        _input.DeterminismLevel = DeterminismLevels.NOT_DETERMINISTIC;

        var result = await GoogleBigQuery.CreateResource(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Url);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Create_Routine_DETERMINISTIC()
    {
        _input.RoutineId = $"newroutine_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}_3";
        _input.Resource = Resources.Routine;
        _input.DefinitionBody = "function multiplyByThree(x) { return x * 3; }";
        _input.Language = "JAVASCRIPT";
        _input.DeterminismLevel = DeterminismLevels.DETERMINISTIC;

        var result = await GoogleBigQuery.CreateResource(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Url);
        Assert.IsNull(result.ErrorMessage);
    }

    [Ignore("Can't share the file. Set _connection.CredentialsFilePath manually.")]
    [TestMethod]
    public async Task Create_RowData_FileCred()
    {
        _connection.ReadJsonMethod = ReadJsonMethods.File;
        _connection.CredentialsFilePath = @"path";

        var result = await GoogleBigQuery.CreateResource(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Create_Success_BaseUri()
    {
        _connection.BaseUri = "https://bigquery.googleapis.com/bigquery/v2/";
        var result = await GoogleBigQuery.CreateResource(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Create_Invalid_SecretJson_ReturnError()
    {
        _connection.SecretJson = @"{ ""Foo"": ""Bar"" }";
        _options.ThrowOnError = false;

        var result = await GoogleBigQuery.CreateResource(_connection, _input, _options, default);
        Assert.IsFalse(result.Success);
        Assert.AreEqual("Error occured: Error creating credential from JSON or JSON parameters. Unrecognized credential type .", result.ErrorMessage);
    }
}