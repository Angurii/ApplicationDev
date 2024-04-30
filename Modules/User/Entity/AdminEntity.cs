using ApplicationDev.Common.Database.BaseEntity;

namespace ApplicationDev.Modules.Admin.Entity
{
	public class AdminEntity : BaseUserEntity
	{
		public required string Name { get; set; }

	}
}