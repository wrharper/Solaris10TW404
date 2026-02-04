# How to Create a GitHub Release

This guide explains how to create a GitHub release using the included release package.

## Quick Steps

1. **Navigate to Releases**
   - Go to https://github.com/wrharper/Solaris10TW404
   - Click on "Releases" in the right sidebar
   - Click the "Create a new release" button

2. **Choose a Tag**
   - Click "Choose a tag"
   - Type: `v1.0.0` (or your preferred version number)
   - Click "Create new tag: v1.0.0 on publish"

3. **Set Release Title**
   - Title: `Initial Release v1.0.0` (or your preferred title)

4. **Add Release Description**
   - Copy the content from `RELEASE_NOTES.md` into the description field
   - Or write your own custom description

5. **Upload the Release Asset**
   - Click "Attach binaries by dropping them here or selecting them"
   - Upload the file: `Solaris10TW404-Release.zip`
   - The file should appear in the "Assets" section

6. **Publish**
   - Review everything looks correct
   - Click "Publish release" button

## What the Release Package Contains

The `Solaris10TW404-Release.zip` file includes:

- **IPConverter Application** - Ready-to-run Windows executable
  - IPConverter.exe and required dependencies
  - Converts IP addresses to decimal format
  - Uses the corrected formula: `(d × 256³) + (c × 256²) + (b × 256) + a`

- **Documentation** - Complete setup guide
  - README.md with full Solaris 10 TalesWeaver server setup instructions
  - README.txt with quick start guide for the IPConverter tool

- **Source Code** - Full source for IPConverter
  - All XAML and C# files
  - Project and solution files
  - Can be built with Visual Studio 2022 + .NET 10.0 SDK

## After Publishing

Once the release is published:

1. Users can download the zip file from the Releases page
2. The release will be visible on the repository main page
3. Users can extract and run IPConverter.exe on Windows
4. The release will be tagged with the version number you chose

## Release Asset Details

- **Filename**: Solaris10TW404-Release.zip
- **Size**: 143 KB
- **Format**: ZIP archive
- **Platform**: Windows (for IPConverter), Documentation is platform-agnostic

## Additional Notes

- This is a binary + source release
- The IPConverter executable is built for Windows with .NET 10.0
- Users need .NET 10.0 runtime to run the executable
- Source code is included for users who want to build from source

## Future Releases

For subsequent releases:
1. Update the version number (e.g., v1.1.0, v2.0.0)
2. Update RELEASE_NOTES.md with new changes
3. Rebuild the release package if needed
4. Follow the same steps above with the new version number
