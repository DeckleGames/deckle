# Deckle

A modern tool for designing and managing tabletop game components. Create cards, dice, and other game components with ease.

## What is Deckle?

Deckle helps board game designers and hobbyists create professional-looking game components. Whether you're prototyping a new game or preparing files for print-on-demand services, Deckle streamlines the design process.

### Features

- **Project Management**: Organize your game components into projects
- **Card Designer**: Create custom cards with configurable sizes and layouts
- **Dice Designer**: Design custom dice with various face configurations
- **Data Integration**: Connect to Google Sheets and other data sources
- **Export Ready**: Generate print-ready files for your game components

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- Visual Studio 2022 or [Visual Studio Code](https://code.visualstudio.com/) with C# Dev Kit

### Running Deckle

1. Clone the repository
2. Navigate to the solution root directory
3. Run the application:

```bash
aspire run
```

The AppHost will start the Aspire application and launch the dashboard in your browser.

Note: First run may require:

```bash
dotnet restore
dotnet build
```

## Versioning

Deckle uses [GitVersion](https://gitversion.net/) for automatic semantic versioning. We follow **GitHubFlow**: master is always releasable, features branch off and PR back.

### How It Works

Every merge to master automatically gets a version number following [Semantic Versioning](https://semver.org/):

- **MAJOR** (breaking changes): Add `+semver: major` to commit message
- **MINOR** (new features): Add `+semver: minor` to commit message
- **PATCH** (bug fixes): Default, or add `+semver: patch`

### Workflow

```bash
# 1. Create a feature branch
git checkout -b feature/new-component

# 2. Make changes and commit
git commit -m "Add player mat component +semver: minor"

# 3. Push and create PR
git push origin feature/new-component

# 4. Merge PR to master
# GitHub Actions automatically:
#   - Calculates version (e.g., 1.3.0)
#   - Tags Docker images
#   - Creates Git tag (v1.3.0)
```

### Docker Image Tags

Every master build creates images tagged with:

- **{SemVer}** (1.2.3) - Full version
- **{Major}.{Minor}** (1.2) - Minor version
- **{Major}** (1) - Major version
- **{sha}** (abc1234) - Git commit
- **latest** - Always points to newest master build

### Version Endpoint

Check the version of a running instance:

```bash
curl http://localhost:5000/api/version
```

## Technology Stack

- **Backend**: .NET 10 with Aspire orchestration
- **API**: ASP.NET Core Minimal APIs with Scalar documentation
- **Frontend**: Svelte 5 + SvelteKit
- **Database**: PostgreSQL

## Development

For detailed development guidelines, architecture information, and contribution rules, see [AGENTS.md](AGENTS.md).

## License

Deckle is licensed under the [Business Source License 1.1](LICENSE.md).

**TL;DR**: You can use Deckle for personal use, internal company use, and to design games you sell commercially. You cannot offer Deckle itself as a hosted service to others. The license converts to Apache 2.0 on after 2 years (exact date for current version in LICENSE.md)

For commercial licensing inquiries: deckle.games@gmail.com

## Contact

For questions, issues, or feature requests, please open an issue on GitHub.

---

Made with care for board game designers everywhere.