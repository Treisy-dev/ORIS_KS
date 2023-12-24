using System;
using HttpServer.Attributes;

namespace HttpServer.Attributes
{
	public class HttpPostAttribute: HttpMethodAttribute
    {
		public string actionName;

		public HttpPostAttribute(string actionName) : base(actionName)
		{
			this.actionName = actionName;
		}
	}
}

