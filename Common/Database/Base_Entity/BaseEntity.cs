using ApplicationDev.Common.Database.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ApplicationDev.Common.Database.Base_Entity
{
	public class BaseEntity : IBaseModelInterface
	{
		[Key]
		public int id { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public DateTime? DeletedAt { get; set; }


		//Initalizing Default Value
		public BaseEntity()
		{
			CreatedAt = DateTime.UtcNow;
			UpdatedAt = DateTime.UtcNow;
			DeletedAt = null;
		}

	}
}
