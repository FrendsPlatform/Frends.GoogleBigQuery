using Frends.GoogleBigQuery.ListResource.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.GoogleBigQuery.ListResource.Tests;

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

        var ex = await Assert.ThrowsExceptionAsync<Exception>(() => GoogleBigQuery.ListResource(input, connection, options, default));
        StringAssert.Contains(ex.Message, "CustomErrorMessage");
    }
}
