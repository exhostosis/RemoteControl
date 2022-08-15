﻿using Autostart;
using Config;
using Control.Wrappers;
using Http.Listeners;
using Logging;
using RemoteControlApp;
using RemoteControlApp.Web.Controllers;
using RemoteControlApp.Web.Middleware;
using Shared.Enums;

var fileLogger = new FileLogger(AppContext.BaseDirectory + "error.log");
var consoleLogger = new ConsoleLogger(LoggingLevel.Info);

var audio = new AudioSwitchWrapper();
var input = new WindowsInputLibWrapper();
var display = new User32Wrapper();

var uiListener = new GenericListener();
var apiListener = new GenericListener();

var controllers = new BaseController[]
{
    new AudioController(audio, consoleLogger),
    new KeyboardController(input, consoleLogger),
    new MouseController(input, consoleLogger),
    new DisplayController(display, consoleLogger)
};

var uiMiddlewareChain = new LoggingMiddleware(consoleLogger).Attach(new FileMiddleware()).GetFirst();
var apiMiddlewareChain = new LoggingMiddleware(consoleLogger).Attach(new ApiMiddlewareV1(controllers)).GetFirst();

var app = new RemoteControl(uiListener, apiListener, uiMiddlewareChain, apiMiddlewareChain);
var config = new LocalFileConfigService(fileLogger);
var autostart = new WinAutostartService();

var uri = config.GetConfig().UriConfig.Uri;

RemoteControlWinForms.Program.Inject(app, config, autostart, fileLogger);
RemoteControlWinForms.Program.Main();

Console.Read();