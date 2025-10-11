# TextAnalytics Solution - Documentation

This directory contains all project documentation and information files.

## Directory Structure

```
docs/
├── README.md                    # This file - documentation index
├── VERSION_MANAGEMENT.md        # Guide to version management and SemVer
└── updates/                     # Package update history logs
    └── 2025-10-11_Package_Update_Log.md
```

## Documentation Files

### VERSION_MANAGEMENT.md
**Purpose**: Comprehensive guide on package version management

**Contents**:
- Semantic Versioning (SemVer) explanation
- Impact of version updates on API compatibility
- Practical dotnet CLI commands
- Update strategies (Conservative, Balanced, Aggressive)
- Best practices for production environments
- Transitive dependencies explanation

**When to read**: Before updating any NuGet packages in the solution

### updates/ Directory
**Purpose**: Historical log of all package updates performed

**Naming convention**: `YYYY-MM-DD_Package_Update_Log.md`

**Contents**: Each update log includes:
- Date and summary of updates
- List of updated packages with version changes
- Update type (PATCH/MINOR/MAJOR)
- Commands used
- Verification results (build & tests)
- Impact analysis and risk assessment

## Quick Reference

### Common Commands

```bash
# Check for outdated packages
dotnet list <project>.csproj package --outdated

# Update a package
dotnet add <project>.csproj package <PackageName> --version <Version>

# Build solution
dotnet build

# Run tests
dotnet test
```

### When to Update

- **PATCH** (X.X.1 → X.X.2): Safe to apply immediately
- **MINOR** (X.1.0 → X.2.0): Test before applying
- **MAJOR** (1.0.0 → 2.0.0): Plan carefully, may require code changes

## Contributing to Documentation

When making significant changes to the project:

1. **Package Updates**: Create a new update log in `docs/updates/` with current date
2. **New Features**: Document in relevant .md files
3. **Breaking Changes**: Update VERSION_MANAGEMENT.md if it affects the guide

## Related Documentation

- [.NET Package Management](https://docs.microsoft.com/en-us/nuget/)
- [Semantic Versioning](https://semver.org/)
- [Project README](../README.md) - Main project documentation
