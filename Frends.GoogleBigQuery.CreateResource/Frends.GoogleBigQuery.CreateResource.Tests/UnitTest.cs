using System.Text;
using Frends.GoogleBigQuery.CreateResource.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.GoogleBigQuery.CreateResource.Tests;

[TestClass]
public class UnitTest
{
    private readonly string? secretJson = Environment.GetEnvironmentVariable("BigQuery_SecretJson");
    private readonly string? testuserEmail = Environment.GetEnvironmentVariable("Frends_TestUser_Email");
    private readonly string projectId = "instant-stone-387712";
    private readonly string datasetId = "tasktest";
    private Connection connection = new();
    private Input input = new();
    private Options options = new();

    [TestInitialize]
    public void Init()
    {
        string json;

        // This one is to handle CI secret problems.
        if (File.Exists(secretJson))
        {
            json = File.ReadAllText(secretJson);
        }
        else
        {
            var json1 = !string.IsNullOrEmpty(secretJson) ? secretJson : string.Empty;
            byte[] bytes = Convert.FromBase64String(json1);
            json = Encoding.UTF8.GetString(bytes);
        }

        connection = new()
        {
            ReadJsonMethod = ReadJsonMethods.JSON,
            SecretJson = json,
            ProjectId = projectId,
            DatasetId = datasetId,
            BaseUri = null,
            CredentialsFilePath = null,
        };

        input = new()
        {
            Resource = Resources.Dataset,
            Description = "This is Description.",
            FriendlyName = "This is FriendlyName.",
            Label = null,
            SetEncryptionConfiguration = default,
            KmsKeyName = null,

            // Dataset
            DatasetId = $"Dataset_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}",
            Location = "EU",
            StorageBillingModel = "LOGICAL",
            IsCaseInsensitive = true,
            MaxTimeTravelHours = 48,
            Access = new[] { new AccessParameters() { Role = "READER", UserByEmail = testuserEmail } },
            Tag = new[] { new TagParameters() { Key = "key", Value = "value" } },

            // Routines
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

        options = new()
        {
            ThrowErrorOnFailure = true,
        };
    }

    [TestMethod]
    public async Task Create_Dataset()
    {
        var result = await GoogleBigQuery.CreateResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Url);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Create_Dataset_NULL_BOOL_Unlimited()
    {
        input.IsCaseInsensitive = true;
        input.MaxTimeTravelHours = 0;
        input.Access = null;
        input.Tag = null;

        var result = await GoogleBigQuery.CreateResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Url);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Create_Table()
    {
        input.TableId = $"newtable_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}_1";
        input.Resource = Resources.Table;

        var result = await GoogleBigQuery.CreateResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Url);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Create_Table_TimePartitioning_Day()
    {
        input.TableId = $"newtable_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}_2";
        input.Resource = Resources.Table;
        input.Partition = TablePartitionOptions.TimePartitioning;

        var result = await GoogleBigQuery.CreateResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Url);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Create_Table_TimePartitioning_MONTH()
    {
        input.TableId = $"newtable_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}_3";
        input.Resource = Resources.Table;
        input.Partition = TablePartitionOptions.TimePartitioning;
        input.Type = TimeTypeOptions.MONTH;

        var result = await GoogleBigQuery.CreateResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Url);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Create_Table_TimePartitioning_YEAR()
    {
        input.TableId = $"newtable_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}_4";
        input.Resource = Resources.Table;
        input.Partition = TablePartitionOptions.TimePartitioning;
        input.Type = TimeTypeOptions.YEAR;

        var result = await GoogleBigQuery.CreateResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Url);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Create_Table_Bool()
    {
        input.TableId = $"newtable_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}_5";
        input.RoutineId = $"table_bool{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}";
        input.Resource = Resources.Table;
        input.RequirePartitionFilter = true;

        var result = await GoogleBigQuery.CreateResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Url);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Create_Routine()
    {
        input.RoutineId = $"newroutine_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}_1";
        input.Resource = Resources.Routine;

        var result = await GoogleBigQuery.CreateResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Url);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Create_Routine_NOT_DETERMINISTIC()
    {
        input.RoutineId = $"newroutine_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}_2";
        input.Resource = Resources.Routine;
        input.DefinitionBody = "function multiplyByThree(x) { return x * 3; }";
        input.Language = "JAVASCRIPT";
        input.DeterminismLevel = DeterminismLevels.NOT_DETERMINISTIC;

        var result = await GoogleBigQuery.CreateResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Url);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Create_Routine_DETERMINISTIC()
    {
        input.RoutineId = $"newroutine_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}_3";
        input.Resource = Resources.Routine;
        input.DefinitionBody = "function multiplyByThree(x) { return x * 3; }";
        input.Language = "JAVASCRIPT";
        input.DeterminismLevel = DeterminismLevels.DETERMINISTIC;

        var result = await GoogleBigQuery.CreateResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Url);
        Assert.IsNull(result.Error);
    }

    [Ignore("Can't share the file. Set _connection.CredentialsFilePath manually.")]
    [TestMethod]
    public async Task Create_RowData_FileCred()
    {
        connection.ReadJsonMethod = ReadJsonMethods.File;
        connection.CredentialsFilePath = @"path";

        var result = await GoogleBigQuery.CreateResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Create_Success_BaseUri()
    {
        connection.BaseUri = "https://bigquery.googleapis.com/bigquery/v2/";
        var result = await GoogleBigQuery.CreateResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Create_Invalid_SecretJson_ReturnError()
    {
        connection.SecretJson = @"{ ""Foo"": ""Bar"" }";
        options.ThrowErrorOnFailure = false;

        var result = await GoogleBigQuery.CreateResource(input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.AreEqual("Error creating credential from JSON or JSON parameters. Unrecognized credential type .", result.Error.Message);
    }

    [TestMethod]
    public async Task ErrorHandler_Throws_When_ThrowErrorOnFailure_Is_True()
    {
        connection.SecretJson = @"{ ""Foo"": ""Bar"" }";
        options.ThrowErrorOnFailure = true;

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => GoogleBigQuery.CreateResource(input, connection, options, default));
    }

    [TestMethod]
    public async Task ErrorHandler_Uses_Custom_ErrorMessageOnFailure()
    {
        connection.SecretJson = @"{ ""Foo"": ""Bar"" }";
        options.ThrowErrorOnFailure = true;
        options.ErrorMessageOnFailure = "CustomErrorMessage";

        var ex = await Assert.ThrowsExceptionAsync<Exception>(() => GoogleBigQuery.CreateResource(input, connection, options, default));
        StringAssert.Contains(ex.Message, "CustomErrorMessage");
    }
}
