using ApplicationDev.Common.DTOs;
using ApplicationDev.Common.Exceptions;
using ApplicationDev.Modules.Admin.Entity;
using ApplicationDev.Modules.Admin.Services;
using ApplicationDev.Modules.Blogs.Entity;
using ApplicationDev.Modules.Blogs.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationDev.Common.Middleware.Authentication;
using ApplicationDev.Modules.Votes.DTOs;
using ApplicationDev.Modules.Votes.Entity;
using ApplicationDev.Modules.Votes.Services;
using System.Net;
using ApplicationDev.Modules.Comments.Entity;
using ApplicationDev.Modules.Comments.Services;

namespace ApplicationDev.Modules.Votes.Controller
{
	[ApiExplorerSettings(GroupName = "admin")]
	[Tags("Votes")]
	[Route("api/admin/vote")]
	public class AdminVoteController : ControllerBase
	{
		private readonly VoteService _voteService;
		private readonly AdminService _adminService;

		private readonly BlogService _blogService;

		private readonly CommentsService _commentsService;
		private readonly ILogger<AdminVoteController> _logger;

		public AdminVoteController(AdminService adminService, BlogService blogService, VoteService voteService, ILogger<AdminVoteController> logger, CommentsService commentsService)
		{
			_adminService = adminService;
			_blogService = blogService;
			_voteService = voteService;
			_commentsService = commentsService;
			_logger = logger;
		}

		[HttpPost("info-blog/{blogId}")]
		[ServiceFilter(typeof(RoleAuthentication))]
		public async Task<GetVoteResponseDTO?> GetInfoAboutBlogVotes(string blogId)
		{
			//First Get User Info
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

			//Get Blog Info
			BlogEntity? blogInfo = await _blogService.GetByIdAsync(int.Parse(blogId));

			if (blogInfo == null)
			{
				throw new HttpException(HttpStatusCode.NotFound, "Blog not found");
			}

			//Check if the user has already voted
			VoteEntity? existingVote = await _voteService.FindVoteByUserAndBlog(blogInfo.id, int.Parse(adminInfo.UserId));
			// if (existingVote != null)
			// {
			//     throw new HttpException(HttpStatusCode.BadRequest, "You have already voted");
			// }
			GetVoteResponseDTO returnData = new GetVoteResponseDTO()
			{
				Id = existingVote?.id ?? null,
				IsUpVote = existingVote?.IsUpVote ?? null
			};
			return returnData;
		}

		[HttpPost("upvote-blog/{blogId}")]
		[ServiceFilter(typeof(RoleAuthentication))]
		public async Task<VoteResponseDTO> BlogUpVote(string blogId)
		{


			//First Get User Info
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

			//Get Blog Info
			BlogEntity? blogInfo = await _blogService.GetByIdAsync(int.Parse(blogId));

			if (blogInfo == null)
			{
				throw new HttpException(HttpStatusCode.NotFound, "Blog not found");
			}

			//Check if the user has already voted
			VoteEntity? existingVote = await _voteService.FindVoteByUserAndBlog(blogInfo.id, int.Parse(adminInfo.UserId));

			_logger.LogInformation("Existing Vote" + existingVote);

			if (existingVote != null) //If the user has already voted
			{
				if (existingVote.IsUpVote == true)
				{
					//If user has already upvote don't give again
					throw new HttpException(HttpStatusCode.BadRequest, "You have already down voted");
				}

				if (existingVote.IsUpVote == false)
				{
					//Initial was upVote now change and it to downvote
					blogInfo.UpVote += 1;
					blogInfo.DownVote -= 1;
					await _blogService.UpdateFormOtherService(blogInfo);
					existingVote.IsUpVote = true;
					await _voteService.UpdateVote(existingVote);
					HttpContext.Items["CustomMessage"] = "Upvote Successfully";
					return new VoteResponseDTO { Id = blogInfo.id };
				}
			}

			//If the user has not voted before
			blogInfo.UpVote += 1;



			//Create Vote
			VoteEntity voteEntity = new VoteEntity()
			{
				BlogId = blogInfo.id,
				CommentsId = null,
				Comment = null,
				IsUpVote = true,
				Blog = blogInfo,
				VoteUser = new UserInfo { UserId = int.Parse(adminInfo.UserId), Name = adminInfo.Name },

			};

			VoteEntity createdVote = await _voteService.CreateVote(voteEntity);

			if (createdVote == null)
			{
				throw new HttpException(HttpStatusCode.BadRequest, "Vote not created");
			}
			_logger.LogInformation("Vote Created" + createdVote);
			//Updating the Blog
			blogInfo.Votes.Add(createdVote);
			await _blogService.UpdateFormOtherService(blogInfo);
			HttpContext.Items["CustomMessage"] = "Upvote Successfully";
			VoteResponseDTO responseData = new VoteResponseDTO { Id = blogInfo.id };
			return responseData;
		}

