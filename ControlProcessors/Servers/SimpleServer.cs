﻿using Shared;
using Shared.Config;
using Shared.ControlProcessor;
using Shared.Logging.Interfaces;
using Shared.Server;
using Shared.Server.Interfaces;

namespace Servers;

public class SimpleServer: IServerProcessor
{
    private static readonly ServerConfig DefaultConfig = new()
    {
        Scheme = "http",
        Host = "localhost",
        Port = 80
    };

    private readonly IListener _listener;

    public ServerConfig CurrentConfig {
        get => _currentConfig;
        set
        {
            _currentConfig = value;
            ConfigChanged?.Invoke(value);
        }
    }

    private ServerConfig _currentConfig;

    CommonConfig IControlProcessor.CurrentConfig
    {
        get => _currentConfig;
        set => _currentConfig = value as ServerConfig ?? _currentConfig;
    }

    public event ConfigEventHandler? ConfigChanged;
    public bool Working => _listener.IsListening;
    
    private readonly ILogger _logger;
    private readonly List<IObserver<bool>> _observers = new();

    public SimpleServer(IListener listener, AbstractMiddleware middleware, ILogger logger, ServerConfig? config = null)
    {
        _currentConfig = config ?? DefaultConfig;

        _listener = listener;
        _listener.OnRequest += middleware.ProcessRequest;
        _listener.OnStatusChange += status => _observers.ForEach(x => x.OnNext(status));

        _logger = logger;
    }

    public void Start(ServerConfig? config)
    {
        if(config != null)
            ConfigChanged?.Invoke(config);

        var c = config ?? _currentConfig;

        if (c.Uri == null)
            throw new Exception("Wrong uri config");

        _listener.StartListen(c.Uri);

        _currentConfig = c;
    }


    public void Restart(ServerConfig? config)
    {
        Stop();
        Start(config ?? _currentConfig);
    }

    public void Start(CommonConfig? config) => Start(config as ServerConfig ?? _currentConfig);
    public void Restart(CommonConfig? config) => Restart(config as ServerConfig ?? _currentConfig);

    public void Stop() => _listener.StopListen();

    public IDisposable Subscribe(IObserver<bool> observer)
    {
        _observers.Add(observer);
        return new Unsubscriber<bool>(_observers, observer);
    }
}