using System.Text;
using Frends.GoogleBigQuery.GetResource.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.GoogleBigQuery.GetResource.Tests;

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
            JobId = null,
            ModelId = null,
            ReadMask = null,
            RoutineId = "newroutine_x_2023_09_12_11_46_44",
            SelectedFields = null,
            View = Views.Storagestats,
        };

        options = new()
        {
            ThrowErrorOnFailure = true,
        };
    }

    [TestMethod]
    public async Task Get_Success_Datasets()
    {
        var result = await GoogleBigQuery.GetResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Get_Success_Routines()
    {
        input.Resource = Resources.Routines;

        var result = await GoogleBigQuery.GetResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Get_Success_Routines_ReadMask()
    {
        input.Resource = Resources.Routines;
        input.ReadMask = "Description";

        var result = await GoogleBigQuery.GetResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Get_Success_Tables()
    {
        input.Resource = Resources.Tables;
        var result = await GoogleBigQuery.GetResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Get_Success_Tables_SelectedFields()
    {
        input.Resource = Resources.Tables;
        input.SelectedFields = "age,country";
        var result = await GoogleBigQuery.GetResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Get_Success_Tables_ViewsFull()
    {
        input.View = Views.Full;
        var result = await GoogleBigQuery.GetResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Get_Success_Tables_ViewsBasic()
    {
        input.View = Views.Basic;
        var result = await GoogleBigQuery.GetResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Get_Success_Tables_ViewsTablemetadataviewunspecified()
    {
        input.View = Views.Tablemetadataviewunspecified;
        var result = await GoogleBigQuery.GetResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Get_Success_Tables_ViewsStoragestats()
    {
        input.View = Views.Storagestats;
        var result = await GoogleBigQuery.GetResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Get_Invalid_SecretJson_Throw()
    {
        connection.SecretJson = @"{ 
""Foo"": ""Bar""
}";
        var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => GoogleBigQuery.GetResource(input, connection, options, default));
        Assert.IsNotNull(ex);
        Assert.AreEqual("Error creating credential from JSON or JSON parameters. Unrecognized credential type .", ex.Message);
    }

    [TestMethod]
    public async Task Get_Invalid_SecretJson_ReturnError()
    {
        connection.SecretJson = @"{ 
""Foo"": ""Bar""
}";

        options.ThrowErrorOnFailure = false;

        var result = await GoogleBigQuery.GetResource(input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNull(result.Data);
        Assert.AreEqual("Error creating credential from JSON or JSON parameters. Unrecognized credential type .", result.Error.Message);
    }

    [TestMethod]
    public async Task Get_Success_BaseUri()
    {
        connection.BaseUri = "https://bigquery.googleapis.com/bigquery/v2/";
        var result = await GoogleBigQuery.GetResource(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.Error);
    }
}
