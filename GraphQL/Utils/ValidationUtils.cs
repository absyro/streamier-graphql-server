namespace StreamierGraphQLServer.GraphQL.Utils;

using System.ComponentModel.DataAnnotations;
using HotChocolate;

/// <summary>
/// Utility class for input validation.
/// </summary>
public static class ValidationUtils
{
    /// <summary>
    /// Validates an input object using data annotations.
    /// </summary>
    public static void ValidateInput(object input)
    {
        var validationContext = new ValidationContext(input);
        var validationResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(input, validationContext, validationResults, true))
        {
            throw new GraphQLException(
                ErrorBuilder
                    .New()
                    .SetMessage("Input validation failed")
                    .SetCode("VALIDATION_ERROR")
                    .SetExtension("validationErrors", validationResults)
                    .Build()
            );
        }
    }
}
