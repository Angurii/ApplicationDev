using ApplicationDev.Common.Database.BaseRepository;
using ApplicationDev.Modules.Admin.Entity;

namespace ApplicationDev.Modules.Admin.Repos
{
	public class AdminRepository : BaseRepository<AdminEntity>
	{

		public AdminRepository(MyAppDbContext context) : base(context)
		{
		}
	}
}