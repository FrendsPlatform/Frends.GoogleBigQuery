using Frends.GoogleBigQuery.GetResource.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Frends.GoogleBigQuery.GetResource.Tests;

[TestClass]
public class UnitTest
{
    private Connection _connection = new();
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
            Resource = Resources.Datasets,
            SecretJson = json,
            ProjectId = "instant-stone-387712",
            DatasetId = "tasktest",
            TableId = "testtable",
            BaseUri = null,
            CredentialsFilePath = null,
            JobId = null,
            ModelId = null,
            ReadMask = null,
            RoutineId = null,
            SelectedFields = null,
            View = Views.Storagestats
        };

        _options = new()
        {
            ThrowOnError = true,
        };
    }

    [TestMethod]
    public async Task Get_Success_Datasets()
    {
        var result = await GoogleBigQuery.GetResource(_connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Get_Success_Tables()
    {
        _connection.Resource = Resources.Tables;
        var result = await GoogleBigQuery.GetResource(_connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Get_Success_Tables_SelectedFields()
    {
        _connection.Resource = Resources.Tables;
        _connection.SelectedFields = ("age,country");
        var result = await GoogleBigQuery.GetResource(_connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Get_Success_Tables_ViewsFull()
    {
        _connection.View = Views.Full;
        var result = await GoogleBigQuery.GetResource(_connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Get_Success_Tables_ViewsBasic()
    {
        _connection.View = Views.Basic;
        var result = await GoogleBigQuery.GetResource(_connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Get_Success_Tables_ViewsTablemetadataviewunspecified()
    {
        _connection.View = Views.Tablemetadataviewunspecified;
        var result = await GoogleBigQuery.GetResource(_connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Get_Success_Tables_ViewsStoragestats()
    {
        _connection.View = Views.Storagestats;
        var result = await GoogleBigQuery.GetResource(_connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Get_Invalid_SecretJson_Throw()
    {
        _connection.SecretJson = @"{ 
""Foo"": ""Bar""
}";
        var ex = await Assert.ThrowsExceptionAsync<Exception>(() => GoogleBigQuery.GetResource(_connection, _options, default));
        Assert.IsNotNull(ex);
        Assert.AreEqual("Error occured: Error creating credential from JSON or JSON parameters. Unrecognized credential type .", ex.Message);
    }

    [TestMethod]
    public async Task Get_Invalid_SecretJson_ReturnError()
    {
        _connection.SecretJson = @"{ 
""Foo"": ""Bar""
}";

        _options.ThrowOnError = false;

        var result = await GoogleBigQuery.GetResource(_connection, _options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNull(result.Data);
        Assert.AreEqual("Error occured: Error creating credential from JSON or JSON parameters. Unrecognized credential type .", result.ErrorMessage);
    }

    [TestMethod]
    public async Task Get_Success_BaseUri()
    {
        _connection.BaseUri = "https://bigquery.googleapis.com/bigquery/v2/";
        var result = await GoogleBigQuery.GetResource(_connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);
    }
}