﻿using System.Collections.Generic;

namespace Shared.Config;

public class AppConfig
{
    public List<ServerConfig> ServerConfigs { get; set; } = [];

    public AppConfig() { }

    public AppConfig(List<ServerConfig> items) => ServerConfigs = items;
}