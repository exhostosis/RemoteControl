﻿using Servers.Results;

namespace Servers.ApiControllers;

public abstract class BaseApiController
{
    protected static IActionResult Ok() => new OkResult();
    protected static IActionResult Error(string? message) => new ErrorResult(message);
    protected static IActionResult Json(object data) => new JsonResult(data);
    protected static IActionResult Text(object data) => new TextResult(data);
}