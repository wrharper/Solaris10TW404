# IPConverter - Release Instructions

## For Users

### Download and Run

1. Go to the [Releases page](https://github.com/wrharper/Solaris10TW404/releases)
2. Download the latest `IPConverter.exe` file
3. Double-click to run - no installation needed!

The application is a self-contained executable that includes all required dependencies.

### What It Does

IPConverter is a simple Windows application that converts IPv4 addresses to their decimal representation.

- Enter an IP address (e.g., `67.173.216.42`)
- Click "Convert"
- Copy the decimal value to your clipboard

### System Requirements

- Windows 10 or Windows 11 (x64)
- No additional software required (includes .NET runtime)

## For Developers

### Creating a Release

#### Option 1: Use the Helper Script (Recommended)

**On Windows (PowerShell):**
```powershell
.\create-release.ps1 1.0.0
```

**On Linux/Mac (Bash):**
```bash
./create-release.sh 1.0.0
```

#### Option 2: Manual Tag Creation

1. Create and push a version tag:
   ```bash
   git tag -a v1.0.0 -m "Release v1.0.0 - IPConverter standalone executable"
   git push origin v1.0.0
   ```

2. The GitHub Actions workflow will automatically:
   - Build the project on Windows
   - Create a self-contained executable
   - Create a ZIP archive
   - Publish a GitHub Release with both files
   - Add release notes automatically

3. Monitor the build progress:
   - Actions: https://github.com/wrharper/Solaris10TW404/actions
   - Once complete, the release appears at: https://github.com/wrharper/Solaris10TW404/releases

#### Option 3: Manual Workflow Trigger

You can also trigger a build without creating a release by manually running the workflow from the GitHub Actions tab (workflow_dispatch).

### Manual Build

To build locally on Windows:

```bash
dotnet restore IPConverter.sln
dotnet build IPConverter.sln --configuration Release
dotnet publish IPConverter/IPConverter.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

The executable will be in `IPConverter/bin/Release/net10.0-windows/win-x64/publish/`

### Creating the First Release

After this PR is merged to main, create the first release:

```bash
# Switch to main branch
git checkout main
git pull origin main

# Create the v1.0.0 release
git tag -a v1.0.0 -m "Release v1.0.0 - Initial IPConverter standalone executable release"
git push origin v1.0.0
```

Or simply use the provided script:
```bash
./create-release.sh 1.0.0
```
