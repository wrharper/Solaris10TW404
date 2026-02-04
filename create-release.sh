#!/bin/bash
# Script to create a new release for IPConverter
# Usage: ./create-release.sh [version]
# Example: ./create-release.sh 1.0.0

set -e

if [ -z "$1" ]; then
  echo "Usage: ./create-release.sh [version]"
  echo "Example: ./create-release.sh 1.0.0"
  exit 1
fi

VERSION=$1
TAG="v${VERSION}"

echo "Creating release ${TAG}..."
echo ""

# Check if tag already exists
if git rev-parse "$TAG" >/dev/null 2>&1; then
  echo "Error: Tag ${TAG} already exists!"
  exit 1
fi

# Create and push the tag
git tag -a "$TAG" -m "Release ${TAG} - IPConverter standalone executable"
echo "Created tag: ${TAG}"

echo ""
echo "Pushing tag to GitHub..."
git push origin "$TAG"

echo ""
echo "✓ Success! The GitHub Actions workflow will now build and publish the release."
echo "  Check the progress at: https://github.com/wrharper/Solaris10TW404/actions"
echo "  The release will be available at: https://github.com/wrharper/Solaris10TW404/releases"
