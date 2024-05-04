using System.Diagnostics.CodeAnalysis;

namespace RestAdventure.Game.Controllers;

/// <inheritdoc />
public class OperationResult : IOperationResult
{
    /// <inheritdoc />
    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public required bool IsSuccess { get; init; }

    /// <inheritdoc />
    public required int HttpStatusCode { get; init; }

    /// <inheritdoc />
    public string? ErrorMessage { get; init; }

    /// <summary>
    ///     The operation succeeded
    /// </summary>
    public static OperationResult<T> Ok<T>(T result) => new() { IsSuccess = true, HttpStatusCode = StatusCodes.Status200OK, Result = result };

    /// <summary>
    ///     The operation succeeded
    /// </summary>
    public static OperationResult NoContent() => new() { IsSuccess = true, HttpStatusCode = StatusCodes.Status204NoContent };

    /// <summary>
    ///     The operation succeeded
    /// </summary>
    public static OperationResult<T> NoContent<T>() => new() { IsSuccess = true, HttpStatusCode = StatusCodes.Status204NoContent };

    /// <summary>
    ///     The server cannot process the request because of a client error
    /// </summary>
    public static OperationResult BadRequest(string? message = null) =>
        new() { IsSuccess = false, HttpStatusCode = StatusCodes.Status400BadRequest, ErrorMessage = message ?? "The server cannot process the request because of a client error" };

    /// <summary>
    ///     The server cannot process the request because of a client error
    /// </summary>
    public static OperationResult<T> BadRequest<T>(string? message = null) =>
        new() { IsSuccess = false, HttpStatusCode = StatusCodes.Status400BadRequest, ErrorMessage = message ?? "The server cannot process the request because of a client error" };

    /// <summary>
    ///     The requested resource could not be found
    /// </summary>
    public static OperationResult NotFound(string? message = null) =>
        new() { IsSuccess = false, HttpStatusCode = StatusCodes.Status404NotFound, ErrorMessage = message ?? "The requested resource could not be found" };

    /// <summary>
    ///     The requested resource could not be found
    /// </summary>
    public static OperationResult<T> NotFound<T>(string? message = null) =>
        new() { IsSuccess = false, HttpStatusCode = StatusCodes.Status404NotFound, ErrorMessage = message ?? "The requested resource could not be found" };
}
