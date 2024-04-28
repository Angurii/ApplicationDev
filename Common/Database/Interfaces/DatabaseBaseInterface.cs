using System.Linq.Expressions;
using ApplicationDev.Common.Database.BaseEntity;
namespace ApplicationDev.Common.Database.Interfaces
{
	//Abstract of what my Base Repository should have
	public interface IDatabaseBaseInterface<T> where T : class  //T must be a class, an interface
	{

		Task<IEnumerable<T>> GetAllAsync(); // Returns collection of type T
		Task<T?> FindByIdAsync(int id); //Takes an id and Returns of type T or null

		// Returns either an instance of T or null
		Task<T?> FindOne(Expression<Func<T, bool>> predicate); //Accepts lambda expression that takes an entity of type T and returns a bool

		Task<T> CreateAsync(T entity, bool UseTransaction = false); //Optional Parameter

		Task<T> UpdateAsync(T entity, bool UseTransaction = false); //Optional Parameter

		Task<T> DeleteAsync(T entity, bool UseTransaction = false);

		Task<T> SoftDeleteAsync(T entity, bool UseTransaction = false);


	}	
	}

