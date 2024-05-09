using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ApplicationDev.Common.Database.Base_Entity;

namespace ApplicationDev.Modules.Blogs.Entity
{
	public class BlogComment
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public int UserId { get; set; }
		[Required]
		public string Message { get; set; }

		public int? UpVote { get; set; }
		public int? DownVote { get; set; }
		public int BlogEntityId { get; set; }

		[ForeignKey("BlogEntityId")]
		public BlogEntity BlogEntity { get; set; }
	}
}