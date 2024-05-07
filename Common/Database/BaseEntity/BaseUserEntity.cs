
using System.ComponentModel.DataAnnotations;

namespace ApplicationDev.Common.Database.BaseEntity
{
	public class BaseUserEntity : BaseEntity
	{
		[Required]
		public string UserName { get; set; }

		[Required]
		public string Password { get; set; }

		public bool? IsActive { get; set; }

		public BaseUserEntity()
		{
			IsActive = false;
		}
	}
}

