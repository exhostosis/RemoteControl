﻿using System.Net;

namespace RemoteControl.App.Web.DataObjects
{
    internal class Response
    {
        public string ContentType = "text/plain";
        public byte[] Payload = Array.Empty<byte>();
        public HttpStatusCode StatusCode = HttpStatusCode.OK;
    }
}