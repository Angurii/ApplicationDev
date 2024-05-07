using ApplicationDev.Modules.User.DTOs;
using ApplicationDev.Modules.User.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApplicationDev.Common.Middlewares.Authentication;
using ApplicationDev.Modules.User.Entity;
namespace ApplicationDev.Modules.User.Controllers
{
	[ApiExplorerSettings(GroupName = "user")] //Provides metadata about the API Explorer group that an action belongs to.
	[Tags("Users")]
	[Route("api/user/user/")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly UserService _userService;

		public UserController(UserService userService)
		{
			_userService = userService;
		}



		[HttpPost("register")]
		// [ServiceFilter(typeof(RoleAuthFilter))]
		public async Task<IActionResult> CreateUser(UserCreateDTO incomingData)
		{
			try
			{
				UserEntity result = await _userService.CreateUser(incomingData);
				UserResponseDTO responseData = new UserResponseDTO { Id = result.id };
				// return Created($"/api/users/{result.Id}", result);
				HttpContext.Items["CustomMessage"] = "Please verify your email to continue";
				return Created("", responseData);
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}