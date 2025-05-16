# Streamier GraphQL Server

A modern GraphQL server built with .NET 9.0, HotChocolate, and Entity Framework Core, providing a robust API for the Streamier platform.

## Prerequisites

- .NET 9.0 SDK
- PostgreSQL database

## Getting Started

1. Clone the repository:

   ```bash
   git clone https://github.com/absyro/streamier-graphql-server.git
   cd streamier-graphql-server
   ```

2. Configure the database connection and Resend API token in user secrets:

   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:Postgres" "Host=localhost;Database=streamier;Username=your_username;Password=your_password"
   dotnet user-secrets set "Resend:ApiToken" "your_resend_api_token"
   ```

3. Run database migrations:

   ```bash
   dotnet ef database update
   ```

4. Start the server:
   ```bash
   dotnet run
   ```

The GraphQL endpoint will be available at `http://localhost:5014/graphql`

## Development

- The project uses Entity Framework Core for database operations
- HotChocolate provides the GraphQL implementation
- Security features include BCrypt for password hashing and OTP for two-factor authentication
- Rate limiting and CORS are implemented for API protection

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.

## Security

For security concerns, please read [SECURITY.md](SECURITY.md) and report any vulnerabilities to our security team.

## License

This project is licensed under the terms specified in [LICENSE.md](LICENSE.md).
