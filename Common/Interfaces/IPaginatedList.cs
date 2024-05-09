namespace ApplicationDev.Common.Interfaces
{
	public interface IPaginatedList<T>
	{
		public int PageNumber { get; set; }
		public int DataPerPage { get; set; }
		public IEnumerable<T> Data { get; set; }
		public int TotalCount { get; set; }
	}
}
