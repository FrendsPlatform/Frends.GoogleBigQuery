using System.Text;
using Frends.GoogleBigQuery.Insert.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.GoogleBigQuery.Insert.Tests;

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
            DatasetId = "tasktest",
            TableId = "testtable",
            BaseUri = null,
            CredentialsFilePath = null,
        };

        input = new()
        {
            SourceFormat = SourceFormats.CSV,
            Delimiter = @";",
            SkipTopRows = 0,
            RowData = null,
            Csv = @"Name;Age;Country
Foo Csv;1;Fooland
Bar Csv;2;Barland",
            Json = @"[ {""Name"": ""Foo Json"", ""Age"": 1, ""Country"": ""Fooland""}, {""Name"": ""Bar Json"", ""Age"": 2, ""Country"": ""Barland""} ]",
        };

        options = new()
        {
            ThrowErrorOnFailure = true,
        };
    }

    [TestMethod]
    public async Task Import_Csv()
    {
        var result = await GoogleBigQuery.Insert(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Import_Csv_Delimiter()
    {
        input.Delimiter = @",";
        input.Csv = @"Name,Age,Country
Foo,1,Fooland
Bar,2,Barland";
        var result = await GoogleBigQuery.Insert(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Import_Csv_ColumnDoesntExists_ThrowErrorOnFailure_False()
    {
        options.ThrowErrorOnFailure = false;
        input.Csv = @"Invalid;Age;Country
Foo Csv;1;Fooland
Bar Csv;2;Barland";

        var result = await GoogleBigQuery.Insert(input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.AreEqual("no such field: Invalid.", result.Error.Message);
    }

    [TestMethod]
    public async Task Import_Csv_ColumnDoesntExists_ThrowErrorOnFailure_True()
    {
        input.Csv = @"Invalid;Age;Country
Foo Csv;1;Fooland
Bar Csv;2;Barland";

        var ex = await Assert.ThrowsExceptionAsync<Exception>(() => GoogleBigQuery.Insert(input, connection, options, default));
        Assert.IsNotNull(ex);
        Assert.AreEqual("no such field: Invalid.", ex.Message);
    }

    [TestMethod]
    public async Task Import_Json()
    {
        input.SourceFormat = SourceFormats.JSON;
        var result = await GoogleBigQuery.Insert(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Import_RowData_JSONCred()
    {
        input.SourceFormat = SourceFormats.RowData;

        var insertParams = new[]
        {
            new RowData
            {
                Row = new ColumnData[]
                {
                    new ColumnData { ColumnName = "Name", Value = $"Foo Par" },
                    new ColumnData { ColumnName = "Age", Value = 1 },
                    new ColumnData { ColumnName = "Country", Value = "Fooland" },
                },
            },
            new RowData
            {
                Row = new ColumnData[]
                {
                    new ColumnData { ColumnName = "Name", Value = $"Bar Par" },
                    new ColumnData { ColumnName = "Age", Value = 2 },
                    new ColumnData { ColumnName = "Country", Value = "Barland" },
                },
            },
        };

        input.RowData = insertParams;

        var result = await GoogleBigQuery.Insert(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);
    }

    [Ignore("Can't share the file. Set _connection.CredentialsFilePath manually.")]
    [TestMethod]
    public async Task Import_RowData_FileCred()
    {
        input.SourceFormat = SourceFormats.RowData;

        var insertParams = new[]
        {
            new RowData
            {
                Row = new ColumnData[]
                {
                    new ColumnData { ColumnName = "Name", Value = $"foo{Guid.NewGuid()}" },
                    new ColumnData { ColumnName = "Age", Value = 1 },
                    new ColumnData { ColumnName = "Country", Value = "Fooland" },
                },
            },
        };

        input.RowData = insertParams;

        connection.ReadJsonMethod = ReadJsonMethods.File;
        connection.CredentialsFilePath = @"path";

        var result = await GoogleBigQuery.Insert(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Import_Success_BaseUri()
    {
        connection.BaseUri = "https://bigquery.googleapis.com/bigquery/v2/";
        var result = await GoogleBigQuery.Insert(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task Import_Invalid_SecretJson_ReturnError()
    {
        connection.SecretJson = @"{ ""Foo"": ""Bar"" }";
        options.ThrowErrorOnFailure = false;

        var result = await GoogleBigQuery.Insert(input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.AreEqual("Error creating credential from JSON or JSON parameters. Unrecognized credential type .", result.Error.Message);
    }
}
