using ApplicationDev.Common.Database.BaseRepository;
using ApplicationDev.Modules.Admin.Entity;


namespace ApplicationDev.Modules.Admin.Repos
{
	public class AdminRepos : BaseRepository<AdminEntity>
	{

		public AdminRepos(MyAppDbContext context) : base(context)
		{
		}
	}
}