using Frends.GoogleBigQuery.ExecuteQuery.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.GoogleBigQuery.ExecuteQuery.Tests;

[TestClass]
public class ErrorHandlerTests
{
    [TestMethod]
    public async Task Uses_Custom_ErrorMessageOnFailure()
    {
        var connection = new Connection
        {
            ReadJsonMethod = ReadJsonMethods.JSON,
            SecretJson = @"{ ""Foo"": ""Bar"" }",
        };
        var input = new Input();
        var options = new Options
        {
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = "CustomErrorMessage",
        };

        var ex = await Assert.ThrowsExceptionAsync<Exception>(() => GoogleBigQuery.ExecuteQuery(input, connection, options, default));
        StringAssert.Contains(ex.Message, "CustomErrorMessage");
    }
}
