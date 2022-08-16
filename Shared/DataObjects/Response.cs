﻿using System.Net;
using Shared.Interfaces.Web;

namespace Shared.DataObjects
{
    public class Response : IResponse
    {
        public string ContentType { get; set; } = "text/plain";
        public byte[] Payload { get; set; } = Array.Empty<byte>();
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    }
}