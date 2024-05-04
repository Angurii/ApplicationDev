namespace ApplicationDev.Modules.Authentication.DTOs
{
	public record UserLoginDTO
	{
		public string? Email { get; set; }
		public string? UserName { get; set; }
		public required string Password { get; set; }
	}
}
