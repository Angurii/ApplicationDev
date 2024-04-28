using System.Net;

namespace ApplicationDev.Common.Exceptions
{
	public class HttpException : Exception
	{
		public HttpStatusCode StatusCode { get; set; }
		public HttpException(HttpStatusCode statusCode, string message) : base(message)
		{
			StatusCode = statusCode;
		}
	}
}