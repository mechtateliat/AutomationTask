# Install Playwright Browsers Script
# This script can be run from either the repository root or the AutomationTask directory

Write-Host "Installing Playwright browsers..." -ForegroundColor Green

# Determine the correct path based on current directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectDir = if (Test-Path "$scriptDir\AutomationTask.csproj") { $scriptDir } else { "$scriptDir\AutomationTask" }

# Build the project first
Push-Location $projectDir
dotnet build

# Install Playwright browsers
$playwrightPath = "bin/Debug/net8.0/playwright.ps1"

if (Test-Path $playwrightPath) {
    Write-Host "Installing Chrome, Chromium, Firefox, and WebKit browsers..." -ForegroundColor Yellow
    pwsh $playwrightPath install chrome
    pwsh $playwrightPath install chromium
    pwsh $playwrightPath install firefox
    pwsh $playwrightPath install webkit
    Write-Host "Playwright browsers installed successfully!" -ForegroundColor Green
} else {
    Write-Host "Error: Playwright script not found. Make sure the project is built." -ForegroundColor Red
    Write-Host "Path checked: $playwrightPath" -ForegroundColor Yellow
    Pop-Location
    exit 1
}

Pop-Location
Write-Host "Done!" -ForegroundColor Green
