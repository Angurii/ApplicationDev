using System.Net;
using ApplicationDev.Common.DTOs;
using ApplicationDev.Common.Exceptions;
using ApplicationDev.Modules.Admin.Entity;
using ApplicationDev.Modules.Admin.Services;
using ApplicationDev.Modules.Blogs.DTOs;
using ApplicationDev.Modules.Blogs.Entity;
using ApplicationDev.Modules.Blogs.Services;
using ApplicationDev.Common.Middlewares.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace CourseWork.Modules.Blogs.Controller
{
	[ApiExplorerSettings(GroupName = "admin")]
	[Tags("Blogs")]
	[Route("api/admin/blogs")]
	public class AdminBlogController : ControllerBase
	{
		private readonly BlogService _blogService;
		private readonly ILogger<AdminBlogController> _logger;

		private readonly AdminService _adminService;

		public AdminBlogController(BlogService blogService, ILogger<AdminBlogController> logger, AdminService adminService)
		{
			_blogService = blogService;
			_logger = logger;
			_adminService = adminService;
		}

		[HttpPost("create")]
		[ServiceFilter(typeof(RoleAuthentication))]
		public async Task<IActionResult> CreateBlogs([FromBody] BlogCreateDto incomingData)
		{
			string userId = (HttpContext.Items["UserId"] as string)!; //Since we are using the RoleAuthFilter, we can safely assume that the UserId is a string and never null
			int parseUserId = int.Parse(userId); // Convert the string to an int
			AdminEntity? adminUser = await _adminService.GetUserByIdAsync(parseUserId);

			if (adminUser == null)
			{
				throw new HttpException(HttpStatusCode.NotFound, "Admin not found");
			}
			var adminInfo = new CommonUserDTO()
			{
				UserId = adminUser.id.ToString(),
				Name = adminUser.UserName
			};
			BlogEntity result = await _blogService.CreateBlogs(incomingData, adminInfo);
			return Created("", result);
		}

	}
}