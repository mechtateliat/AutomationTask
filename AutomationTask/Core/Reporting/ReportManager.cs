using AutomationTask.Configuration;
using AutomationTask.Core.TestArtifacts;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;
using System.Globalization;

namespace AutomationTask.Core.Reporting;

public interface IReportManager
{
    ExtentTest CreateTest(string testName, string? description = null);
    void Flush();
}

public class ReportManager : IReportManager
{
    private static ExtentReports? _extent;
    private static readonly object _lock = new();

    public ReportManager(TestSettings settings)
    {
        if (_extent == null)
        {
            lock (_lock)
            {
                if (_extent == null)
                {
                    InitializeReport(settings);
                }
            }
        }
    }

    private void InitializeReport(TestSettings settings)
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

        // Ensure all artifact directories exist
        TestArtifactsManager.EnsureDirectoriesExist();

        // Get report path using centralized manager
        var reportFileName = TestArtifactsManager.GetReportFileName("AutomationReport");
        var reportPath = Path.Combine(TestArtifactsManager.ReportsDirectory, reportFileName);

        var htmlReporter = new ExtentSparkReporter(reportPath);
        htmlReporter.Config.DocumentTitle = settings.Reporting.ReportTitle;
        htmlReporter.Config.ReportName = settings.Reporting.ReportTitle;
        htmlReporter.Config.Theme = Theme.Standard;

        // Make timestamp column wider to fit full date and time
        htmlReporter.Config.CSS = @"
                                 .timestamp-col {
                                 min-width: 200px !important;
                                 width: 200px !important;}";

        _extent = new ExtentReports();
        _extent.AttachReporter(htmlReporter);

        _extent.AddSystemInfo("Environment", settings.Environment);
        _extent.AddSystemInfo("Browser", settings.Ui.Browser);
        _extent.AddSystemInfo("Base URL (UI)", settings.Ui.BaseUrl);
        _extent.AddSystemInfo("Base URL (API)", settings.Api.BaseUrl);
    }

    public ExtentTest CreateTest(string testName, string? description = null)
    {
        if (_extent == null)
        {
            throw new InvalidOperationException("Report not initialized");
        }

        return _extent.CreateTest(testName, description);
    }

    public void Flush()
    {
        _extent?.Flush();
    }

    public static void ResetReport()
    {
        lock (_lock)
        {
            _extent = null;
        }
    }
}
