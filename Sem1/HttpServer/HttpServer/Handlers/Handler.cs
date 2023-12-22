using System;
using System.Net;

namespace HttpServer.Handlers
{
    public abstract class Handler
    {
        public Handler Successor { get; set; }
        public abstract void HandleRequest(HttpListenerContext context);
    }
}