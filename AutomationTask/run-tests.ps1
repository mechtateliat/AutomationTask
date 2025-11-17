# Test Execution Script
# This script can be run from either the repository root or the AutomationTask directory

param(
    [string]$Environment = "dev",
    [string]$Filter = "",
    [string]$Browser = "chrome"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Test Automation Execution" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Environment: $Environment" -ForegroundColor Yellow
Write-Host "Browser: $Browser" -ForegroundColor Yellow
if ($Filter) {
    Write-Host "Filter: $Filter" -ForegroundColor Yellow
}
Write-Host "========================================`n" -ForegroundColor Cyan

# Set environment variables
$env:TEST_ENVIRONMENT = $Environment
$env:UI__Browser = $Browser

# Determine the correct path based on current directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectDir = if (Test-Path "$scriptDir\AutomationTask.csproj") { $scriptDir } else { "$scriptDir\AutomationTask" }
$repoRoot = if (Test-Path "$scriptDir\.git") { $scriptDir } else { Split-Path -Parent $scriptDir }

# Change to repository root for dotnet commands
Push-Location $repoRoot

# Build the project
Write-Host "Building project..." -ForegroundColor Green
dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    Pop-Location
    exit 1
}

# Run tests
Write-Host "`nRunning tests..." -ForegroundColor Green

if ($Filter) {
    dotnet test --filter $Filter --logger "console;verbosity=detailed"
} else {
    dotnet test --logger "console;verbosity=detailed"
}

# Check results
if ($LASTEXITCODE -eq 0) {
    Write-Host "`n========================================" -ForegroundColor Green
    Write-Host "Tests completed successfully!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
} else {
    Write-Host "`n========================================" -ForegroundColor Red
    Write-Host "Tests failed!" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
}

Write-Host "`nTest report generated at: AutomationTask/TestReports/AutomationReport.html" -ForegroundColor Cyan

Pop-Location

# Examples of usage:
# .\run-tests.ps1
# .\run-tests.ps1 -Environment "staging"
# .\run-tests.ps1 -Filter "Category=Smoke"
# .\run-tests.ps1 -Environment "testing" -Filter "Category=API"
