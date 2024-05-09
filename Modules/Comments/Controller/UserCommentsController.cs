using ApplicationDev.Common.DTOs;
using ApplicationDev.Common.Exceptions;
using ApplicationDev.Modules.Blogs.Entity;
using ApplicationDev.Modules.Blogs.Services;
using ApplicationDev.Modules.User.Entity;
using ApplicationDev.Modules.User.Services;
using ApplicationDev.Modules.Comments.Services;
using ApplicationDev.Modules.Comments.Entity;
using ApplicationDev.Modules.Comments.DTOs;
using ApplicationDev.Common.Middleware.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApplicationDev.Modules.Comments.Controller
{
	[ApiExplorerSettings(GroupName = "user")]
	[Tags("Comments")]
	[Route("api/user/blogs")]
	public class UserCommentsController : ControllerBase
	{
		private readonly CommentsService _commentsService;

		private readonly BlogService _blogService;

		private readonly UserService _userService;

		public UserCommentsController(CommentsService commentsService, BlogService blogService, UserService userService)
		{
			_commentsService = commentsService;
			_blogService = blogService;
			_userService = userService;
		}

		[HttpPost("create/{blogId}")]
		[ServiceFilter(typeof(RoleAuthentication))]
		public async Task<CommonCommentResponseDTO> CreateComment([FromBody] CommentCreateDTO incomingData, string blogId)
		{
			string userId = (HttpContext.Items["UserId"] as string)!; //Since we are using the RoleAuthFilter, we can safely assume that the UserId is a string and never null
			int parseUserId = int.Parse(userId); // Convert the string to an int
			UserEntity? user = await _userService.GetUserByIdAsync(parseUserId);

			if (user == null)
			{
				throw new HttpException(HttpStatusCode.NotFound, "User not found");
			}
			var userInfo = new CommonUserDTO()
			{
				UserId = user.id.ToString(),
				Name = user.Name
			};

			BlogEntity? existingBlog = await _blogService.GetByIdAsync(int.Parse(blogId));
			if (existingBlog == null)
			{
				throw new HttpException(HttpStatusCode.NotFound, "Blog not found");
			}


			CommentsEntity result = await _commentsService.CreateCommentAsync(incomingData, existingBlog, userInfo);
			CommonCommentResponseDTO dataToResponse = new CommonCommentResponseDTO()
			{
				Id = result.id,
				BlogId = result.BlogId,
				CommentedUserName = result.CommentedUserName,
				Message = result.Message,

			};

			HttpContext.Items["CustomMessage"] = "Comment Created Successfully";
			return dataToResponse;

		}

		[HttpPost("update/{commentId}")]
		[ServiceFilter(typeof(RoleAuthentication))]
		public async Task<CommonCommentResponseDTO> UpdateComment([FromBody] UpdateCommentDTO incomingData, string commentId)
		{
			string userId = (HttpContext.Items["UserId"] as string)!; //Since we are using the RoleAuthFilter, we can safely assume that the UserId is a string and never null
			int parseUserId = int.Parse(userId); // Convert the string to an int
			UserEntity? user = await _userService.GetUserByIdAsync(parseUserId);

			if (user == null)
			{
				throw new HttpException(HttpStatusCode.NotFound, "User not found");
			}

			//If User its User Comment then he can update it
			CommentsEntity? existingComment = await _commentsService.GetByIdAsync(int.Parse(commentId));

			if (existingComment == null)
			{
				throw new HttpException(HttpStatusCode.NotFound, "Comment not found");
			}

			//Check if the user is the owner of the comment
			if (existingComment.CommentedUserId != user.id)
			{
				throw new HttpException(HttpStatusCode.Forbidden, "Sorry Cannot Edit Others Comment");
			}

			//if own then get the to edit the comment
			CommentsEntity updatedComment = await _commentsService.UpdateComments(existingComment, incomingData);

			CommonCommentResponseDTO dataToResponse = new CommonCommentResponseDTO()
			{
				Id = updatedComment.id,
				BlogId = updatedComment.BlogId,
				CommentedUserName = updatedComment.CommentedUserName,
				Message = updatedComment.Message,

			};

			return dataToResponse;

		}

		[HttpPost("reply/{commentId}")]
		[ServiceFilter(typeof(RoleAuthentication))]
		public async Task<CommonCommentResponseDTO> ReplyComment([FromBody] CommentCreateDTO incomingData, string commentId)
		{
			string userId = (HttpContext.Items["UserId"] as string)!; //Since we are using the RoleAuthFilter, we can safely assume that the UserId is a string and never null
			int parseUserId = int.Parse(userId); // Convert the string to an int
			UserEntity? user = await _userService.GetUserByIdAsync(parseUserId);

			//Check if User is Null or Not
			if (user == null)
			{
				throw new HttpException(HttpStatusCode.NotFound, "User not found");
			}

			//Create User Information
			CommonUserDTO userInfo = new CommonUserDTO()
			{
				UserId = user.id.ToString(),
				Name = user.Name
			};

			//Check if the Parent Comment Exists or not
			CommentsEntity? parentComment = await _commentsService.GetByIdAsync(int.Parse(commentId));

			if (parentComment == null)
			{
				throw new HttpException(HttpStatusCode.NotFound, "Comment not found");
			}

			//Check if the Blog Exists of not
			BlogEntity? existingBlog = await _blogService.GetByIdAsync(parentComment.BlogId);
			if (existingBlog == null)
			{
				throw new HttpException(HttpStatusCode.NotFound, "Blog not found");
			}

			CommentsEntity result = await _commentsService.ReplyCommentAsync(incomingData, parentComment, userInfo);

			CommonCommentResponseDTO dataToResponse = new CommonCommentResponseDTO()
			{
				Id = result.id,
				BlogId = result.BlogId,
				CommentedUserName = result.CommentedUserName,
				Message = result.Message,

			};

			HttpContext.Items["CustomMessage"] = "Comment Reply Successfully";
			return dataToResponse;

		}

		[HttpGet("comment/{comment}")]
		[ServiceFilter(typeof(RoleAuthentication))]

		public async Task<CommentsGetResponseDTO> GetCommentsById(string comment)
		{
			CommentsGetResponseDTO? commentDto = await _commentsService.GetCommentWithReplies(int.Parse(comment));
			if (commentDto == null)
			{
				throw new HttpException(HttpStatusCode.NotFound, "Comment with that id was not found");
			}
			return commentDto;
		}
	}
}