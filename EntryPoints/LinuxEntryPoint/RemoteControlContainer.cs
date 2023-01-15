﻿using Autostart;
using ConfigProviders;
using ConsoleUI;
using ControlProviders;
using ControlProviders.Wrappers;
using Logging;
using Shared;
using Shared.Config;
using Shared.ControlProviders;
using Shared.Logging;
using Shared.Logging.Interfaces;
using Shared.UI;

namespace LinuxEntryPoint;

public class RemoteControlContainer : IPlatformDependantContainer
{
    public IConfigProvider ConfigProvider { get; }
    public IAutostartService AutostartService { get; }
    public IUserInterface UserInterface { get; }
    public IAudioControlProvider AudioProvider { get; }
    public IDisplayControlProvider DisplayProvider { get; }
    public IKeyboardControlProvider KeyboardProvider { get; }
    public IMouseControlProvider MouseProvider { get; }
    public ControlFacade ControlProviders { get; }
    public ILogger Logger { get; }
    public ILogger NewLogger()
    {
#if DEBUG
        return new TraceLogger();
#else
        return new FileLogger(Path.Combine(AppContext.BaseDirectory, "error.log"));
#endif
    }

    public IConfigProvider NewConfigProvider(ILogger logger) =>
        new LocalFileConfigProvider(Path.Combine(AppContext.BaseDirectory, "config.ini"),new LogWrapper<LocalFileConfigProvider>(logger));

    public IAutostartService NewAutostartService(ILogger logger) =>
        new DummyAutostartService(new LogWrapper<DummyAutostartService>(logger));

    public IUserInterface NewUserInterface() => new MainConsole();

    public IKeyboardControlProvider NewKeyboardProvider(ILogger logger) =>
        new InputProvider(new YdoToolWrapper(), new LogWrapper<InputProvider>(Logger));

    public IMouseControlProvider NewMouseProvider(ILogger logger) =>
        new InputProvider(new YdoToolWrapper(), new LogWrapper<InputProvider>(logger));

    public IDisplayControlProvider NewDisplayProvider(ILogger logger) =>
        new DummyProvider(new LogWrapper<DummyProvider>(logger));

    public IAudioControlProvider NewAudioProvider(ILogger logger) =>
        new DummyProvider(new LogWrapper<DummyProvider>(logger));

    public RemoteControlContainer()
    {
        Logger = NewLogger();

        var ydoToolProvider = new InputProvider(new YdoToolWrapper(), new LogWrapper<InputProvider>(Logger));
        var dummyProvider = new DummyProvider(new LogWrapper<DummyProvider>(Logger));
        
        AudioProvider = dummyProvider;
        KeyboardProvider = ydoToolProvider;
        MouseProvider = ydoToolProvider;
        DisplayProvider = dummyProvider;

        ControlProviders = new ControlFacade(AudioProvider, KeyboardProvider, MouseProvider, DisplayProvider);

        ConfigProvider = NewConfigProvider(Logger);
        AutostartService = NewAutostartService(Logger);
        
        UserInterface = NewUserInterface();
    }
}