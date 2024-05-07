using System.ComponentModel.DataAnnotations;
using ApplicationDev.Common.Database.BaseEntity;

namespace ApplicationDev.Modules.User.Entity

{
	public class UserEntity : BaseUserEntity
	{
		[Required]
		public string Name { get; set; }
		[Required]
		public string Email { get; set; }

	}
}