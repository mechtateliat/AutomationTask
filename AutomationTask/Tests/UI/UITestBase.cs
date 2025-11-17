using Autofac;
using AutomationTask.Configuration;
using AutomationTask.Core.TestArtifacts;
using AutomationTask.Core.UI;
using AutomationTask.Pages;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Globalization;

namespace AutomationTask.Tests.UI;

[TestFixture]
public abstract class UiTestBase : TestBase
{
    private ILifetimeScope? _scope;
    protected IPageContext? PageContext;
    protected IPage Page => PageContext?.Page ?? throw new InvalidOperationException("Page not initialized");
    protected TestSettings Settings = null!;

    [SetUp]
    public async Task SetUp()
    {
        var container = UiTestSetup.GetContainer();
        _scope = container.BeginLifetimeScope();
        
        Settings = _scope.Resolve<TestSettings>();
        PageContext = _scope.Resolve<IPageContext>();
        
        await PageContext.InitializeAsync();

        // Create test in report
        var testName = TestContext.CurrentContext.Test.Name;
        Test = UiTestSetup.GetReportManager().CreateTest(testName);

        // Assign categories based on test categories
        if (Test != null)
        {
            foreach (var category in TestContext.CurrentContext.Test.Properties["Category"])
            {
                Test.AssignCategory(category.ToString()!);
            }
        }

        // Log test start timestamp
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        Test?.Info($"Test started at: {timestamp}");
    }

    [TearDown]
    public async Task TearDown()
    {
        var testName = TestContext.CurrentContext.Test.Name;
        var testFailed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed;
        
        if (Test != null)
        {
            if (testFailed)
            {
                Test.Fail(TestContext.CurrentContext.Result.Message);

                // Take screenshot on failure - only if PageContext and Page are initialized
                if (PageContext != null)
                {
                    try
                    {
                        // Check if page is actually initialized before accessing it
                        var page = PageContext.Page;
                        if (page != null)
                        {
                            var screenshot = await page.ScreenshotAsync();
                            var screenshotFileName = TestArtifactsManager.GetScreenshotFileName(testName, "Failed");
                            var screenshotPath = Path.Combine(TestArtifactsManager.ScreenshotsDirectory, screenshotFileName);
                            
                            // Save screenshot to file
                            File.WriteAllBytes(screenshotPath, screenshot);
                            
                            // Add to report
                            Test.AddScreenCaptureFromPath(screenshotPath);
                            
                            TestContext.WriteLine($"Screenshot saved to: {screenshotPath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        TestContext.WriteLine($"Failed to save screenshot: {ex.Message}");
                        Test.Warning($"Screenshot capture failed: {ex.Message}");
                    }
                }

                // Save trace on failure if enabled
                if (PageContext != null && Settings?.Ui?.Trace != "off")
                {
                    try
                    {
                        var page = PageContext.Page;
                        if (page != null)
                        {
                            var traceFileName = TestArtifactsManager.GetTraceFileName(testName);
                            var tracePath = Path.Combine(TestArtifactsManager.TracesDirectory, traceFileName);
                            await page.Context.Tracing.StopAsync(new TracingStopOptions
                            {
                                Path = tracePath
                            });
                            TestContext.WriteLine($"Trace saved to: {tracePath}");
                            // Add a download link to the trace file in the report
                            var relativeTracePath = $"traces/{traceFileName}";
                            var traceHtml = $@"<a href='{relativeTracePath}' download>Download Playwright trace (.zip)</a>";
                            Test.Info(traceHtml);
                        }
                    }
                    catch (Exception ex)
                    {
                        TestContext.WriteLine($"Failed to save trace: {ex.Message}");
                        Test.Warning($"Trace capture failed: {ex.Message}");
                    }
                }
            }
            else
            {
                Test.Pass("Test passed");
                
                // Stop trace without saving if test passed and trace is set to "retain-on-failure"
                if (PageContext != null && Settings?.Ui?.Trace == "retain-on-failure")
                {
                    try
                    {
                        var page = PageContext.Page;
                        if (page != null)
                        {
                            await page.Context.Tracing.StopAsync();
                        }
                    }
                    catch
                    {
                        // Ignore trace stop errors on passed tests
                    }
                }
            }
        }

        // Handle video recording - only if PageContext is initialized
        if (PageContext != null && Settings?.Ui?.Video != "off")
        {
            try
            {
                // Safely get the page and video reference
                IVideo? video = null;
                try
                {
                    var page = PageContext.Page;
                    if (page != null)
                    {
                        video = page.Video;
                    }
                }
                catch
                {
                    // Page not initialized, skip video handling
                    video = null;
                }
                
                // Close the context to finalize the video
                await PageContext.CleanupAsync();

                // Get the video path after context is closed
                string? videoPath = null;
                if (video != null)
                {
                    videoPath = await video.PathAsync();
                }

                // Rename video based on test result and settings
                if (Settings?.Ui?.Video == "on" || (Settings?.Ui?.Video == "retain-on-failure" && testFailed))
                {
                    var suffix = testFailed ? "Failed" : "Passed";
                    var newVideoPath = await TestArtifactsManager.RenameVideoAsync(videoPath, testName, suffix);
                    
                    if (newVideoPath != null && Test != null)
                    {
                        TestContext.WriteLine($"Video saved to: {newVideoPath}");
                        
                        // Get relative path for the report
                        var videoFileName = Path.GetFileName(newVideoPath);
                        var relativeVideoPath = $"videos/{videoFileName}";
                        
                        // Embed video in report using HTML5 video tag
                        var videoHtml = $@"
                            <video width='800' controls>
                                <source src='{relativeVideoPath}' type='video/webm'>
                                Your browser does not support the video tag.
                            </video>
                            <br/>
                            <a href='{relativeVideoPath}' target='_blank'>Open video in new tab</a>";
                        
                        Test.Info(videoHtml);
                    }
                }
                else if (!testFailed && Settings?.Ui?.Video == "retain-on-failure")
                {
                    // Delete video if test passed and we only retain on failure
                    if (!string.IsNullOrEmpty(videoPath) && File.Exists(videoPath))
                    {
                        File.Delete(videoPath);
                    }
                }
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Failed to handle video: {ex.Message}");
                Test?.Warning($"Video handling failed: {ex.Message}");
                
                // Ensure cleanup happens even if video handling fails
                if (PageContext != null)
                {
                    try
                    {
                        await PageContext.CleanupAsync();
                    }
                    catch
                    {
                        // Ignore cleanup errors
                    }
                }
            }
        }
        else
        {
            // No video recording, just cleanup normally
            if (PageContext != null)
            {
                try
                {
                    await PageContext.CleanupAsync();
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        _scope?.Dispose();
    }
    protected async Task TakeScreenshot(string name)
    {
        var screenshot = await Page.ScreenshotAsync();
        var screenshotFileName = TestArtifactsManager.GetScreenshotFileName(name, "Manual");
        var screenshotPath = Path.Combine(TestArtifactsManager.ScreenshotsDirectory, screenshotFileName);
        File.WriteAllBytes(screenshotPath, screenshot);
        Test?.AddScreenCaptureFromPath(screenshotPath);
    }

    // Helper method to resolve pages from DI container after PageContext is initialized
    protected T ResolvePage<T>() where T : BasePage
    {
        return _scope?.Resolve<T>() ?? throw new InvalidOperationException("Scope not initialized");
    }
}
