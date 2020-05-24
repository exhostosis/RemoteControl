﻿using RemoteControlCore.Interfaces;
using System.Collections.Generic;
using System.IO;
using RemoteControlCore.Abstract;

namespace RemoteControlCore.Controllers
{
    internal class HttpController : AbstractController
    {
        private const string ContentFolder = "www";

        private readonly Dictionary<string, string> _contentTypes = new Dictionary<string, string>()
        {
            { ".html", "text/html" },
            { ".htm", "text/html" },
            { ".ico", "image/x-icon" },
            { ".js", "text/javascript" },
            { ".mjs", "text/javascript" },
            { ".css", "text/css" }
        };

        public override void ProcessRequest(IHttpRequestArgs context)
        {
            var path = ContentFolder + context.Request.Url.LocalPath;

            if (context.Request.Url.LocalPath == "/")
            {
                path += (context.Request.UserAgent?.Contains("SM-R800") ?? false) || context.Simple ? "index-simple.html" : "index.html";
            }

            var extension = Path.GetExtension(path);

            context.Response.ContentType = _contentTypes.ContainsKey(extension) ? _contentTypes[extension] : "text/plain";

            if (File.Exists(path))
            {
                var buffer = File.ReadAllBytes(path);
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                context.Response.StatusCode = 404;
            }
        }
    }
}