		[HttpPost("downvote-blog/{blogId}")]
		[ServiceFilter(typeof(RoleAuthentication))]
		public async Task<VoteResponseDTO> BlogDownVote(string blogId)
		{


			//First Get User Info
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

			//Get Blog Info
			BlogEntity? blogInfo = await _blogService.GetByIdAsync(int.Parse(blogId));

			if (blogInfo == null)
			{
				throw new HttpException(HttpStatusCode.NotFound, "Blog not found");
			}

			//Check if the user has already voted
			VoteEntity? existingVote = await _voteService.FindVoteByUserAndBlog(blogInfo.id, int.Parse(adminInfo.UserId));
			_logger.LogInformation("Existing Vote" + existingVote);
			//Trying to downvote 
			//If it is the user has already up vote now change it to down vote
			if (existingVote != null)
			{
				if (existingVote.IsUpVote == false)
				{
					throw new HttpException(HttpStatusCode.BadRequest, "You have already down voted");
				}

				if (existingVote.IsUpVote == true)
				{
					//Initial was upVote now change and it to downvote
					blogInfo.UpVote -= 1;
					blogInfo.DownVote += 1;
					await _blogService.UpdateFormOtherService(blogInfo);
					existingVote.IsUpVote = false;
					await _voteService.UpdateVote(existingVote);
					HttpContext.Items["CustomMessage"] = "DownVote Successfully";
					return new VoteResponseDTO { Id = blogInfo.id };

				}
			}

			//If the user has not voted before
			blogInfo.DownVote += 1;
			//Create Vote
			VoteEntity voteEntity = new VoteEntity()
			{
				BlogId = blogInfo.id,
				IsUpVote = false,
				Blog = blogInfo,
				VoteUser = new UserInfo { UserId = int.Parse(adminInfo.UserId), Name = adminInfo.Name },

			};

			VoteEntity createdVote = await _voteService.CreateVote(voteEntity);

			if (createdVote == null)
			{
				throw new HttpException(HttpStatusCode.BadRequest, "Vote not created");
			}
			_logger.LogInformation("Vote Created" + createdVote);
			//Updating the Blog

			blogInfo.Votes.Add(createdVote);
			await _blogService.UpdateFormOtherService(blogInfo);
			HttpContext.Items["CustomMessage"] = "DownVote Successfully";
			VoteResponseDTO responseData = new VoteResponseDTO { Id = blogInfo.id };
			return responseData;
		}


		[HttpPost("info-comment/{commentId}")]
		[ServiceFilter(typeof(RoleAuthentication))]
		public async Task<GetVoteResponseDTO?> GetInfoAboutCommentVotes(string commentId)
		{
			//First Get User Info
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

			//Get Blog Info
			CommentsEntity? commentInfo = await _commentsService.GetByIdAsync(int.Parse(commentId));

			if (commentInfo == null)
			{
				throw new HttpException(HttpStatusCode.NotFound, "Blog not found");
			}

			//Check if the user has already voted
			VoteEntity? existingVote = await _voteService.FindVoteByUserAndComment(commentInfo.id, int.Parse(adminInfo.UserId));
			// if (existingVote != null)
			// {
			//     throw new HttpException(HttpStatusCode.BadRequest, "You have already voted");
			// }
			GetVoteResponseDTO returnData = new GetVoteResponseDTO()
			{
				Id = existingVote?.id ?? null,
				IsUpVote = existingVote?.IsUpVote ?? null
			};
			return returnData;
		}

