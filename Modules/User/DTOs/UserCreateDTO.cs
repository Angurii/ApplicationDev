namespace ApplicationDev.Modules.User.DTOs
{
	public record UserCreateDTO
	{
		public required string Name { get; set; }
		public required string UserName { get; set; }
		public required string Password { get; set; }


	}
}
