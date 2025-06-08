<#
.SYNOPSIS
    Downloads and installs the latest release of the GateEntry application from GitHub.
.DESCRIPTION
    This script automates the update process for the GateEntry application.
    It performs the following steps:
    1. Fetches the URL of the latest .zip release from the specified GitHub repository.
    2. Stops any currently running GateEntry.exe process.
    3. Clears the contents of the installation directory.
    4. Downloads and extracts the new version into the directory.
    5. Cleans up the downloaded archive.
    6. Starts the newly installed GateEntry.exe.
.NOTES
    Author: Gemini
    Version: 1.0
    Requires: PowerShell 5.1 or later. Must be run as an Administrator.
#>

# --- Configuration ---
$githubRepo = "davojc/gate-entry"
$processName = "GateEntry"
$installDir = "C:\Program Files\GateEntry"
$exeName = "GateEntry.exe"
$zipFileName = "gateentry.zip"
# ---------------------

# Step 1: Check for Administrator privileges
Write-Host "Checking for Administrator privileges..." -ForegroundColor Yellow
if (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Error "This script must be run as an Administrator to stop services and write to Program Files. Please re-run from an elevated PowerShell terminal."
    # Pause to allow the user to read the error before the window closes
    Read-Host "Press Enter to exit"
    exit 1
}
Write-Host "Administrator privileges confirmed." -ForegroundColor Green

try {
    # Step 2: Get the latest release information from GitHub
    Write-Host "Fetching latest release from GitHub repository: $githubRepo" -ForegroundColor Yellow

    $repoPath = "https://github.com/davojc/gate-entry/releases/latest/download"
    $downloadUrl = "$repoPath/$zipFileName"

    function Download-File {
        param (
            [string]$url,
            [string]$destinationPath
        )
        Write-Output "Downloading file: $url"
        Invoke-WebRequest -Uri $url -OutFile $destinationPath
    }

    # Step 3: Download and extract the release
    $tempZipPath = Join-Path $env:TEMP $zipFileName
    Write-Host "Downloading $zipFileName to $tempZipPath..." -ForegroundColor Yellow
    Invoke-WebRequest -Uri $downloadUrl -OutFile $tempZipPath

    # Step 4: Stop the running process if it exists
    Write-Host "Checking for running process: $processName" -ForegroundColor Yellow
    $runningProcess = Get-Process -Name $processName -ErrorAction SilentlyContinue
    if ($runningProcess) {
        Write-Host "Process found. Stopping $processName..."
        Stop-Process -Name $processName -Force
        Write-Host "$processName stopped successfully." -ForegroundColor Green
    } else {
        Write-Host "Process is not currently running."
    }

    # Step 4: Prepare the installation directory
    Write-Host "Preparing installation directory: $installDir" -ForegroundColor Yellow
    if (Test-Path $installDir) {
        Write-Host "Directory exists. Clearing all contents..."
        Get-ChildItem -Path $installDir | Remove-Item -Recurse -Force
    } else {
        Write-Host "Directory does not exist. Creating..."
        New-Item -Path $installDir -ItemType Directory -Force | Out-Null
    }
    Write-Host "Directory is ready." -ForegroundColor Green

    Write-Host "Download complete. Extracting archive to $installDir..."
    Expand-Archive -Path $tempZipPath -DestinationPath $installDir -Force
    Write-Host "Extraction complete." -ForegroundColor Green

    # Step 6: Clean up the downloaded zip file
    Write-Host "Cleaning up temporary files..." -ForegroundColor Yellow
    Remove-Item -Path $tempZipPath -Force
    Write-Host "Cleanup complete." -ForegroundColor Green
    
    # Step 7: Start the new application
    $exePath = Join-Path $installDir $exeName
    Write-Host "Starting the new application from $exePath..." -ForegroundColor Yellow
    if (Test-Path $exePath) {
        Start-Process -FilePath $exePath
        Write-Host "GateEntry started successfully!" -ForegroundColor Cyan
    } else {
        throw "Could not find $exeName in the installation directory after extraction."
    }

}
catch {
    Write-Error "An error occurred during installation: $_"
    # Pause to allow the user to read the error before the window closes
    Read-Host "Press Enter to exit"
    exit 1
}

Read-Host "Installation script finished. Press Enter to exit."