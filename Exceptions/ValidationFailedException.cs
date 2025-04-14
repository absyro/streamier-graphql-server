namespace StreamierGraphQLServer.Exceptions;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Thrown when input validation fails during a mutation operation.
/// </summary>
/// <param name="validationResults">The collection of validation results that failed.</param>
public class ValidationFailedException(IEnumerable<ValidationResult> validationResults)
    : Exception(
        $"Input validation failed: {string.Join(" ", validationResults.Select(r => r.ErrorMessage))}"
    )
{
    /// <summary>
    /// The validation results that caused the failure.
    /// </summary>
    public IReadOnlyCollection<ValidationResult> ValidationResults { get; } =
        validationResults.ToList().AsReadOnly();
}
