# MERQO VERTEXA — local build script (Windows, .NET 8 SDK required)
# Usage: ./scripts/build.ps1 [-Configuration Release]
param([string]$Configuration = "Release")

$ErrorActionPreference = "Stop"
Set-Location (Split-Path $PSScriptRoot -Parent)

dotnet restore src/MerqoVertexa.sln
dotnet build src/MerqoVertexa.sln -c $Configuration --no-restore
dotnet test src/MerqoVertexa.sln -c $Configuration --no-build

$out = "src/Merqo.Vertexa.App/bin/$Configuration/net8.0-windows"
Write-Host "Build complete. Run: $out/Merqo.Vertexa.App.exe"
