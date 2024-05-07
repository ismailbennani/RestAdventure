using System.Diagnostics.CodeAnalysis;

namespace RestAdventure.Kernel.Errors;

public class Maybe<T>
{
    /// <summary>
    ///     The success status
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(WhyNot))]
    public required bool Success { get; init; }

    /// <summary>
    ///     The reason why <see cref="Success" /> is false
    /// </summary>
    public string? WhyNot { get; init; }

    /// <summary>
    ///     The value if <see cref="Success" /> is true
    /// </summary>
    public T? Value { get; init; }

    public static implicit operator Maybe<T>(T value) => new() { Success = true, Value = value };
    public static implicit operator Maybe<T>(string reason) => new() { Success = false, WhyNot = reason };

    public static implicit operator Maybe(Maybe<T> maybe) => new() { Success = maybe.Success, WhyNot = maybe.WhyNot };

    public static implicit operator bool(Maybe<T> maybe) => maybe.Success;
}

public class Maybe
{
    /// <summary>
    ///     The success status
    /// </summary>
    [MemberNotNullWhen(false, nameof(WhyNot))]
    public required bool Success { get; init; }

    /// <summary>
    ///     The reason why <see cref="Success" /> is false
    /// </summary>
    public string? WhyNot { get; init; }

    public static implicit operator Maybe(bool success) => new() { Success = success, WhyNot = success ? null : "Could not perform the operation" };
    public static implicit operator Maybe(string reason) => new() { Success = false, WhyNot = reason };

    public static implicit operator bool(Maybe maybe) => maybe.Success;
}
