﻿using Shared.DataObjects.Web;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Servers.Middleware;

public class LoggingMiddleware : IWebMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public event EventHandler<WebContext>? OnNext;

    public void ProcessRequest(object? _, WebContext context)
    {
        _logger.LogInfo(context.WebRequest.Path);

        OnNext?.Invoke(null, context);

        _logger.LogInfo($"{context.WebResponse.StatusCode}\n{context.WebResponse.ContentType}\n{context.WebResponse.Payload}");
    }
}