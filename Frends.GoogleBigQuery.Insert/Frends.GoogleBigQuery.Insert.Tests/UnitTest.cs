using Frends.GoogleBigQuery.Insert.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Frends.GoogleBigQuery.Insert.Tests;

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
            DatasetId = "tasktest",
            TableId = "testtable",
            BaseUri = null,
            CredentialsFilePath = null,
        };

        _input = new()
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

        _options = new()
        {
            ThrowOnError = true,
        };
    }

    [TestMethod]
    public async Task Import_Csv()
    {
        var result = await GoogleBigQuery.Insert(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Import_Csv_Delimiter()
    {
        _input.Delimiter = @",";
        _input.Csv = @"Name,Age,Country
Foo,1,Fooland
Bar,2,Barland";
        var result = await GoogleBigQuery.Insert(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Import_Csv_ColumnDoesntExists_ThrowOnError_False()
    {
        _options.ThrowOnError = false;
        _input.Csv = @"Invalid;Age;Country
Foo Csv;1;Fooland
Bar Csv;2;Barland";

        var result = await GoogleBigQuery.Insert(_connection, _input, _options, default);
        Assert.IsFalse(result.Success);
        Assert.AreEqual("Error occured: no such field: Invalid.", result.ErrorMessage);
    }

    [TestMethod]
    public async Task Import_Csv_ColumnDoesntExists_ThrowOnError_True()
    {
        _input.Csv = @"Invalid;Age;Country
Foo Csv;1;Fooland
Bar Csv;2;Barland";

        var ex = await Assert.ThrowsExceptionAsync<Exception>(() => GoogleBigQuery.Insert(_connection, _input, _options, default));
        Assert.IsNotNull(ex);
        Assert.AreEqual("Error occured: no such field: Invalid.", ex.Message);
    }

    [TestMethod]
    public async Task Import_Json()
    {
        _input.SourceFormat = SourceFormats.JSON;
        var result = await GoogleBigQuery.Insert(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Import_RowData_JSONCred()
    {
        _input.SourceFormat = SourceFormats.RowData;

        var insertParams = new[]
        {
            new RowData
            {
                Row = new ColumnData[]
                {
                    new ColumnData { ColumnName = "Name", Value = $"Foo Par" },
                    new ColumnData { ColumnName = "Age", Value = 1 },
                    new ColumnData { ColumnName = "Country", Value = "Fooland" },
                }
            },
            new RowData
            {
                Row = new ColumnData[]
                {
                    new ColumnData { ColumnName = "Name", Value = $"Bar Par" },
                    new ColumnData { ColumnName = "Age", Value = 2 },
                    new ColumnData { ColumnName = "Country", Value = "Barland" },
                }
            },
        };

        _input.RowData = insertParams;

        var result = await GoogleBigQuery.Insert(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorMessage);
    }

    [Ignore("Can't share the file. Set _connection.CredentialsFilePath manually.")]
    [TestMethod]
    public async Task Import_RowData_FileCred()
    {
        _input.SourceFormat = SourceFormats.RowData;

        var insertParams = new[]
        {
            new RowData
            {
                Row = new ColumnData[]
                {
                    new ColumnData { ColumnName = "Name", Value = $"foo{Guid.NewGuid()}" },
                    new ColumnData { ColumnName = "Age", Value = 1 },
                    new ColumnData { ColumnName = "Country", Value = "Fooland" },
                }
            },
        };

        _input.RowData = insertParams;

        _connection.ReadJsonMethod = ReadJsonMethods.File;
        _connection.CredentialsFilePath = @"path";

        var result = await GoogleBigQuery.Insert(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Import_Success_BaseUri()
    {
        _connection.BaseUri = "https://bigquery.googleapis.com/bigquery/v2/";
        var result = await GoogleBigQuery.Insert(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public async Task Import_Invalid_SecretJson_ReturnError()
    {
        _connection.SecretJson = @"{ ""Foo"": ""Bar"" }";
        _options.ThrowOnError = false;

        var result = await GoogleBigQuery.Insert(_connection, _input, _options, default);
        Assert.IsFalse(result.Success);
        Assert.AreEqual("Error occured: Error creating credential from JSON or JSON parameters. Unrecognized credential type .", result.ErrorMessage);
    }
}