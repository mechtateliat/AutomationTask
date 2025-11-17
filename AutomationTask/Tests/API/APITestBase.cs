using AutomationTask.Configuration;
using AutomationTask.Core.API;
using NUnit.Framework;

namespace AutomationTask.Tests.API;

public abstract class ApiTestBase : TestBase
{
    protected ApiClient ApiClient = null!;
    protected ApiUserClient ApiUserClient = null!;
    protected TestSettings Settings = null!;

    [SetUp]
    public async Task SetUp()
    {
        Settings = ApiTestSetup.GetSettings();
        ApiClient = await ApiClient.CreateAsync(Settings);
        ApiUserClient = new ApiUserClient(ApiClient);

        var testName = TestContext.CurrentContext.Test.Name;
        Test = ApiTestSetup.GetReportManager().CreateTest(testName);

        // Assign categories based on test categories
        foreach (var category in TestContext.CurrentContext.Test.Properties["Category"])
        {
            Test.AssignCategory(category.ToString()!);
        }
    }

    [TearDown]
    public async Task TearDown()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
        {
            Test?.Fail(TestContext.CurrentContext.Result.Message);
            Test?.Fail("Stack trace: " + TestContext.CurrentContext.Result.StackTrace);
        }
        else
        {
            Test?.Pass("Test passed");
        }

        if (ApiClient != null)
        {
            await ApiClient.DisposeAsync();
        }
    }
}
