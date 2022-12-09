﻿using System.Net;
using Shared.DataObjects.Interfaces;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Servers.Endpoints;

public class StaticFilesEndpoint : AbstractEndpoint
{
    private readonly string _contentFolder;

    public StaticFilesEndpoint(ILogger logger, string directory = "www") : base(logger)
    {
        _contentFolder = AppContext.BaseDirectory + directory;
        IsStaticFiles = true;
    }

    private static readonly Dictionary<string, string> ContentTypes = new()
    {
        { ".html", "text/html" },
        { ".htm", "text/html" },
        { ".ico", "image/x-icon" },
        { ".js", "text/javascript" },
        { ".mjs", "text/javascript" },
        { ".css", "text/css" }
    };

    public override void ProcessRequest(IContext context)
    {
        var uriPath = context.Request.Path;

        if (uriPath.Contains(".."))
        {
            context.Response.StatusCode = HttpStatusCode.NotFound;
            return;
        }

        var path = _contentFolder + uriPath;

        if (string.IsNullOrEmpty(uriPath) || uriPath == "/")
        {
            path += "index.html";
        }

        var extension = Path.GetExtension(path);

        context.Response.ContentType = ContentTypes.ContainsKey(extension) ? ContentTypes[extension] : "text/plain";

        if (File.Exists(path))
        {
            context.Response.Payload = File.ReadAllBytes(path);
        }
        else
        {
            context.Response.StatusCode = HttpStatusCode.NotFound;
        }
    }
}