namespace ApplicationDev.Modules.Votes.DTOs
{
	public class VoteResponseDTO
	{
		public required int Id { get; set; }


	}

	public class GetVoteResponseDTO
	{
		public int? Id { get; set; }

		public bool? IsUpVote { get; set; }
	}
}
