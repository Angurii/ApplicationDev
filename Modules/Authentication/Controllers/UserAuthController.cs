using ApplicationDev.Common.Exceptions;
using ApplicationDev.Modules.Authentication.DTOs;
using ApplicationDev.Modules.User.Entity;
using ApplicationDev.Modules.User.Services;
using ApplicationDev.Modules.Authentication.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ApplicationDev.Modules.User.DTOs;

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
		public async Task<IActionResult> Login([FromBody] UserLoginDTO incomingData)
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

				if (user.IsActive == false)
				{
					throw new HttpException(HttpStatusCode.BadRequest, "User Not Found");
				}
				string token = _authService.Login(user, incomingData, "user");
				HttpContext.Items["CustomMessage"] = "User LoggedIn Successfully";
				return Ok(token);

			}
			catch (Exception)
			{
				throw;
			}
		}

		[HttpGet("verify-email/{email}")]
		public async Task<IActionResult> VerifyEmail(string email)
		{
			//Decode Email
			email = WebUtility.UrlDecode(email);
			//From Email check that email
			UserEntity? existingUser = await _userService.FindOneByEmail(email);
			if (existingUser == null)
			{
				throw new HttpException(HttpStatusCode.NotFound, "User not Found");
			}

			//Update User 
			existingUser.IsActive = true;
			UserEntity updatedData = await _userService.ActivateUser(existingUser);
			UserResponseDTO responseData = new UserResponseDTO { Id = updatedData.id };
			HttpContext.Items["CustomMessage"] = "User Created Successfully";
			//Redirect user to login page in frontend
			return Redirect("http://localhost:3000/login");
		}

	}
}