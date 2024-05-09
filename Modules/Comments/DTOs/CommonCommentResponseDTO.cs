namespace ApplicationDev.Modules.Comments.DTOs
{
	public class CommonCommentResponseDTO
	{
		public required int Id { get; set; }

		public required int BlogId { get; set; }
		public required string CommentedUserName { get; set; }
		public required string Message { get; set; }
	}
}