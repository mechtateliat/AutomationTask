using AventStack.ExtentReports;

namespace AutomationTask.Tests;

public abstract class TestBase
{
    protected ExtentTest? Test;

    protected void LogInfo(string message)
    {
        Test?.Info(message);
    }

    protected void LogPass(string message)
    {
        Test?.Pass(message);
    }
}
