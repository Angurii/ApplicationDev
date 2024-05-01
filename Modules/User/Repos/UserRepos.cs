using ApplicationDev.Common.Database.BaseRepository;

using ApplicationDev.Modules.User.Entity;

namespace ApplicationDev.Modules.User.Repos
{
	public class UserRepository : BaseRepository<UserEntity>
	{
		public UserRepository(MyAppDbContext context) : base(context) { }


	}
}
