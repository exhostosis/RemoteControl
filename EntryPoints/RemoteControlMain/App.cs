﻿using Shared.AutoStart;
using Shared.Config;
using Shared.Enums;
using Shared.Observable;
using Shared.Server;
using Shared.UI;

namespace RemoteControlMain;

public class App(IUserInterface ui, IConfigProvider configProvider, IAutoStartService autoStartService, ServerFactory serverFactory)
{
    private int _id;
    private static AppConfig GetConfig(IEnumerable<IServer> servers) =>
        new(servers.Select(x => x.Config));

    public List<IServer> Servers { get; private set; } = [];

    public void Run()
    {
        var config = configProvider.GetConfig();

        Servers = config.ServerConfigs.Select<CommonConfig, IServer>(x =>
        {
            switch (x)
            {
                case WebConfig s:
                    var server = serverFactory.GetServer();
                    server.CurrentConfig = s;
                    server.Id = _id++;
                    return server;
                case BotConfig b:
                    var bot = serverFactory.GetBot();
                    bot.CurrentConfig = b;
                    bot.Id = _id++;
                    return bot;
                default:
                    throw new NotSupportedException("Config not supported");
            }
        }).ToList();

        Servers.ForEach(x =>
        {
            if (x.Config.AutoStart)
                x.Start();
        });

        ui.SetAutoStartValue(autoStartService.CheckAutoStart());

        ui.ServerStart.Subscribe(new MyObserver<int?>( id =>
        {
            if (!id.HasValue)
            {
                Servers.ForEach(x => x.Start());
            }
            else
            {
                Servers.FirstOrDefault(x => x.Id == id)?.Start();
            }
        }));

        ui.ServerStop.Subscribe(new MyObserver<int?>(id =>
        {
            if (!id.HasValue)
            {
                Servers.ForEach(x => x.Stop());
            }
            else
            {
                Servers.FirstOrDefault(x => x.Id == id)?.Stop();
            }
        }));

        ui.ServerAdd.Subscribe(new MyObserver<ServerType>(mode =>
        {
            IServer server = mode switch
            {
                ServerType.Http => serverFactory.GetServer(),
                ServerType.Bot => serverFactory.GetBot(),
                _ => throw new NotSupportedException()
            };

            server.Id = _id++;

            Servers.Add(server);
            ui.AddServer(server);
        }));

        ui.ServerRemove.Subscribe(new MyObserver<int>(id =>
        {
            var server = Servers.FirstOrDefault(x => x.Id == id);
            if (server == null)
                return;

            server.Stop();
            Servers.Remove(server);

            configProvider.SetConfig(GetConfig(Servers));
        }));

        ui.AutoStartChange.Subscribe(new MyObserver<bool>(value =>
        {
            autoStartService.SetAutoStart(value);
            ui.SetAutoStartValue(autoStartService.CheckAutoStart());
        }));

        ui.ConfigChange.Subscribe(new MyObserver<(int, CommonConfig)>(configTuple =>
        {
            var server = Servers.FirstOrDefault(x => x.Id == configTuple.Item1);
            if (server == null)
                return;

            if (server.Status.Working)
            {
                server.Restart(configTuple.Item2);
            }
            else
            {
                server.Config = configTuple.Item2;
            }

            config = GetConfig(Servers);
            configProvider.SetConfig(config);
        }));

        ui.AppClose.Subscribe(new MyObserver<object?>(_ =>
        {
            Environment.Exit(0);
        }));

        ui.RunUI(Servers);
    }
}