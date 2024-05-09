namespace ApplicationDev.Modules.Votes.DTOs
{
	public record VoteCreateDTO
	{
		public int BlogId { get; init; }
		public bool IsUpVote { get; init; }
	}
}
