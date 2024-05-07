using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ApplicationDev.Common.Database.BaseEntity;

namespace ApplicationDev.Modules.Blogs.Entity
{
	public class BlogEntity : BaseEntity
	{
		public required string Title { get; set; }
		[Column(TypeName = "nvarchar(MAX)")]
		public required string Content { get; set; }

		public required UserInfo PostUser { get; set; }
		public required string ImgUrl { get; set; }
		public required int UpVote { get; set; }
		public required int DownVote { get; set; }
		public ICollection<BlogComment> Comments { get; set; } = new List<BlogComment>();


	}
	public class UserInfo()
	{
		public required string UserId { get; set; }
		public required string Name { get; set; }
	}
}