using ApplicationDev.Common.Database.BaseRepository;
using ApplicationDev.Modules.Blogs.Entity;

namespace ApplicationDev.Modules.Blogs.Repos
{

    public class BlogRepository : BaseRepository<BlogEntity>
	{
		public BlogRepository(MyAppDbContext context) : base(context) { }
	}
}