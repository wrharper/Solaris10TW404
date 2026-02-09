# Script to create a new release for IPConverter
# Usage: .\create-release.ps1 [version]
# Example: .\create-release.ps1 1.0.0

param(
    [Parameter(Mandatory=$true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"

$Tag = "v$Version"

Write-Host "Creating release $Tag..." -ForegroundColor Cyan
Write-Host ""

# Check if tag already exists
$tagExists = git tag -l $Tag
if ($tagExists) {
    Write-Host "Error: Tag $Tag already exists!" -ForegroundColor Red
    exit 1
}

# Create and push the tag
git tag -a $Tag -m "Release $Tag - IPConverter standalone executable"
Write-Host "Created tag: $Tag" -ForegroundColor Green

Write-Host ""
Write-Host "Pushing tag to GitHub..." -ForegroundColor Cyan
git push origin $Tag

Write-Host ""
Write-Host "✓ Success! The GitHub Actions workflow will now build and publish the release." -ForegroundColor Green
Write-Host "  Check the progress at: https://github.com/wrharper/Solaris10TW404/actions"
Write-Host "  The release will be available at: https://github.com/wrharper/Solaris10TW404/releases"
