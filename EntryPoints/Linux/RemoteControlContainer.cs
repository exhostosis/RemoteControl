﻿using Autostart;
using ConfigProviders;
using ConsoleUI;
using ControlProviders;
using Logging;
using Shared;
using Shared.Config;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;
using Shared.UI;

namespace Linux;

public class RemoteControlContainer : IPlatformDependantContainer
{
    public IConfigProvider ConfigProvider { get; }
    public IAutostartService AutostartService { get; }
    public ILogger Logger { get; }
    public IUserInterface UserInterface { get; }
    public ControlFacade ControlProviders { get; }

    public RemoteControlContainer()
    {
#if DEBUG
        Logger = new TraceLogger();
#else
        Logger = new FileLogger("error.log");
#endif
        var ydotoolWrapper = new YdotoolProvider(Logger);
        var dummyWrapper = new DummyProvider(Logger);

        ControlProviders = new ControlFacade(dummyWrapper, ydotoolWrapper, ydotoolWrapper, dummyWrapper);

        ConfigProvider = new LocalFileConfigProvider(Logger);
        AutostartService = new DummyAutostartService(Logger);
        UserInterface = new MainConsole();
    }
}