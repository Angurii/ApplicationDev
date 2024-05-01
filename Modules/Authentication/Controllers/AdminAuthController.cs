using ApplicationDev.Common.Exceptions;
using ApplicationDev.Modules.Admin.Entity;
using ApplicationDev.Modules.Admin.Services;
using ApplicationDev.Modules.Authentication.DTOs;
using ApplicationDev.Modules.Authentication.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApplicationDev.Modules.Authentication.Controllers
{
	[ApiExplorerSettings(GroupName = "admin")]
	[Tags("Auth")]
	[Route("api/admin/auth/")]
	public class AdminAuthController : ControllerBase
	{
		private readonly AuthenticationService _authService;
		private readonly AdminService _adminService;

		private readonly ILogger<UserAuthController> _logger;

		public AdminAuthController(AuthenticationService authService, AdminService adminService, ILogger<UserAuthController> logger)
		{
			_authService = authService;
			_adminService = adminService;
			_logger = logger;
		}
		[HttpPost("login")]

		async public Task<IActionResult> Login([FromBody] UserLoginDTO incomingData)
		{
			try
			{
				AdminEntity? user = await _adminService.FindOne(incomingData.UserName);
				if (user == null)
				{
					throw new HttpException(HttpStatusCode.NotFound, "Admin Not Found");
				}

				HttpContext.Items["CustomMessage"] = "LoggedIn Successfully";

				return Ok(_authService.Login(user, incomingData, "admin"));

			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}