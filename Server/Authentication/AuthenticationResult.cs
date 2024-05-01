﻿using System.Diagnostics.CodeAnalysis;

namespace Server.Authentication;

class AuthenticationResult
{
    AuthenticationResult(bool isSuccess, PlayerSession? session)
    {
        IsSuccess = isSuccess;
        Session = session;
    }

    [MemberNotNullWhen(true, nameof(Session))]
    public bool IsSuccess { get; }

    public PlayerSession? Session { get; }

    public static AuthenticationResult Success(PlayerSession playerSession) => new(true, playerSession);
    public static AuthenticationResult Failure() => new(false, null);
}
