using System;
using HttpServer.Attributes;

namespace HttpServer.Attributes
{
	public class HttpGetAttribute: HttpMethodAttribute
    {
        public string actionName;

        public HttpGetAttribute(string actionName) : base(actionName)
		{
            this.actionName = actionName;
        }
    }
}

