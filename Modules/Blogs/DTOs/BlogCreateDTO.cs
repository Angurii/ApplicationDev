namespace ApplicationDev.Modules.Blogs.DTOs
{
	public class BlogCreateDto
	{

		public required string Title { get; set; }

		public required string Content { get; set; }
		public required string ImgUrl { get; set; }
		public required int UpVote { get; set; }
		public required int DownVote { get; set; }

	}

	public class BlogCreateServiceDto : BlogCreateDto
	{
		public required string UserId { get; set; }
	}
}