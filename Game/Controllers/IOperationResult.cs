namespace RestAdventure.Game.Controllers;

/// <summary>
///     Represent the result of an operation
/// </summary>
public interface IOperationResult
{
    /// <summary>
    ///     Has the operation been successful
    /// </summary>
    bool IsSuccess { get; init; }

    /// <summary>
    ///     The status code describing the outcome
    /// </summary>
    int HttpStatusCode { get; init; }

    /// <summary>
    ///     If set, an error message describing why the operation failed
    /// </summary>
    string? ErrorMessage { get; init; }
}
