using System;
using HttpServer.Attributes;

namespace HttpServer.Attributes
{
	public class HttpController : Attribute
	{
		public string name { get; }

        public HttpController(string name)
        {
            this.name = name;
        }
    }
}

