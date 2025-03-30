namespace StreamierServer.Validators;

using FluentValidation;
using StreamierServer.GraphQL;
using Zxcvbn;

/// <summary>
/// Validates input for creating a new session, ensuring all required fields meet specified criteria.
/// Implements validation rules for email format, password strength, and session expiration period.
/// </summary>
public sealed class CreateSessionInputValidator : AbstractValidator<Mutation.CreateSessionInput>
{
    private const int MinimumPasswordLength = 8;
    private const int MinimumPasswordScore = 3;

    private const int MinimumExpirationDays = 1;
    private const int MaximumExpirationDays = 90;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSessionInputValidator"/> class.
    /// Configures all validation rules for session creation input.
    /// </summary>
    public CreateSessionInputValidator()
    {
        RuleFor(input => input.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MaximumLength(254)
            .WithMessage("Email cannot exceed 254 characters.");

        RuleFor(input => input.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(MinimumPasswordLength)
            .WithMessage($"Password must be at least {MinimumPasswordLength} characters long.")
            .Must(BeAStrongPassword)
            .WithMessage("Password is too weak. Please choose a stronger password.");

        RuleFor(input => input.ExpirationDays)
            .NotEmpty()
            .WithMessage("Expiration days is required.")
            .InclusiveBetween(MinimumExpirationDays, MaximumExpirationDays)
            .WithMessage(
                $"Expiration days must be between {MinimumExpirationDays} and {MaximumExpirationDays}."
            );
    }

    /// <summary>
    /// Validates password strength using zxcvbn algorithm.
    /// </summary>
    /// <param name="password">The password string to evaluate.</param>
    /// <returns>
    /// <c>true</c> if password meets minimum strength requirements (score >= MinimumPasswordScore),
    /// <c>false</c> otherwise.
    /// </returns>
    private static bool BeAStrongPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        var result = Core.EvaluatePassword(password);

        return result.Score >= MinimumPasswordScore;
    }
}
