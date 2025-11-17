namespace AutomationTask.Configuration;

public class TestSettings
{
    public string Environment { get; set; } = "dev";
    public UiSettings Ui { get; set; } = new();
    public ApiSettings Api { get; set; } = new();
    public ReportingSettings Reporting { get; set; } = new();
}

public enum ViewportProfile
{
    Custom,
    DesktopFullHD,
    DesktopHD,
    Laptop,
    TabletLandscape,
    TabletPortrait,
    MobileLarge,
    MobileMedium,
    MobileSmall
}

public class ViewportDimensions
{
    public int Width { get; set; }
    public int Height { get; set; }

    public ViewportDimensions(int width, int height)
    {
        Width = width;
        Height = height;
    }
}

public class UiSettings
{
    // Predefined viewport profiles for common devices
    private static readonly Dictionary<ViewportProfile, ViewportDimensions> s_viewportProfiles;

    static UiSettings()
    {
        s_viewportProfiles = new Dictionary<ViewportProfile, ViewportDimensions>
        {
            { ViewportProfile.DesktopFullHD, new ViewportDimensions(1920, 1080) },
            { ViewportProfile.DesktopHD, new ViewportDimensions(1366, 768) },
            { ViewportProfile.Laptop, new ViewportDimensions(1280, 720) },
            { ViewportProfile.TabletLandscape, new ViewportDimensions(1024, 768) },
            { ViewportProfile.TabletPortrait, new ViewportDimensions(768, 1024) },
            { ViewportProfile.MobileLarge, new ViewportDimensions(414, 896) },  // iPhone XR, 11
            { ViewportProfile.MobileMedium, new ViewportDimensions(375, 667) }, // iPhone 6/7/8
            { ViewportProfile.MobileSmall, new ViewportDimensions(320, 568) }   // iPhone SE
        };
    }

    public string BaseUrl { get; set; } = string.Empty;
    public string Browser { get; set; } = "chrome";
    public bool Headless { get; set; } = false;
    public int Timeout { get; set; } = 30000;
    public int SlowMo { get; set; } = 0;
    
    // Profile-based or custom viewport
    public string Profile { get; set; } = "Custom";
    public int ViewportWidth { get; set; } = 1920;
    public int ViewportHeight { get; set; } = 1080;
    
    public string Screenshot { get; set; } = "only-on-failure";
    public string Video { get; set; } = "on";
    public string Trace { get; set; } = "retain-on-failure";

    /// <summary>
    /// Gets the effective viewport size based on profile or custom dimensions
    /// </summary>
    public (int Width, int Height) GetViewportSize()
    {
        // Try to parse the profile string to enum
        if (Enum.TryParse<ViewportProfile>(Profile, true, out var profile) 
            && profile != ViewportProfile.Custom)
        {
            if (s_viewportProfiles.TryGetValue(profile, out var size))
            {
                return (size.Width, size.Height);
            }
        }

        // Fall back to custom dimensions
        return (ViewportWidth, ViewportHeight);
    }
}

public class ApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public int Timeout { get; set; } = 30000;
    public Dictionary<string, string> Headers { get; set; } = new();
}

public class ReportingSettings
{
    public string OutputPath { get; set; } = "TestReports";
    public string ReportTitle { get; set; } = "Test Automation Report";
    public string ReportName { get; set; } = "AutomationReport.html";
}
