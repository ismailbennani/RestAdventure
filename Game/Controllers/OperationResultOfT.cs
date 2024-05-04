using System.Diagnostics.CodeAnalysis;

namespace RestAdventure.Game.Controllers;

/// <inheritdoc />
public class OperationResult<T> : IOperationResult
{
    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(Result))]
    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public required bool IsSuccess { get; init; }

    /// <inheritdoc />
    public required int HttpStatusCode { get; init; }

    /// <inheritdoc />
    public string? ErrorMessage { get; init; }

    /// <summary>
    ///     The result of the operation
    /// </summary>
    public T? Result { get; init; }

    /// <summary>
    ///     Convert the result to a <see cref="OperationResult" /> by dropping <see cref="Result" />
    /// </summary>
    public static implicit operator OperationResult(OperationResult<T> result) =>
        new()
        {
            IsSuccess = result.IsSuccess, HttpStatusCode = result.HttpStatusCode, ErrorMessage = result.ErrorMessage
        };
}
