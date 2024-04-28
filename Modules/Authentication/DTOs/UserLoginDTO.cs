namespace ApplicationDev.Modules.Authentication.DTOs
{
	public record UserLoginDTO
	{
		public required string UserName { get; set; }
		public required string Password { get; set; }
	}
}
