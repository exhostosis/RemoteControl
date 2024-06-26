﻿using MainApp.Interfaces;
using Microsoft.Extensions.Logging;

#if DEBUG
using Microsoft.Extensions.Logging.Debug;
#else
using NReco.Logging.File;
#endif

namespace MainApp;

public sealed class AppHostBuilder
{
    private ILoggerProvider? _loggerProvider;
    private ServerFactory? _serverFactory;
    private RegistryAutoStartService? _autoStartService;
    private IConfigurationProvider? _configProvider;

    public AppHostBuilder UseLogger(ILoggerProvider loggerProvider)
    {
        _loggerProvider = loggerProvider;
        return this;
    }

    public AppHostBuilder UseConfiguration(IConfigurationProvider configProvider)
    {
        _configProvider = configProvider;
        return this;
    }

    public AppHost Build()
    {
#if DEBUG
        _loggerProvider ??= new DebugLoggerProvider();
#else
        _loggerProvider ??= new FileLoggerProvider(Path.Combine(AppContext.BaseDirectory, "error.log"), new FileLoggerOptions
        {
            Append = true,
            MinLevel = LogLevel.Error
        });
#endif
        _serverFactory ??= new ServerFactory(_loggerProvider);
        _autoStartService ??=
            new RegistryAutoStartService(_loggerProvider.CreateLogger(nameof(RegistryAutoStartService)));
        _configProvider ??= new JsonConfigurationProvider(
            _loggerProvider.CreateLogger(nameof(JsonConfigurationProvider)),
            Path.Combine(AppContext.BaseDirectory, "appsettings.json"));

        return new AppHost(_loggerProvider, _serverFactory, _autoStartService, _configProvider);
    }
}