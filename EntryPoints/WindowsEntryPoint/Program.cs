﻿using Autostart;
using ConfigProviders;
using ControlProviders.Wrappers;
using ControlProviders;
using Microsoft.Win32;
using Shared.Config;
using Shared.ControlProviders.Input;
using Shared.ControlProviders.Provider;
using Shared.Logging.Interfaces;
using Shared.UI;
using Shared;
using System.Runtime.InteropServices;
using WinFormsUI;
using Logging;
using Shared.ConsoleWrapper;
using Shared.Logging;
using Shared.Wrappers.Registry;
using Shared.Wrappers.RegistryWrapper;

namespace WindowsEntryPoint;

public static class Program
{
    public static void Main()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

#if DEBUG
        ILogger logger = new TraceLogger(new TraceWrapper());
#else
        ILogger logger = new FileLogger(Path.Combine(AppContext.BaseDirectory, "error.log"));
#endif

        var container = new Container()
            .Register<ILogger>(logger)
            .Register(typeof(ILogger<>), typeof(LogWrapper<>), Lifetime.Singleton)
            .Register<IConfigProvider, LocalFileConfigProvider>(Lifetime.Singleton)
            .Register<IRegistry, RegistryWrapper>(Lifetime.Singleton)
            .Register<IAutostartService, RegistryAutostartService>(Lifetime.Singleton)
            .Register<IGeneralControlProvider, InputProvider>(Lifetime.Singleton)
            .Register<IUserInterface, MainForm>(Lifetime.Singleton)
            .Register<IKeyboardInput, User32Wrapper>(Lifetime.Singleton)
            .Register<IMouseInput, User32Wrapper>(Lifetime.Singleton)
            .Register<IDisplayInput, User32Wrapper>(Lifetime.Singleton)
            .Register<IAudioInput, NAudioWrapper>(Lifetime.Singleton);
        
        var type = typeof(Program);

        var indexes = new List<int>();

        SystemEvents.SessionSwitch += (_, args) =>
        {
            switch (args.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    {
                        logger.LogInfo(type, "Stopping servers due to logout");

                        indexes.Clear();

                        for (var i = 0; i < RemoteControlMain.Program.Servers.Count; i++)
                        {
                            if (RemoteControlMain.Program.Servers[i].Status.Working)
                            {
                                indexes.Add(i);
                                RemoteControlMain.Program.Servers[i].Stop();
                            }
                        }

                        break;
                    }
                case SessionSwitchReason.SessionUnlock:
                    {
                        logger.LogInfo(type, "Resoring servers");

                        foreach (var index in indexes)
                        {
                            RemoteControlMain.Program.Servers[index].Start();
                        }
                        break;
                    }
                default:
                    break;
            }
        };

        try
        {
            RemoteControlMain.Program.Run(container);
        }
        catch (Exception e)
        {
            logger.LogError(type, e.Message);
        }
    }
}
