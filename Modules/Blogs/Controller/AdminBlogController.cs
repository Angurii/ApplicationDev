﻿using System.Net;
using ApplicationDev.Common.DTOs;
using ApplicationDev.Common.Exceptions;
using ApplicationDev.Modules.Admin.Entity;
using ApplicationDev.Modules.Admin.Services;
using ApplicationDev.Modules.Blogs.DTOs;
using ApplicationDev.Modules.Blogs.Entity;
using ApplicationDev.Modules.Blogs.Services;
using ApplicationDev.Common.Middleware.Authentication;
using Microsoft.AspNetCore.Mvc;
using ApplicationDev.Common.Constants.Enums;
using ApplicationDev.Common.Middleware.Response;

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

			HttpContext.Items["CustomMessage"] = "Blog Created Successfully";
			return Created("", result);
		}

		[HttpGet("list")]
		[ServiceFilter(typeof(RoleAuthentication))]
		public async Task<PaginatedResponse<BlogEntity>> GetBlogList([FromQuery] string? page = "1", [FromQuery] ShortByEnum shortBy = ShortByEnum.Latest)  //ToDoType
		{
			//Getting page from query
			//  page = HttpContext.Request.Query["page"].ToString();
			_logger.LogInformation("SortBy: " + shortBy);

			int parsePage = int.Parse(page);

			PaginatedResponse<BlogEntity> result = await _blogService.GetPaginatedBlogList(parsePage, shortBy);

			HttpContext.Items["CustomMessage"] = "Blog List Successfully";
			return result;
		}


		[HttpGet("info/{blog}")]
		[ServiceFilter(typeof(RoleAuthentication))]
		public async Task<BlogEntity> GetBlogById(string blog)
		{
			BlogEntity? existingBlog = await _blogService.GetByIdAsync(int.Parse(blog));
			if (existingBlog == null)
			{
				throw new HttpException(HttpStatusCode.NotFound, "Blog with that id was not found");
			}

			HttpContext.Items["CustomMessage"] = "Blog Successfully Get";
			return existingBlog;
		}

		[HttpPost("update/{blog}")]
		[ServiceFilter(typeof(RoleAuthentication))]
		public async Task<BlogEntity> UpdateBlogs(string blog, [FromBody] BlogUpdateDTO incomingData)
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

			//Check if that blog exists or not
			BlogEntity? existingBlog = await _blogService.GetByIdAsync(int.Parse(blog));
			if (existingBlog == null)
			{
				throw new HttpException(HttpStatusCode.NotFound, "Blog with that id was not found");
			}

			BlogEntity result = await _blogService.UpdateBlogs(incomingData, adminInfo, existingBlog);

			HttpContext.Items["CustomMessage"] = "Blog Updated Successfully";
			return result;
		}

		[HttpDelete("soft-delete/{blog}")]
		[ServiceFilter(typeof(RoleAuthentication))]
		public async Task<BlogEntity> SoftDeleteBlog(string blog)
		{

			return await _blogService.SoftDeleteBlog(int.Parse(blog));
		}

		[HttpPost("restore/{blog}")]
		[ServiceFilter(typeof(RoleAuthentication))]
		public async Task<BlogEntity> RestoreBlog(string blog)
		{

			return await _blogService.RestoreBlog(int.Parse(blog));
		}


		[HttpDelete("hard-delete/{blog}")]
		[ServiceFilter(typeof(RoleAuthentication))]
		public async Task<BlogEntity> HardDelete(string blog)
		{

			return await _blogService.HardDelete(int.Parse(blog));
		}
	}
}