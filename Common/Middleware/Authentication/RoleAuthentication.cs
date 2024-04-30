using System.IdentityModel.Tokens.Jwt;
using System.Net;
using ApplicationDev.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApplicationDev.Common.Middlewares.Authentication
{

	public class RoleAuthentication : IActionFilter
	{
		private readonly ILogger<RoleAuthentication> _logger;
		public RoleAuthentication(ILogger<RoleAuthentication> logger)
		{
			_logger = logger;
		}
		public void OnActionExecuting(ActionExecutingContext context)
		{
			try
			{
				string? token = context.HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
				_logger.LogInformation("Token: {token}", token);

				if (token == null)
				{
					throw new HttpException(HttpStatusCode.Unauthorized, "Token Not Found");
				}


				var jwtToken = new JwtSecurityToken(token);
				var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role");


				string roleFromToken = roleClaim.Value;
				string roleFromUrl = context.HttpContext.Request.Path.Value.Split('/')[2]; // Get the role from the URL
				_logger.LogInformation("This is Role from url: " + roleFromUrl);
				_logger.LogInformation("Role from token: {roleFromToken}", roleFromToken);

				if (roleFromToken != roleFromUrl)
				{
					throw new HttpException(HttpStatusCode.Forbidden, "Not Authorized");
				}
			}
			catch (Exception)
			{
				throw;
			}

		}

		public void OnActionExecuted(ActionExecutedContext context)
		{
			// Do nothing
		}
	}
}