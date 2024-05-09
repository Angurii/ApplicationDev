using ApplicationDev.Common.DTOs;
using ApplicationDev.Common.Exceptions;
using ApplicationDev.Modules.Blogs.Entity;
using ApplicationDev.Modules.Comments.DTOs;
using ApplicationDev.Modules.Comments.Entity;
using ApplicationDev.Modules.Comments.Repos;
using System.Net;

namespace ApplicationDev.Modules.Comments.Services
{
	public class CommentsService
	{
		private readonly CommentsRepos _commentsRepo;
		public CommentsService(CommentsRepos commentsRepo)
		{
			_commentsRepo = commentsRepo;
		}


		public async Task<CommentsEntity> CreateCommentAsync(CommentCreateDTO commentCreateDto, BlogEntity blog, CommonUserDTO userInfo)
		{
			CommentsEntity comment = new CommentsEntity
			{

				CommentedUserId = int.Parse(userInfo.UserId),
				CommentedUserName = userInfo.Name,
				Message = commentCreateDto.Message,
				BlogId = blog.id,
				BlogEntity = blog,
				ParentCommentId = null,
				ParentComment = null
			};
			await _commentsRepo.CreateAsync(comment);
			return comment;
		}

		public async Task<CommentsEntity> ReplyCommentAsync(CommentCreateDTO commentReplyDto, CommentsEntity parentComment, CommonUserDTO userInfo)
		{
			CommentsEntity comment = new CommentsEntity
			{

				CommentedUserId = int.Parse(userInfo.UserId),
				CommentedUserName = userInfo.Name,
				Message = commentReplyDto.Message,
				BlogId = parentComment.BlogId,
				BlogEntity = parentComment.BlogEntity,
				ParentCommentId = parentComment.id,
				ParentComment = parentComment
			};
			await _commentsRepo.CreateAsync(comment);
			return comment;
		}

		public async Task<CommentsEntity> UpdateComments(CommentsEntity commentsEntity, UpdateCommentDTO incomingData)
		{
			if (incomingData.Message != null)
			{
				commentsEntity.Message = incomingData.Message;
			}
			return await _commentsRepo.UpdateAsync(commentsEntity);
		}

		public async Task<CommentsEntity> UpdateCommentsByOtherService(CommentsEntity commentsEntity)
		{

			return await _commentsRepo.UpdateAsync(commentsEntity);
		}


		public async Task<CommentsEntity?> GetByIdAsync(int id)
		{
			return await _commentsRepo.FindByIdAsync(id);

		}

		public async Task<CommentsGetResponseDTO> GetCommentWithReplies(int commentId)
		{
			var (parentComment, replies) = await _commentsRepo.GetCommentWithRepliesAsync(commentId);

			if (parentComment == null)
			{
				throw new HttpException(HttpStatusCode.NotFound, "Comment not found");
			}

			CommentsGetResponseDTO result = new CommentsGetResponseDTO
			{
				Id = parentComment.id,
				CommentedUserName = parentComment.CommentedUserName,
				Message = parentComment.Message,
				Replies = replies.Select(r => new ReplyDto
				{
					Message = r.Message,
					CommentedUserName = r.CommentedUserName
				}).ToList()
			};

			return result;
		}
	}
}