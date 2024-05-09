using ApplicationDev.Common.Database.Base_Entity;
using ApplicationDev.Modules.Blogs.Entity;
using ApplicationDev.Modules.Comments.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationDev.Modules.Votes.Entity
{
	public class VoteEntity : BaseEntity
	{
		public int? BlogId { get; set; }

		[ForeignKey("BlogId")]
		public BlogEntity? Blog { get; set; }
		public UserInfo VoteUser { get; set; }

		public int? CommentsId { get; set; }

		[ForeignKey("CommentsId")]
		public CommentsEntity? Comment { get; set; }
		public bool IsUpVote { get; set; }
	}
}
