namespace ApplicationDev.Modules.Comments.DTOs
{
	public record CommentCreateDTO
	{

		public required string Message { get; init; }

	}
}
