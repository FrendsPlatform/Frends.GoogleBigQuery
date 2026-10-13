using System.Text;
using Frends.GoogleBigQuery.ListResource.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.GoogleBigQuery.ListResource.Tests;

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
            ProjectId = "instant-stone-387712",
            BaseUri = null,
            CredentialsFilePath = null,
        };

        input = new()
        {
            Resource = Resources.Datasets,
            DatasetId = "tasktest",
            TableId = "testtable",
        };

        options = new()
        {
            ThrowErrorOnFailure = true,
        };
    }

    [TestMethod]
    public async Task List_Success_Datasets()
    {
        var result = await GoogleBigQuery.ListResource(input, connection, options, default);
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
            input.Resource = item;
            var result = await GoogleBigQuery.ListResource(input, connection, options, default);
            Assert.IsTrue(result.Success, item.ToString());
            Assert.IsNotNull(result.Data, item.ToString());
            Assert.IsNull(result.Error, item.ToString());
        }
    }

    [TestMethod]
    public async Task List_Invalid_SecretJson_Throw()
    {
        connection.SecretJson = @"{ 
""Foo"": ""Bar""
}";
        var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => GoogleBigQuery.ListResource(input, connection, options, default));
        Assert.IsNotNull(ex);
        Assert.AreEqual("Error creating credential from JSON or JSON parameters. Unrecognized credential type .", ex.Message);
    }

    [TestMethod]
    public async Task List_Invalid_SecretJson_ReturnError()
    {
        connection.SecretJson = @"{ 
""Foo"": ""Bar""
}";

        options.ThrowErrorOnFailure = false;

        var result = await GoogleBigQuery.ListResource(input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNull(result.Data);
        Assert.AreEqual("Error creating credential from JSON or JSON parameters. Unrecognized credential type .", result.Error.Message);
    }

    [TestMethod]
    public async Task List_Success_BaseUri()
    {
        connection.BaseUri = "https://bigquery.googleapis.com/bigquery/v2/";
        var result = await GoogleBigQuery.ListResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task ErrorHandler_Uses_Custom_ErrorMessageOnFailure()
    {
        connection.SecretJson = @"{ ""Foo"": ""Bar"" }";
        options.ThrowErrorOnFailure = true;
        options.ErrorMessageOnFailure = "CustomErrorMessage";

        var ex = await Assert.ThrowsExceptionAsync<Exception>(() => GoogleBigQuery.ListResource(input, connection, options, default));
        StringAssert.Contains(ex.Message, "CustomErrorMessage");
    }
}
