﻿using System.Collections.Generic;

namespace RemoteControl.Core.Interfaces
{
    internal interface IListener
    {
        void StartListen(string url);
        void StartListen();
        void StopListen();
        void RestartListen(string url);
        void RestartListen();

        event HttpEventHandler OnHttpRequest;
        event HttpEventHandler OnApiRequest;
    }
}
