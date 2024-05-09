using ApplicationDev.Modules.Votes.Entity;
using ApplicationDev.Modules.Votes.Repos;
namespace ApplicationDev.Modules.Votes.Services
{
	public class VoteService
	{
		private readonly VoteRepos _voteRepo;

		public VoteService(VoteRepos voteRepo)
		{
			_voteRepo = voteRepo;
		}

		public async Task<VoteEntity> CreateVote(VoteEntity voteEntity)
		{
			return await _voteRepo.CreateAsync(voteEntity);
		}

		public async Task<VoteEntity?> FindVoteByUserAndBlog(int blogId, int userId)
		{
			return await _voteRepo.FindOne(entity => entity.BlogId == blogId && entity.VoteUser.UserId == userId);
		}

		// public async Task<VoteEntity?> FindVoteByUserAndBlogAndDecideToVote(int blogId, int userId, bool isUpVote)
		// {
		//     return await _voteRepo.FindOne(entity => entity.BlogId == blogId && entity.VoteUser.UserId == userId && entity.IsUpVote == isUpVote);
		// }

		public async Task<VoteEntity> UpdateVote(VoteEntity voteEntity)
		{
			return await _voteRepo.UpdateAsync(voteEntity);
		}

		public async Task<VoteEntity?> FindVoteByUserAndComment(int commentId, int userId)
		{
			return await _voteRepo.FindOne(entity => entity.CommentsId == commentId && entity.VoteUser.UserId == userId);
		}
	}
}