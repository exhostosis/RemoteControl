﻿using Shared.DataObjects.Http;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Servers.Middleware;

public class LoggingMiddleware : AbstractMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger, EventHandler<Context>? next = null) : base(next)
    {
        _logger = logger;
    }

    public override void ProcessRequest(object? sender, Context context)
    {
        _logger.LogInfo(context.Request.Path);

        Next?.Invoke(this, context);

        _logger.LogInfo($"{context.Response.StatusCode}\n{context.Response.ContentType}\n{context.Response.Payload}");
    }
}