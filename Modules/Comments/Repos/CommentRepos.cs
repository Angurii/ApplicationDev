using ApplicationDev.Common.Database.BaseRepository;
using ApplicationDev.Modules.Comments.Entity;
using Microsoft.EntityFrameworkCore;

namespace ApplicationDev.Modules.Comments.Repos
{
	public class CommentsRepos : BaseRepository<CommentsEntity>
	{
		private readonly MyAppDbContext _context;
		public CommentsRepos(MyAppDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<(CommentsEntity, List<CommentsEntity>)> GetCommentWithRepliesAsync(int commentId)
		{
			// Fetch the parent comment
			var parentComment = await _context.BlogComments
				.AsNoTracking()
				.FirstOrDefaultAsync(c => c.id == commentId);

			// Check if parent comment exists
			if (parentComment == null)
			{
				return (null, null);
			}

			// Fetch all replies for the comment
			var replies = await _context.BlogComments
				.Where(c => c.ParentCommentId == commentId)
				.AsNoTracking()
				.ToListAsync();

			return (parentComment, replies);
		}

	}
}