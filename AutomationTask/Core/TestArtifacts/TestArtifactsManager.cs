namespace AutomationTask.Core.TestArtifacts;

/// <summary>
/// Centralized manager for all test artifacts paths (reports, screenshots, videos, traces)
/// </summary>
public static class TestArtifactsManager
{
    private static readonly Lazy<string> _projectRoot = new Lazy<string>(() =>
        Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."))
    );

    private static readonly Lazy<string> _testReportsRoot = new Lazy<string>(() =>
        Path.Combine(_projectRoot.Value, "TestReports")
    );

    /// <summary>
    /// Gets the project root directory
    /// </summary>
    public static string ProjectRoot => _projectRoot.Value;

    /// <summary>
    /// Gets the root directory for all test reports and artifacts
    /// </summary>
    public static string TestReportsRoot => _testReportsRoot.Value;

    /// <summary>
    /// Gets the directory for HTML test reports
    /// </summary>
    public static string ReportsDirectory => TestReportsRoot;

    /// <summary>
    /// Gets the directory for test screenshots
    /// </summary>
    public static string ScreenshotsDirectory => Path.Combine(TestReportsRoot, "screenshots");

    /// <summary>
    /// Gets the directory for test videos
    /// </summary>
    public static string VideosDirectory => Path.Combine(TestReportsRoot, "videos");

    /// <summary>
    /// Gets the directory for test traces
    /// </summary>
    public static string TracesDirectory => Path.Combine(TestReportsRoot, "traces");

    /// <summary>
    /// Ensures all artifact directories exist
    /// </summary>
    public static void EnsureDirectoriesExist()
    {
        Directory.CreateDirectory(ReportsDirectory);
        Directory.CreateDirectory(ScreenshotsDirectory);
        Directory.CreateDirectory(VideosDirectory);
        Directory.CreateDirectory(TracesDirectory);
    }

    /// <summary>
    /// Gets a timestamped filename for a report
    /// </summary>
    public static string GetReportFileName(string baseName = "AutomationReport")
    {
        return $"{baseName}_{DateTime.Now:yyyyMMdd_HHmmss}.html";
    }

    /// <summary>
    /// Gets a timestamped filename for a screenshot
    /// </summary>
    public static string GetScreenshotFileName(string testName, string suffix = "Failed")
    {
        return $"{SanitizeFileName(testName)}_{suffix}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
    }

    /// <summary>
    /// Gets a timestamped filename for a trace
    /// </summary>
    public static string GetTraceFileName(string testName)
    {
        return $"{SanitizeFileName(testName)}_trace_{DateTime.Now:yyyyMMdd_HHmms}.zip";
    }

    /// <summary>
    /// Gets a timestamped filename for a video
    /// </summary>
    public static string GetVideoFileName(string testName, string suffix = "")
    {
        var suffixPart = string.IsNullOrEmpty(suffix) ? "" : $"_{suffix}";
        return $"{SanitizeFileName(testName)}{suffixPart}_{DateTime.Now:yyyyMMdd_HHmmss}.webm";
    }

    /// <summary>
    /// Renames a video file from Playwright's random name to a meaningful name
    /// </summary>
    public static async Task<string?> RenameVideoAsync(string? videoPath, string testName, string suffix = "")
    {
        if (string.IsNullOrEmpty(videoPath) || !File.Exists(videoPath))
        {
            return null;
        }

        try
        {
            var newFileName = GetVideoFileName(testName, suffix);
            var newVideoPath = Path.Combine(VideosDirectory, newFileName);
            
            // Wait a bit to ensure file is fully written
            await Task.Delay(500);
            
            File.Move(videoPath, newVideoPath, overwrite: true);
            return newVideoPath;
        }
        catch
        {
            // If rename fails, return original path
            return videoPath;
        }
    }

    /// <summary>
    /// Sanitizes a filename by removing invalid characters
    /// </summary>
    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
    }
}
