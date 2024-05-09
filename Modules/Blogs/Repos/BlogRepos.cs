using ApplicationDev.Common.Database.BaseRepository;
using ApplicationDev.Modules.Blogs.Entity;

namespace ApplicationDev.Modules.Blogs.Repos
{
	public class BlogRepos : BaseRepository<BlogEntity>
	{
		// private readonly MyAppDbContext _context;
		private readonly ILogger<BlogRepos> _logger;
		public BlogRepos(MyAppDbContext context, ILogger<BlogRepos> logger) : base(context)
		{
			// _context = context;
			_logger = logger;
		}

		// public async Task<BlogEntity?> GetByIdWithVotesAsync(int id)
		// {
		//     BlogEntity? data = await _context.Blogs
		//     .Include(b => b.Votes)
		//     .SingleOrDefaultAsync(b => b.id == id);
		//     return data;
		// }

	}
}