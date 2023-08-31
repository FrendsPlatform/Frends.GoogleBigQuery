using Frends.GoogleBigQuery.List.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.GoogleBigQuery.List.Tests;

[TestClass]
public class UnitTest
{
    private Connection _connection = new();
    private Options _options = new();
    private readonly string? _secretJson = Environment.GetEnvironmentVariable("BigQuery_SecretJson");

    [TestInitialize]
    public void Init()
    {
        var json = File.Exists(_secretJson) ? File.ReadAllText(_secretJson) : _secretJson; //Plaintext in CI

        _connection = new()
        {
            ReadJsonMethod = ReadJsonMethods.JSON,
            Resource = Resource.Datasets,
            SecretJson = json,
            ProjectId = "instant-stone-387712",
            DatasetId = "tasktest",
            TableId = "testtable",
            BaseUri = null,
            CredentialsFilePath = null,
        };

        _options = new()
        {
            ThrowOnError = true,
        };
    }

    [TestMethod]
    public async Task List_Success_Datasets()
    {
        var result = await GoogleBigQuery.List(_connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task List_Success_AllResources()
    {
        var allResourceTypes = Enum.GetValues(typeof(Resource)).Cast<Resource>().ToList();

        foreach (var item in allResourceTypes)
        {
            _connection.Resource = item;
            var result = await GoogleBigQuery.List(_connection, _options, default);
            Assert.IsTrue(result.Success, item.ToString());
            Assert.IsNotNull(result.Data, item.ToString());
            Assert.IsNull(result.ErrorMessage, item.ToString());
        }
    }

    [TestMethod]
    public async Task List_Invalid_SecretJson()
    {
        _connection.SecretJson = @"{ 
""Foo"": ""Bar""
}";
        var ex = await Assert.ThrowsExceptionAsync<Exception>(() => GoogleBigQuery.List(_connection, _options, default));
        Assert.IsNotNull(ex);
        Assert.IsTrue(ex.Message.Contains("Error creating credential from JSON or JSON parameters. Unrecognized credential type"));
    }

    [TestMethod]
    public async Task List_Success_BaseUri()
    {
        _connection.BaseUri = "https://bigquery.googleapis.com/bigquery/v2/";
        var result = await GoogleBigQuery.List(_connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);
    }
}