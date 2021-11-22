﻿using RemoteControl.App.Web.Interfaces;
using System.Net;

namespace RemoteControl.App.Web.Listeners
{
    internal static class MyHttpListener
    {
        private static readonly HttpListener _listener = new();

        public static void RestartListen()
        {
            StopListen();
            StartListen();
        }

        public static event HttpEventHandler? OnRequest;

        public static void StartListen(string url)
        {
            if (_listener.IsListening) return;

            if (!string.IsNullOrEmpty(url))
            {
                _listener.Prefixes.Clear();
                _listener.Prefixes.Add(url);
            }

            if (_listener.Prefixes.Count == 0) return;

            _listener.Start();

            Task.Factory.StartNew(Listen, TaskCreationOptions.LongRunning);
        }

        public static void StartListen() => StartListen(string.Empty);

        private static void Listen()
        {
            while (true)
            {
                HttpListenerContext context;

                try
                {
                    context = _listener.GetContext();
                }
                catch { return; }

                context.Response.StatusCode = 200;

                OnRequest?.Invoke(context);

                context?.Response.Close();
            }
        }

        public static void StopListen()
        {
            if (_listener.IsListening)
            {
                _listener.Stop();
            }
        }

        public static void RestartListen(string url)
        {
            StopListen();
            StartListen(url);
        }
    }
}
