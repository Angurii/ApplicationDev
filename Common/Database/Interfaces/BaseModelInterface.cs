namespace ApplicationDev.Common.Database.Interfaces
{
	public interface IBaseModelInterface
	{
		public int id { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public DateTime? DeletedAt { get; set; }
	}
}
