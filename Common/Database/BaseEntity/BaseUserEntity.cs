namespace ApplicationDev.Common.Database.BaseEntity
{

		public class BaseUserEntity : BaseEntity
		{
			public required string UserName { get; set; }
			public required string Password { get; set; }
		}

}
