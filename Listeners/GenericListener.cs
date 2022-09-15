﻿using Shared.DataObjects;
using Shared.Logging.Interfaces;
using Shared.Server;
using Shared.Server.Interfaces;
using System.Net;

namespace Listeners
{
    public class GenericListener : IListener
    {
        private HttpListener _listener = new();
        public bool IsListening => _listener.IsListening;
        public IReadOnlyCollection<string> ListeningUris => _listener.Prefixes.ToList();

        public event HttpEventHandler? OnRequest;

        private readonly TaskFactory _factory = new();

        private readonly ILogger _logger;

        public GenericListener(ILogger logger)
        {
            _logger = logger;
        }

        public void StartListen(string url)
        {
            try
            {
                if (_listener.IsListening)
                    _listener.Stop();
            }
            catch(ObjectDisposedException)
            {

            }
            finally
            {
                _listener = new HttpListener();
            }

            _listener.Prefixes.Add(url);

            try
            {
                _listener.Start();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }

            _logger.LogInfo($"started listening on {url}");

            _factory.StartNew(ProcessRequest, TaskCreationOptions.LongRunning);
        }

        private async Task ProcessRequest()
        {
            while (true)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    var path = context.Request.RawUrl;
                    if (path == null) return;

                    var dto = new Context(path);

                    OnRequest?.Invoke(dto);

                    context.Response.StatusCode = (int)dto.Response.StatusCode;
                    context.Response.ContentType = dto.Response.ContentType;

                    if (dto.Response.Payload.Length > 0)
                        context.Response.OutputStream.Write(dto.Response.Payload);

                    context.Response.Close();
                }
                catch (ObjectDisposedException)
                {
                    _listener = new HttpListener();
                    return;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);

                    if (!_listener.IsListening)
                        return;
                }
            }
        }

        public void StopListen()
        {
            if (_listener.IsListening)
            {
                _listener.Stop();
            }
        }
    }
}
