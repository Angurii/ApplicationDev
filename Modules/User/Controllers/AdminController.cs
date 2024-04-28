using ApplicationDev.Modules.User.DTOs;
using ApplicationDev.Modules.User.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationDev.Modules.User.Controllers
{
	[ApiExplorerSettings(GroupName = "admin")] //Provides metadata about the API Explorer group that an action belongs to.
	[Tags("Users")]
	[Route("api/admin/user")]
	[ApiController]
	public class UserAdminsController : ControllerBase
	{
		private readonly UserService _userService;
		public UserAdminsController(UserService userService)
		{
			_userService = userService;
		}


		[HttpPost("register")]
		public async Task<IActionResult> CreateUser(UserCreateDTO incomingData)
		{
			var result = await _userService.CreateUser(incomingData);
			return Ok(result);
		}
	}
}
