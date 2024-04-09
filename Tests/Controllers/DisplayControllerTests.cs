﻿using ApiControllers;
using Microsoft.Extensions.Logging;
using Moq;
using Servers.Middleware;
using Shared;
using Shared.ApiControllers.Results;
using Shared.ControlProviders.Provider;

namespace UnitTests.Controllers;

public class DisplayControllerTests : IDisposable
{
    private readonly DisplayController _controller;
    private readonly Mock<IDisplayControlProvider> _provider;

    public DisplayControllerTests()
    {
        var logger = Mock.Of<ILogger>();
        _provider = new Mock<IDisplayControlProvider>(MockBehavior.Strict);

        _controller = new DisplayController(_provider.Object, logger);
    }

    [Fact]
    public void DarkenTest()
    {
        _provider.Setup(x => x.DisplayOff());

        var result = _controller.Darken(null);
        Assert.True(result is OkResult);

        _provider.Verify(x => x.DisplayOff(), Times.Once);
    }

    [Fact]
    public void GetMethodsTest()
    {
        var methodNames = new[]
        {
            "darken"
        };

        var methods = _controller.GetActions();
        Assert.True(methods.Count == methodNames.Length && methods.All(x => methodNames.Contains(x.Key)) && methods.All(
            x => x.Value.ReturnType == typeof(IActionResult)));
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}