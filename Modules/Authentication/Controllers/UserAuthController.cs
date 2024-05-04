using ApplicationDev.Common.Exceptions;
using ApplicationDev.Modules.Authentication.DTOs;
using ApplicationDev.Modules.User.Entity;
using ApplicationDev.Modules.User.Services;
using ApplicationDev.Modules.Authentication.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApplicationDev.Modules.Authentication.Controllers
{
	[ApiExplorerSettings(GroupName = "user")]
	[Tags("Auth")]
	[Route("api/user/auth/")]
	public class UserAuthController : ControllerBase
	{
		private readonly AuthenticationService _authService;
		private readonly UserService _userService;

		private readonly ILogger<UserAuthController> _logger;

		public UserAuthController(AuthenticationService authService, UserService userService, ILogger<UserAuthController> logger)
		{
			_authService = authService;
			_userService = userService;
			_logger = logger;
		}

		[HttpPost("login")]
		async public Task<IActionResult> Login([FromBody] UserLoginDTO incomingData)
		{
			try
			{
				UserEntity? user = null;

				if (incomingData.UserName != null)
				{
					user = await _userService.FindOneByUserName(incomingData.UserName);
				}

				if (incomingData.Email != null)
				{

					user = await _userService.FindOneByEmail(incomingData.Email);
				}

				if (user == null)
				{
					throw new HttpException(HttpStatusCode.NotFound, "User Not Found");
				}

				HttpContext.Items["CustomMessage"] = "User LoggedIn Successfully";
				return Ok(_authService.Login(user, incomingData, "user"));

			}
			catch (Exception)
			{
				throw;
			}
		}

	}
}