using AutomationTask.Configuration;
using AutomationTask.Core.Reporting;
using NUnit.Framework;

namespace AutomationTask.Tests.API;

[SetUpFixture]
public class ApiTestSetup
{
    private static IReportManager? reportManager;
    private static TestSettings? settings;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        settings = ConfigurationManager.GetTestSettings();
        reportManager = new ReportManager(settings);
        
        Console.WriteLine($"API Test Setup - Report initialized for environment: {settings.Environment}");
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Console.WriteLine("API Test Teardown - Flushing report");
        reportManager?.Flush();
    }

    public static IReportManager GetReportManager()
    {
        return reportManager ?? throw new InvalidOperationException("ReportManager not initialized");
    }

    public static TestSettings GetSettings()
    {
        return settings ?? throw new InvalidOperationException("Settings not initialized");
    }
}
