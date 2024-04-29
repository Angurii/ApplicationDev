using System.Text.Json;
using System.Text;
using System.Net;
using ApplicationDev.Common.Middleware.DTOs;
using Microsoft.AspNetCore.Http;

namespace ApplicationDev.Common.Middleware.Response
{
	public class ResponseInterceptor
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ResponseInterceptor> _logger;
		public ResponseInterceptor(RequestDelegate next, ILogger<ResponseInterceptor> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			// Skip the middleware if the request is for the Swagger UI
			if (context.Request.Path.StartsWithSegments("/swagger"))
			{
				await _next(context);
				return;
			}

			// Store the original response body stream and replace it with a new one
			var originalBodyStream = context.Response.Body;
			using var newBodyStream = new MemoryStream();
			// Replace the response body with the new stream
			context.Response.Body = newBodyStream;

			try
			{
				// Continue the pipeline processing
				await _next(context);

				// Reset the new stream position to read the response body
				newBodyStream.Seek(0, SeekOrigin.Begin);
				var responseBody = await new StreamReader(newBodyStream).ReadToEndAsync();

				object data;
				// Try to deserialize the responseBody to an object if it's JSON, otherwise use the string directly
				if (IsJsonResponse(context.Response.ContentType))
				{
					data = JsonSerializer.Deserialize<object>(responseBody) ?? new { };
				}
				else
				{
					data = responseBody; // Use the string directly if it's not JSON
				}



				var responseDto = new ResponseDTO
				{
					Date = DateTime.UtcNow,
					RequestedUrl = context.Request.Path,
					StatusCode = context.Response.StatusCode,
					Data = data
				};
				_logger.LogInformation("Context Items : {Response}", context.Items["CustomMessage"]);
				if (context.Items["CustomMessage"] is string message)
				{
					_logger.LogInformation("Custom message: {Message}", message);
					responseDto.Message = message;
				}

				var responseJson = JsonSerializer.Serialize(responseDto, new JsonSerializerOptions { WriteIndented = true });

				context.Response.ContentType = "application/json";
				// context.Response.StatusCode = (int)HttpStatusCode.OK;
				await originalBodyStream.WriteAsync(Encoding.UTF8.GetBytes(responseJson), context.RequestAborted);
			}
			finally
			{
				// Restore the original stream to make sure subsequent middleware can operate correctly
				context.Response.Body = originalBodyStream;
			}
		}

		private bool IsJsonResponse(string contentType)
		{
			// Check if the Content-Type header indicates a JSON response
			return contentType != null && contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase);
		}
	}
}