		[HttpPost("upvote-comment/{commentId}")]
		[ServiceFilter(typeof(RoleAuthentication))]
		public async Task<VoteResponseDTO> CommentUpVote(string commentId)
		{


			//First Get User Info
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

			//Get Blog Info
			CommentsEntity? commentInfo = await _commentsService.GetByIdAsync(int.Parse(commentId));

			if (commentInfo == null)
			{
				throw new HttpException(HttpStatusCode.NotFound, "Blog not found");
			}

			//Check if the user has already voted
			VoteEntity? existingVote = await _voteService.FindVoteByUserAndComment(commentInfo.id, int.Parse(adminInfo.UserId));

			_logger.LogInformation("Existing Vote" + existingVote);

			if (existingVote != null) //If the user has already voted
			{
				if (existingVote.IsUpVote == true)
				{
					//If user has already upvote don't give again
					throw new HttpException(HttpStatusCode.BadRequest, "You have already down voted");
				}

				if (existingVote.IsUpVote == false)
				{
					//Initial was upVote now change and it to downvote
					commentInfo.UpVote += 1;
					commentInfo.DownVote -= 1;
					await _commentsService.UpdateCommentsByOtherService(commentInfo);
					existingVote.IsUpVote = true;
					await _voteService.UpdateVote(existingVote);
					HttpContext.Items["CustomMessage"] = "Upvote Successfully";
					return new VoteResponseDTO { Id = commentInfo.id };
				}
			}

			//If the user has not voted before
			commentInfo.UpVote += 1;



			//Create Vote
			VoteEntity voteEntity = new VoteEntity()
			{
				CommentsId = commentInfo.id,
				IsUpVote = true,
				Comment = commentInfo,
				VoteUser = new UserInfo { UserId = int.Parse(adminInfo.UserId), Name = adminInfo.Name },

			};

			VoteEntity createdVote = await _voteService.CreateVote(voteEntity);

			if (createdVote == null)
			{
				throw new HttpException(HttpStatusCode.BadRequest, "Vote not created");
			}
			_logger.LogInformation("Vote Created" + createdVote);
			//Updating the Blog
			commentInfo.Votes.Add(createdVote);
			await _commentsService.UpdateCommentsByOtherService(commentInfo);
			HttpContext.Items["CustomMessage"] = "Upvote Successfully";
			VoteResponseDTO responseData = new VoteResponseDTO { Id = commentInfo.id };
			return responseData;
		}

		[HttpPost("downvote-comment/{commentId}")]
		[ServiceFilter(typeof(RoleAuthentication))]
		public async Task<VoteResponseDTO> CommentDownVote(string commentId)
		{


			//First Get User Info
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

			//Get Blog Info
			CommentsEntity? commentInfo = await _commentsService.GetByIdAsync(int.Parse(commentId));

			if (commentInfo == null)
			{
				throw new HttpException(HttpStatusCode.NotFound, "Blog not found");
			}

			//Check if the user has already voted
			VoteEntity? existingVote = await _voteService.FindVoteByUserAndComment(commentInfo.id, int.Parse(adminInfo.UserId));
			_logger.LogInformation("Existing Vote" + existingVote);
			//Trying to downvote 
			//If it is the user has already up vote now change it to down vote
			if (existingVote != null)
			{
				if (existingVote.IsUpVote == false)
				{
					throw new HttpException(HttpStatusCode.BadRequest, "You have already down voted");
				}

				if (existingVote.IsUpVote == true)
				{
					//Initial was upVote now change and it to downvote
					commentInfo.UpVote -= 1;
					commentInfo.DownVote += 1;
					await _commentsService.UpdateCommentsByOtherService(commentInfo);
					existingVote.IsUpVote = false;
					await _voteService.UpdateVote(existingVote);
					HttpContext.Items["CustomMessage"] = "DownVote Successfully";
					return new VoteResponseDTO { Id = commentInfo.id };

				}
			}

			//If the user has not voted before
			commentInfo.DownVote += 1;
			//Create Vote
			VoteEntity voteEntity = new VoteEntity()
			{
				Blog = null,
				BlogId = null,
				CommentsId = commentInfo.id,
				IsUpVote = false,
				Comment = commentInfo,
				VoteUser = new UserInfo { UserId = int.Parse(adminInfo.UserId), Name = adminInfo.Name },

			};

			VoteEntity createdVote = await _voteService.CreateVote(voteEntity);

			if (createdVote == null)
			{
				throw new HttpException(HttpStatusCode.BadRequest, "Vote not created");
			}
			_logger.LogInformation("Vote Created" + createdVote);
			//Updating the Blog

			commentInfo.Votes.Add(createdVote);
			await _commentsService.UpdateCommentsByOtherService(commentInfo);
			HttpContext.Items["CustomMessage"] = "DownVote Successfully";
			VoteResponseDTO responseData = new VoteResponseDTO { Id = commentInfo.id };
			return responseData;
		}

	}
}