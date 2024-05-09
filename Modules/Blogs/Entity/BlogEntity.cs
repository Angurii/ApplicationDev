using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ApplicationDev.Common.Database.Base_Entity;
using ApplicationDev.Modules.Votes.Entity;
using ApplicationDev.Modules.Comments.Entity;

namespace ApplicationDev.Modules.Blogs.Entity
{
	public class BlogEntity : BaseEntity
	{
		public required string Title { get; set; }
		[Column(TypeName = "nvarchar(MAX)")]
		public required string Content { get; set; }

		public required UserInfo PostUser { get; set; }
		public required string ImgUrl { get; set; }
		public int UpVote { get; set; } = 0;
		public int DownVote { get; set; } = 0;
		public ICollection<VoteEntity> Votes { get; set; } = new List<VoteEntity>();
		public ICollection<CommentsEntity> Comments { get; set; } = new List<CommentsEntity>();


	}
	public class UserInfo()
	{
		public required int UserId { get; set; }
		public required string Name { get; set; }
	}


}