# Contributing to Streamier GraphQL Server

Thank you for your interest in contributing to our project! This document provides guidelines and instructions for contributing to the Streamier GraphQL Server.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Process](#development-process)
- [Pull Request Process](#pull-request-process)
- [Coding Standards](#coding-standards)
- [Testing](#testing)
- [Documentation](#documentation)

## Code of Conduct

By participating in this project, you agree to abide by our [Code of Conduct](CODE_OF_CONDUCT.md).

## Getting Started

1. Fork the repository
2. Clone your fork:
   ```bash
   git clone https://github.com/absyro/streamier-graphql-server.git
   cd streamier-graphql-server
   ```
3. Create a new branch for your feature:
   ```bash
   git checkout -b feature/your-feature-name
   ```

## Development Process

1. Make your changes following our [coding standards](#coding-standards)
2. Write or update tests as needed
3. Update documentation if necessary
4. Ensure all tests pass
5. Submit a pull request

## Pull Request Process

1. Update the README.md with details of changes if needed
2. The PR must pass all CI checks
3. You may merge the PR once you have the sign-off of at least one other developer

## Coding Standards

### C# Standards

- Follow the [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Keep methods focused and small
- Use async/await for I/O operations

### GraphQL Standards

- Use descriptive type and field names
- Follow the GraphQL naming conventions
- Document all types and fields
- Use proper error handling
- Implement proper authorization

### Git Commit Messages

- Use the [Conventional Commits](https://www.conventionalcommits.org/) format
- Start with a type (feat, fix, docs, style, refactor, test, chore)
- Use the present tense ("add feature" not "added feature")
- Use the imperative mood ("move cursor to..." not "moves cursor to...")
- Limit the first line to 72 characters or less
- Reference issues and pull requests after the first line

## Testing

- Write unit tests for new features
- Ensure all tests pass before submitting a PR
- Follow the existing test patterns
- Use meaningful test names
- Cover edge cases

## Documentation

- Update documentation for any new features
- Keep documentation up-to-date with changes
- Use clear and concise language
- Include examples where appropriate

## Questions?

If you have any questions, feel free to:

- Open an issue
- Join our community chat
- Contact the maintainers

Thank you for contributing to Streamier GraphQL Server!
