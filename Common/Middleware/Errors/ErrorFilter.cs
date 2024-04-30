using ApplicationDev.Common.Exceptions;
using ApplicationDev.Common.Middleware.DTOs;
using System.Net;
using System.Text.Json;

namespace ApplicationDev.Common.Middleware.Errors
{
	public class ErrorFilter
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ErrorFilter> _logger;

		public ErrorFilter(RequestDelegate next, ILogger<ErrorFilter> logger)
		{
			_next = next;
			_logger = logger;

		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{

				// Continue the pipeline processing
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while processing the request.");

				// Handle the exception
				await HandleExceptionAsync(context, ex);
			}
		}
		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			//Default status Code for the response
			var statusCode = HttpStatusCode.InternalServerError;
			//Default Message for the response
			var message = "An unexpected error occurred";

			//Check if the exception is the custom http exception
			if (exception is HttpException httpException)
			{

				//if it is change the values of status code and message
				statusCode = httpException.StatusCode;
				message = httpException.Message;
			}

			//Create the response object
			var response = new ErrorResponseDto
			{
				Date = DateTime.UtcNow,
				RequestedUrl = context.Request.Path,
				Message = message,
				StatusCode = (int)statusCode
			};

			//Set the response values
			context.Response.ContentType = "application/json";
			//Set the status code
			context.Response.StatusCode = (int)statusCode;

			//Serialize the response object
			var result = JsonSerializer.Serialize(response);
			//Write the response
			return context.Response.WriteAsync(result);
		}
	}

}