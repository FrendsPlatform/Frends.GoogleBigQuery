using Frends.GoogleBigQuery.ListResource.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Frends.GoogleBigQuery.ListResource.Tests;

[TestClass]
public class UnitTest
{
    private Connection _connection = new();
    private Input _input = new();
    private Options _options = new();
    private readonly string? _secretJson = Environment.GetEnvironmentVariable("BigQuery_SecretJson");

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
            ProjectId = "instant-stone-387712",
            BaseUri = null,
            CredentialsFilePath = null
        };

        _input = new()
        {
            Resource = Resources.Datasets,
            DatasetId = "tasktest",
            TableId = "testtable"
        };

        _options = new()
        {
            ThrowErrorOnFailure = true,
        };
    }

    [TestMethod]
    public async Task List_Success_Datasets()
    {
        var result = await GoogleBigQuery.ListResource(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task List_Success_AllResources()
    {
        var allResourceTypes = Enum.GetValues(typeof(Resources)).Cast<Resources>().ToList();

        foreach (var item in allResourceTypes)
        {
            _input.Resource = item;
            var result = await GoogleBigQuery.ListResource(_input, _connection, _options, default);
            Assert.IsTrue(result.Success, item.ToString());
            Assert.IsNotNull(result.Data, item.ToString());
            Assert.IsNull(result.Error, item.ToString());
        }
    }

    [TestMethod]
    public async Task List_Invalid_SecretJson_Throw()
    {
        _connection.SecretJson = @"{ 
""Foo"": ""Bar""
}";
        var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => GoogleBigQuery.ListResource(_input, _connection, _options, default));
        Assert.IsNotNull(ex);
        Assert.AreEqual("Error creating credential from JSON or JSON parameters. Unrecognized credential type .", ex.Message);
    }

    [TestMethod]
    public async Task List_Invalid_SecretJson_ReturnError()
    {
        _connection.SecretJson = @"{ 
""Foo"": ""Bar""
}";

        _options.ThrowErrorOnFailure = false;

        var result = await GoogleBigQuery.ListResource(_input, _connection, _options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNull(result.Data);
        Assert.AreEqual("Error creating credential from JSON or JSON parameters. Unrecognized credential type .", result.Error.Message);
    }

    [TestMethod]
    public async Task List_Success_BaseUri()
    {
        _connection.BaseUri = "https://bigquery.googleapis.com/bigquery/v2/";
        var result = await GoogleBigQuery.ListResource(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task ErrorHandler_Uses_Custom_ErrorMessageOnFailure()
    {
        _connection.SecretJson = @"{ ""Foo"": ""Bar"" }";
        _options.ThrowErrorOnFailure = true;
        _options.ErrorMessageOnFailure = "CustomErrorMessage";

        var ex = await Assert.ThrowsExceptionAsync<Exception>(() => GoogleBigQuery.ListResource(_input, _connection, _options, default));
        StringAssert.Contains(ex.Message, "CustomErrorMessage");
    }
}
