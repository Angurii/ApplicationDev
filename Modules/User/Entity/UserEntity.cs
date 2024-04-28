using ApplicationDev.Common.Database.BaseEntity;

namespace ApplicationDev.Modules.User.Entity
{
	public class UserEntity : BaseUserEntity
	{
		public required string Name { get; set; }

	}
}
