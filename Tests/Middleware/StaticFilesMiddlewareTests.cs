﻿using Moq;
using Servers.Middleware;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Servers.DataObjects.Web;

namespace UnitTests.Middleware;

public class StaticFilesMiddlewareTests : IDisposable
{
    private readonly ILogger _logger;
    private readonly StaticFilesMiddleware _middleware;
    private const string Dir = "www";
    private readonly string _path = Path.Combine(AppContext.BaseDirectory, Dir);

    private const string IndexContents = @"
<html>
<title>test</title>
<body>test body</body>
</html>
";

    private const string FileContents = "contents";

    public StaticFilesMiddlewareTests()
    {
        _logger = Mock.Of<ILogger>();
        _middleware = new StaticFilesMiddleware(_logger, Dir);

        if (!Directory.Exists(_path))
            Directory.CreateDirectory(_path);

        File.WriteAllText(Path.Combine(_path, "index.html"), IndexContents);
        File.WriteAllText(Path.Combine(_path, "index.htm"), IndexContents);
        File.WriteAllText(Path.Combine(_path, "file.txt"), FileContents);
        File.WriteAllText(Path.Combine(_path, "file.ico"), FileContents);
        File.WriteAllText(Path.Combine(_path, "file.js"), FileContents);
        File.WriteAllText(Path.Combine(_path, "file.mjs"), FileContents);
        File.WriteAllText(Path.Combine(_path, "file.css"), FileContents);
    }

    [Theory]
    [InlineData("/index.html", HttpStatusCode.OK, "text/html", IndexContents)]
    [InlineData("/index.htm", HttpStatusCode.OK, "text/html", IndexContents)]
    [InlineData("/file.txt", HttpStatusCode.OK, "text/plain", FileContents)]
    [InlineData("/file.ico", HttpStatusCode.OK, "image/x-icon", FileContents)]
    [InlineData("/file.js", HttpStatusCode.OK, "text/javascript", FileContents)]
    [InlineData("/file.mjs", HttpStatusCode.OK, "text/javascript", FileContents)]
    [InlineData("/file.css", HttpStatusCode.OK, "text/css", FileContents)]
    [InlineData("..", HttpStatusCode.NotFound, "text/plain", "")]
    [InlineData("/unexdisting.file", HttpStatusCode.NotFound, "text/plain", "")]
    public async Task RequestTest(string path, HttpStatusCode code, string type, string response)
    {
        var context = new WebContext(new WebContextRequest(path), Mock.Of<WebContextResponse>());
        
        await _middleware.ProcessRequestAsync(context, null!);

        Assert.True(context.WebResponse.StatusCode == code && context.WebResponse.ContentType == type &&
                    Encoding.UTF8.GetString(context.WebResponse.Payload) ==
                    response);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Directory.Delete(_path, true);
    }
}