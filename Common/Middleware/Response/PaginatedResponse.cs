using ApplicationDev.Common.Interfaces;

namespace ApplicationDev.Common.Middleware.Response
{
	public class PaginatedResponse<T> : IPaginatedList<T>
	{
		public int PageNumber { get; set; }
		public int DataPerPage { get; set; }
		public int TotalCount { get; set; }
		public IEnumerable<T> Data { get; set; }

	}
}
