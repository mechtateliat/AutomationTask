# 🧪 Test Automation Framework

A modern, scalable test automation framework built with C# and .NET 8, featuring both UI and API testing capabilities. This project demonstrates best practices in test automation, including Page Object Model pattern, dependency injection, and comprehensive test reporting.

## 📑 Table of Contents

- [About the Project](#about-the-project)
- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Installation & Setup](#installation-setup)
- [Configuration](#configuration)
- [Security - Managing API Keys](#security-managing-api-keys)
- [Running Tests](#running-tests)
- [Test Reports](#test-reports)
- [Key Features](#key-features)
- [Test Examples](#test-examples)
- [Best Practices](#best-practices-demonstrated)

## 📖 About the Project

This test automation framework provides a robust solution for testing web applications and APIs. It targets two main test scenarios:
- **UI Testing**: Automated testing of [SauceDemo](https://www.saucedemo.com/) e-commerce web application
- **API Testing**: RESTful API testing using [ReqRes](https://reqres.in/) service

The framework follows SOLID principles and implements industry best practices including:
- Page Object Model (POM) pattern with controls hierarchy
- Dependency Injection for better testability and maintainability
- Fluent Assertions for readable test validations
- ExtentReports for detailed HTML test reports
- Screenshot and video capture on test failures

## 🛠️ Technologies Used

### Core Framework
- **.NET 8**
- **C# 12.0** - Modern C# language features
- **NUnit 4.4** - Testing framework

### Test Automation
- **Microsoft Playwright 1.56** - Browser automation for UI tests
- **Playwright API** - HTTP client for API tests

### Supporting Libraries
- **FluentAssertions 8.8** - Expressive assertion library
- **Autofac 8.4** - Dependency injection container
- **ExtentReports 5.0** - HTML test reporting
- **Microsoft.Extensions.Configuration 10.0** - Configuration management

### Testing Features
- Multi-browser support (Chrome, Chromium, Firefox, WebKit)
- Parallel test execution
- Automatic screenshots and videos on failures
- Test categorization and filtering

## 📁 Project Structure

```
AutomationTask/
│
├── Configuration/              # Configuration classes and settings
│   ├── ConfigurationManager.cs
│   └── TestSettings.cs
│
├── Core/                      # Core framework components
│   ├── API/                   # API client implementations
│   │   ├── ApiClient.cs
│   │   └── ApiUserClient.cs
│   ├── DI/                    # Dependency injection setup
│   │   └── TestContainerBuilder.cs
│   ├── Reporting/             # Test reporting infrastructure
│   │   └── ReportManager.cs
│   ├── TestArtifacts/         # Screenshots, videos management
│   │   └── TestArtifactsManager.cs
│   └── UI/                    # Browser and page context
│       ├── BrowserFactory.cs
│       └── PageContext.cs
│
├── Extensions/                 # Extension methods
│   └── LocatorExtensions.cs
│
├── Helpers/                    # Utility helpers
│   ├── PriceHelper.cs
│   └── UserApiHelper.cs
│
├── Models/                     # Data models
│   └── UserResponse.cs
│
├── Pages/                     # Page Object Model (POM)
│   ├── BasePage.cs            # Base page class
│   ├── BaseControl.cs         # Base control class
│   ├── Cart/                  # Cart page and controls
│   │   ├── CartPage.cs
│   │   └── Controls/
│   │       ├── CartItemControl.cs
│   │       └── CartListControl.cs
│   ├── Checkout/              # Checkout pages
│   │   ├── CheckoutInfoPage.cs
│   │   ├── CheckoutSummaryPage.cs
│   │   └── CheckoutComplete.cs
│   ├── Controls/              # Shared controls
│   │   ├── HeaderControl.cs
│   │   └── InventoryItemControl.cs
│   ├── Login/                 # Login page and controls
│   │   ├── LoginPage.cs
│   │   └── Controls/
│   │       ├── CredentialsControl.cs
│   │       └── PasswordControl.cs
│   └── Products/              # Products page and controls
│       ├── ProductsPage.cs
│       └── Controls/
│           ├── ProductItemControl.cs
│           ├── ProductsGridControl.cs
│           ├── ProductSortControl.cs
│           └── SideBarMenuControl.cs
│
├── Tests/                     # Test classes
│   ├── TestBase.cs            # Base test class
│   ├── API/                   # API tests
│   │   ├── ApiTestBase.cs
│   │   ├── ApiTestSetup.cs
│   │   └── UserApiTests.cs
│   └── UI/                    # UI tests
│       ├── UiTestBase.cs
│       ├── UiTestSetup.cs
│       ├── CheckoutTests.cs
│       └── SortingTests.cs
│
├── appsettings.json            # Default configuration
├── appsettings.*.json          # Environment-specific configs
└── AutomationTask.csproj       # Project file
```

## 📜 Prerequisites

Before you begin, ensure you have the following installed:

- **.NET 8 SDK** or higher
  - Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
  - Verify installation: `dotnet --version`

- **PowerShell** (for Playwright browser installation)
  - Windows: Built-in
  - macOS/Linux: Install [PowerShell Core](https://github.com/PowerShell/PowerShell)

- **IDE** (Optional but recommended)
  - Visual Studio 2022 (Community or higher)
  - Visual Studio Code with C# extension
  - JetBrains Rider

## 🚀 Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/mechtateliat/AutomationTask.git
cd AutomationTask
```

**Note:** After cloning, you'll be in the **repository root**. The project files are in the `AutomationTask/` subdirectory.

### 2. Restore NuGet Packages

```bash
dotnet restore
```

### 3. Build the Project

```bash
dotnet build
```

### 4. Configure API Key (Required for API Tests)

**Important:** You must configure your API key before running API tests.

#### Step 1: Get Your API Key

Visit [https://reqres.in/signup](https://reqres.in/signup) to obtain a free API key for testing purposes.

#### Step 2: Set Up User Secrets

```bash
# Navigate to the project directory (from repository root)
cd AutomationTask

# Set your API key using User Secrets (replace with your actual key)
dotnet user-secrets set "API:Headers:x-api-key" "your-api-key-from-reqres"

# Verify it was set correctly
dotnet user-secrets list

# Navigate back to the repository root
cd ..
```

💡 **Why User Secrets?** This stores your API key securely outside the project directory and works on all platforms (Windows, macOS, Linux). Your key will never be committed to Git. See the [Security - Managing API Keys](#security-managing-api-keys) section for more details.

### 5. Install Playwright Browsers

Playwright requires browsers to be installed separately.

**Using the provided script (works from repository root or project directory):**
```powershell
# From repository root:
.\AutomationTask\install-browsers.ps1

# OR from AutomationTask directory:
cd AutomationTask
.\install-browsers.ps1
```

**Or manually install browsers:**

After building the project, run from repository root:

```bash
pwsh AutomationTask/bin/Debug/net8.0/playwright.ps1 install
```

Or install specific browsers:

```bash
pwsh AutomationTask/bin/Debug/net8.0/playwright.ps1 install chrome
pwsh AutomationTask/bin/Debug/net8.0/playwright.ps1 install chromium
pwsh AutomationTask/bin/Debug/net8.0/playwright.ps1 install firefox
pwsh AutomationTask/bin/Debug/net8.0/playwright.ps1 install webkit
```

This will download Chrome, Chromium, Firefox, and WebKit browsers (~400MB).

### 6. Verify Setup

Run a quick test to verify everything is configured correctly (from repository root):

```bash
# Run API tests to verify User Secrets are working
dotnet test --filter "Category=API" --logger "console;verbosity=normal"

# Run UI tests to verify Playwright browsers are installed
dotnet test --filter "Category=UI" --logger "console;verbosity=normal"
```

If tests pass, you're all set! 🎉

## ⚙️ Configuration

### Configuration Files

The framework uses `appsettings.json` for configuration. You can create environment-specific configuration files:

- `appsettings.json` - Default configuration
- `appsettings.dev.json` - Development environment
- `appsettings.testing.json` - Testing environment
- `appsettings.staging.json` - Staging environment
- `appsettings.production.json` - Production environment

### Setting the Test Environment

Use the `TEST_ENVIRONMENT` environment variable:

**Windows (PowerShell):**
```powershell
$env:TEST_ENVIRONMENT = "dev"

# Verify it was set
echo $env:TEST_ENVIRONMENT
```

**Windows (Command Prompt):**
```cmd
set TEST_ENVIRONMENT=dev

# Verify it was set
echo %TEST_ENVIRONMENT%
```

**macOS/Linux:**
```bash
export TEST_ENVIRONMENT=dev

# Verify it was set
echo $TEST_ENVIRONMENT
```

### Key Configuration Options

#### UI Settings (`appsettings.json`)

```json
{
  "Ui": {
    "BaseUrl": "https://www.saucedemo.com/",
    "Browser": "chrome",              // chrome (default), chromium, firefox, or webkit
    "Headless": false,                // true for CI/CD pipelines
    "Timeout": 30000,                 // Default timeout in ms
    "SlowMo": 0,                      // Slow down operations (ms)
    "Profile": "Custom",              // Predefined device profile or "Custom"
    "ViewportWidth": 1920,            // Browser width (used when Profile is "Custom")
    "ViewportHeight": 1080,           // Browser height (used when Profile is "Custom")
    "Screenshot": "only-on-failure",  // Screenshot capture mode
    "Video": "on",                    // Video recording
    "Trace": "retain-on-failure"      // Playwright trace
  }
}
```

#### Viewport Configuration - Multiple Resolution Support

The framework supports testing with different browser resolutions for responsive design testing:

**Option 1: Predefined Device Profiles**

Use predefined profiles in `appsettings.json`:

```json
{
  "Ui": {
    "Profile": "DesktopFullHD"  // Use a predefined profile
  }
}
```

**Available profiles:**
- `DesktopFullHD` - 1920 x 1080 (Full HD Desktop)
- `DesktopHD` - 1366 x 768 (Standard HD Desktop)
- `Laptop` - 1280 x 720 (Laptop)
- `TabletLandscape` - 1024 x 768 (Tablet in landscape)
- `TabletPortrait` - 768 x 1024 (Tablet in portrait)
- `MobileLarge` - 414 x 896 (iPhone XR, 11)
- `MobileMedium` - 375 x 667 (iPhone 6/7/8)
- `MobileSmall` - 320 x 568 (iPhone SE)
- `Custom` - Use ViewportWidth and ViewportHeight values

**Option 2: Custom Dimensions**

Set custom dimensions in `appsettings.json`:

```json
{
  "Ui": {
    "Profile": "Custom",
    "ViewportWidth": 1440,
    "ViewportHeight": 900
  }
}
```

**Option 3: Environment Variable Overrides**

Override viewport settings at runtime using environment variables:

**Windows (PowerShell):**
```powershell
# Use a predefined profile
$env:UI__Profile = "TabletLandscape"
dotnet test

# Use custom dimensions
$env:UI__Profile = "Custom"
$env:UI__ViewportWidth = "1440"
$env:UI__ViewportHeight = "900"
dotnet test
```

**macOS/Linux:**
```bash
# Use a predefined profile
export UI__Profile="MobileLarge"
dotnet test

# Use custom dimensions
export UI__Profile="Custom"
export UI__ViewportWidth="1440"
export UI__ViewportHeight="900"
dotnet test
```

**CI/CD Example:**
```yaml
- name: Run tests on mobile resolution
  run: dotnet test
  env:
    UI__Profile: MobileLarge

- name: Run tests on desktop resolution
  run: dotnet test
  env:
    UI__Profile: DesktopFullHD
```

This allows you to:
- Test the same scenarios across multiple device sizes
- Run parallel test jobs in CI/CD with different resolutions
- Quickly switch between devices without changing configuration files

#### API Settings

```json
{
  "Api": {
    "BaseUrl": "https://reqres.in/api/",
    "Timeout": 30000,
    "Headers": {
      "x-api-key": "your-api-key-here"
    }
  }
}
```

⚠️ **Security Note:** Never commit actual API keys to `appsettings.json`. See the [Security - Managing API Keys](#security-managing-api-keys) section below.

#### Reporting Settings

```json
{
  "Reporting": {
    "OutputPath": "TestReports",
    "ReportTitle": "Test Automation Report",
    "ReportName": "AutomationReport.html"
  }
}
```

## 🔐 Security - Managing API Keys

**Important:** Never commit sensitive data like API keys directly to your repository!

### 🏠 Local Development - User Secrets (Cross-Platform)

**User Secrets** work on **all platforms** (Windows, macOS, Linux) and store sensitive data outside your project directory.

#### Quick Setup:

```bash
# From repository root, navigate to the project directory
cd AutomationTask

# Set your API key securely (get your key from https://reqres.in/signup)
dotnet user-secrets set "API:Headers:x-api-key" "your-api-key-from-reqres"

# Verify it was set
dotnet user-secrets list

# Navigate back to repository root
cd ..

# Run tests to verify
dotnet test --filter "Category=API"
```

**Where secrets are stored:**
- Windows: `%APPDATA%\Microsoft\UserSecrets\automationtask-secrets-2024\`
- macOS/Linux: `~/.microsoft/usersecrets/automationtask-secrets-2024/`

### 🔧 CI/CD - Environment Variables

For GitHub Actions (already configured in `.github/workflows/dotnet.yml`):

1. Go to: **Repository → Settings → Secrets and variables → Actions**
2. Click **"New repository secret"**
3. Name: `API_KEY`
4. Value: Your API key from [https://reqres.in/signup](https://reqres.in/signup)

The workflow uses: `API__Headers__x-api-key: ${{ secrets.API_KEY }}`

### Configuration Priority

Settings are loaded in this order (later overrides earlier):

1. `appsettings.json` → Base configuration (placeholders)
2. `appsettings.{Environment}.json` → Environment-specific
3. **User Secrets** → Local development secrets
4. **Environment Variables** → CI/CD secrets (highest priority)

## 🧪 Running Tests

### Using PowerShell Script (Recommended)

The framework includes a convenient PowerShell script for running tests with environment configuration. The script works from both **repository root** and **project directory**.

**From repository root:**

```powershell
# Run all tests with default settings
.\AutomationTask\run-tests.ps1

# Run tests in specific environment
.\AutomationTask\run-tests.ps1 -Environment "staging"

# Run tests with category filter
.\AutomationTask\run-tests.ps1 -Filter "Category=Smoke"

# Combine environment and filter
.\AutomationTask\run-tests.ps1 -Environment "testing" -Filter "Category=API"

# Run specific browser tests
.\AutomationTask\run-tests.ps1 -Browser "firefox" -Filter "Category=UI"
```

**From AutomationTask directory:**

```powershell
cd AutomationTask
.\run-tests.ps1 -Filter "Category=API"
```

**Script Parameters:**
- `-Environment`: Target environment (dev, testing, staging, production) - Default: "dev"
- `-Filter`: NUnit test filter expression
- `-Browser`: Browser to use (chrome, chromium, firefox, webkit) - Default: "chrome"

### Using .NET CLI

All commands should be run from the **repository root**.

#### Run All Tests

```bash
dotnet test
```

#### Run Tests by Category

The framework uses NUnit categories for test organization:

**By Test Type:**
```bash
# Run only UI tests
dotnet test --filter "Category=UI"

# Run only API tests
dotnet test --filter "Category=API"
```

**By Priority:**
```bash
# Run high priority tests
dotnet test --filter "Category=HighPriority"

# Run smoke tests
dotnet test --filter "Category=Smoke"
```

**By Feature:**
```bash
# Run checkout tests
dotnet test --filter "Category=Checkout"

# Run sorting tests
dotnet test --filter "Category=Sorting"

# Run user API tests
dotnet test --filter "Category=Users"
```

#### Run Specific Test

```bash
# By test name
dotnet test --filter "FullyQualifiedName~SuccessfulCheckoutWithProducts"

# By test class
dotnet test --filter "FullyQualifiedName~CheckoutTests"
```

#### Combined Filters

```bash
# High priority UI tests
dotnet test --filter "Category=UI&Category=HighPriority"

# Smoke tests only
dotnet test --filter "Category=Smoke"
```

#### Run Tests with Detailed Output

```bash
dotnet test --logger "console;verbosity=detailed"
```

### Common Test Scenarios

**Run smoke tests in headless mode:**
1. Update `AutomationTask/appsettings.json` → set `"Headless": true`
2. Run: `dotnet test --filter "Category=Smoke"`

**Run UI tests in Firefox:**
1. Update `AutomationTask/appsettings.json` → set `"Browser": "firefox"`
2. Run: `dotnet test --filter "Category=UI"`

**Run specific test method:**
```bash
dotnet test --filter "FullyQualifiedName~ProductSorting_ByPrice_HighToLow"
```

## 📊 Test Reports

### HTML Reports

After test execution, find the HTML report at:
```
AutomationTask/TestReports/AutomationReport.html
```

The report includes:
- Test execution summary (passed/failed/skipped)
- Detailed test steps with timestamps
- Screenshots for failed tests
- Execution time per test
- Test categorization

Open the report in any web browser to view the results.

### Console Output

Console output includes:
- Test execution progress
- Pass/Fail status
- Execution time
- Error messages and stack traces

### Test Artifacts

Failed tests automatically capture:
- **Screenshots**: `TestReports/screenshots/`
- **Videos**: `TestReports/videos/`
- **Traces**: `TestReports/traces/` (Playwright traces)

## ✅ Key Features

### 📄 Page Object Model (POM)
- **Three-layer architecture**: Pages → Controls → Elements
- **BasePage** and **BaseControl** for code reusability
- **Strongly-typed page methods** for better IntelliSense

### 💉 Dependency Injection
- Autofac container for UI tests
- Automatic page registration and resolution
- Scoped lifetime per test

### ✔️ Fluent Assertions
- Readable and expressive test assertions
- Detailed failure messages
- Example: `.Should().Be(expected, "because reason")`

### 📝 ExtentReports Integration
- Beautiful HTML reports
- Hierarchical test structure
- Screenshots and videos embedded in reports
- Custom logging (LogInfo, LogPass)

### 🌐 Multi-Browser Support
- Chrome (default - uses installed Google Chrome)
- Chromium (Playwright's bundled Chromium)
- Firefox
- WebKit (Safari)
- Configurable via settings

### 📸 Automatic Artifacts
- Screenshots on test failure
- Video recording of test execution
- Playwright traces for debugging

## 🔍 Test Examples

### UI Test: Checkout Flow
```csharp
[Test]
[Category("Smoke")]
[Category("Checkout")]
public async Task SuccessfulCheckoutWithProducts()
{
    // Login
    await LoginPage.NavigateAsync();
    await LoginPage.LoginAsync(username, password);
    
    // Add products
    await ProductsPage.AddProductToCartByIndex(0);
    
    // Checkout
    await CartPage.CheckoutAsync();
    await CheckoutInfoPage.EnterCheckoutInformationAsync("John", "Doe", "12345");
    await CheckoutSummaryPage.FinishCheckoutAsync();
    
    // Verify
    var message = await CheckoutCompletePage.GetCompleteHeaderTextAsync();
    message.Should().Be("Thank you for your order!");
}
```

### API Test: Get Users
```csharp
[Test]
[Category("Smoke")]
[Category("API")]
public async Task GetUsers_FromPage()
{
    // Act
    var response = await ApiUserClient.GetUsersAsync(page: 1);
    
    // Assert
    response.Should().NotBeNull();
    response.Page.Should().Be(1);
    response.Data.Should().NotBeEmpty();
}
```

## 📋 Best Practices Demonstrated

1. **Separation of Concerns**: Tests, Pages, Configuration are clearly separated
2. **DRY Principle**: Base classes eliminate code duplication
3. **Single Responsibility**: Each class has one clear purpose
4. **Async/Await**: Proper asynchronous programming throughout
5. **Meaningful Naming**: Descriptive names for tests and methods
6. **.NET Naming Conventions**: 
   - PascalCase for 3+ letter acronyms (e.g., `Api`, `Ui`, `Html`)
   - Private fields use underscore prefix (e.g., `_settings`, `_apiClient`)
   - Extension classes end with "Extensions" suffix
7. **Test Independence**: Tests don't depend on each other
8. **Logging**: Comprehensive logging for debugging
9. **Assertions with Context**: Each assertion includes a reason

## 📝 Additional Notes

- **Test Data**: Uses built-in credentials from the SauceDemo application
- **Test Stability**: Implements proper waits and synchronization
- **Maintenance**: Centralized locators in page objects for easy updates
- **Scalability**: Easy to add new pages, tests, and features

---

**Happy Testing! 🥳**

