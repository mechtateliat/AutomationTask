using Autofac;
using AutomationTask.Core.DI;
using AutomationTask.Core.Reporting;
using NUnit.Framework;

namespace AutomationTask.Tests.UI;

[SetUpFixture]
public class UiTestSetup
{
    private static IContainer? container;
    private static IReportManager? reportManager;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {        
        container = TestContainerBuilder.BuildContainer();
        reportManager = container.Resolve<IReportManager>();
        
        Console.WriteLine("UI Test Setup - Report and container initialized");
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Console.WriteLine("UI Test Teardown - Flushing report and disposing container");
        reportManager?.Flush();
        container?.Dispose();
    }

    public static IContainer GetContainer()
    {
        return container ?? throw new InvalidOperationException("Container not initialized");
    }

    public static IReportManager GetReportManager()
    {
        return reportManager ?? throw new InvalidOperationException("ReportManager not initialized");
    }
}
