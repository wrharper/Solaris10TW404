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

To create a new release:

1. Create and push a version tag:
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```

2. The GitHub Actions workflow will automatically:
   - Build the project
   - Create a self-contained executable
   - Create a ZIP archive
   - Publish a GitHub Release with both files

### Manual Build

To build locally on Windows:

```bash
dotnet restore IPConverter.sln
dotnet build IPConverter.sln --configuration Release
dotnet publish IPConverter/IPConverter.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

The executable will be in `IPConverter/bin/Release/net10.0-windows/win-x64/publish/`
