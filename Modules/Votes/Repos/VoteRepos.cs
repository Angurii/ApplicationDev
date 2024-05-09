using ApplicationDev.Common.Database.BaseRepository;
using ApplicationDev.Modules.Votes.Entity;

namespace ApplicationDev.Modules.Votes.Repos
{
	public class VoteRepos : BaseRepository<VoteEntity>
	{
		public VoteRepos(MyAppDbContext context) : base(context) { }
	}
}