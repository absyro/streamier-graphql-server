namespace StreamierGraphQLServer.Exceptions;

using Zxcvbn;

/// <summary>
/// Thrown when a password doesn't meet the required strength/complexity criteria.
/// </summary>
/// <param name="feedback">The password strength feedback from zxcvbn.</param>
public class WeakPasswordException(FeedbackItem feedback)
    : Exception(
        $"Password is too weak: {string.Join(" ", new[] { feedback.Warning }.Concat(feedback.Suggestions))}"
    ) { }
