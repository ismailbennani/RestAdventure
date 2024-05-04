using Microsoft.AspNetCore.Mvc;

namespace RestAdventure.Game.Controllers;

/// <summary>
///     Base class for API controllers
/// </summary>
[ApiController]
public abstract class ApiController : ControllerBase
{
    protected ActionResult ToActionResult(OperationResult result)
    {
        if (!result.IsSuccess)
        {
            return Problem(result.ErrorMessage, statusCode: result.HttpStatusCode);
        }

        return new StatusCodeResult(result.HttpStatusCode);
    }

    protected ActionResult ToFailedActionResult<T>(OperationResult<T> result)
    {
        if (!result.IsSuccess)
        {
            return Problem(result.ErrorMessage, statusCode: result.HttpStatusCode);
        }

        throw new InvalidOperationException("Expected operation to be a failure");
    }

    protected ActionResult<T> ToActionResult<T>(OperationResult<T> result)
    {
        if (!result.IsSuccess)
        {
            return Problem(result.ErrorMessage, statusCode: result.HttpStatusCode);
        }

        return result.Result;
    }

    protected ActionResult<TResult> ToActionResult<TSource, TResult>(OperationResult<TSource> result, Func<TSource, TResult> selector)
    {
        if (!result.IsSuccess)
        {
            return Problem(result.ErrorMessage, statusCode: result.HttpStatusCode);
        }

        return selector(result.Result);
    }
}
