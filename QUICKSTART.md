# Quick Start: Creating Your First Release

## After Merging This PR

Once this PR is merged to the main branch, follow these simple steps to create the first downloadable release:

### Step 1: Update Your Local Repository
```bash
git checkout main
git pull origin main
```

### Step 2: Create the Release

**Easy Way (using the helper script):**

On Windows (PowerShell):
```powershell
.\create-release.ps1 1.0.0
```

On Linux/Mac/Git Bash:
```bash
./create-release.sh 1.0.0
```

**Manual Way:**
```bash
git tag -a v1.0.0 -m "Release v1.0.0 - Initial IPConverter standalone executable release"
git push origin v1.0.0
```

### Step 3: Monitor the Build

1. Go to: https://github.com/wrharper/Solaris10TW404/actions
2. You'll see the "Build and Release IPConverter" workflow running
3. Wait for it to complete (typically 2-5 minutes)

### Step 4: Download and Test

1. Go to: https://github.com/wrharper/Solaris10TW404/releases
2. You'll see "IPConverter v1.0.0" with:
   - `IPConverter.exe` - standalone executable
   - `IPConverter-win-x64.zip` - zip archive
3. Download `IPConverter.exe` and test it!

## What Happens Automatically

The GitHub Actions workflow will:
- ✅ Build the project on a Windows runner
- ✅ Create a self-contained .exe (no .NET installation required)
- ✅ Create a .zip archive
- ✅ Publish a GitHub Release with download links
- ✅ Add formatted release notes

## Troubleshooting

**If the workflow doesn't trigger:**
- Make sure the tag starts with 'v' (e.g., v1.0.0, not 1.0.0)
- Check that the workflow file exists in `.github/workflows/release.yml`

**If the build fails:**
- Check the Actions tab for error messages
- Common issues: .NET SDK version mismatch, missing dependencies

**Manual trigger:**
You can also manually trigger the workflow from the Actions tab using the "workflow_dispatch" option.
