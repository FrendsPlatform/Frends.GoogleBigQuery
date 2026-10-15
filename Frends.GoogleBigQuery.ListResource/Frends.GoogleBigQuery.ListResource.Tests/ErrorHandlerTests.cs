using Frends.GoogleBigQuery.ListResource.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.GoogleBigQuery.ListResource.Tests;

[TestClass]
public class ErrorHandlerTests
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    [TestMethod]
    public async Task Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        await AssertThrowsAnyExceptionAsync(() =>
            GoogleBigQuery.ListResource(new Input(), new Connection(), new Options(), default));
    }

    [TestMethod]
    public async Task Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = new Options { ThrowErrorOnFailure = false };

        var result = await GoogleBigQuery.ListResource(new Input(), new Connection(), options, default);

        Assert.IsFalse(result.Success);
    }

    [TestMethod]
    public async Task Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = new Options { ErrorMessageOnFailure = CustomErrorMessage };

        var ex = await Assert.ThrowsExceptionAsync<Exception>(() =>
            GoogleBigQuery.ListResource(new Input(), new Connection(), options, default));

        StringAssert.Contains(ex.Message, CustomErrorMessage);
    }

    private static async Task AssertThrowsAnyExceptionAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception)
        {
            return;
        }

        Assert.Fail("Expected the task to throw an exception.");
    }
}
