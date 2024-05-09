using System.Collections.Generic;
using System.Linq.Expressions;
using ApplicationDev.Common.Constants.Enums;
using ApplicationDev.Common.Database.Base_Entity;
using ApplicationDev.Common.Database.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ApplicationDev.Common.Middleware.Response;

namespace ApplicationDev.Common.Database.BaseRepository
{
	public class BaseRepository<T> : IDatabaseBaseInterface<T> where T : BaseEntity
	{
		protected readonly DbContext _context;
		private readonly DbSet<T> _dbSet;


		public BaseRepository(DbContext context)
		{
			_context = context;
			_dbSet = context.Set<T>();
		}

		//CRUD
		public async Task<IEnumerable<T>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}

		public async Task<PaginatedResponse<T>> GetAllPaginatedAsync(int pageNumber, ShortByEnum shortBy)
		{
			int dataPerPage = 20; //Get through enum or constant
			var totalCount = await _dbSet.Where(entity => entity.DeletedAt == null).CountAsync();
			IQueryable<T> sortedData;


			if (shortBy == ShortByEnum.Latest)
			{
				sortedData = _dbSet.Where(entity => entity.DeletedAt == null).OrderByDescending(entity => entity.CreatedAt);
			}
			else // SortOrder.Oldest
			{
				sortedData = _dbSet.Where(entity => entity.DeletedAt == null).OrderBy(entity => entity.CreatedAt);
			}

			IEnumerable<T> data = await sortedData.Skip((pageNumber - 1) * dataPerPage).Take(dataPerPage).ToListAsync();
			return new PaginatedResponse<T>
			{
				PageNumber = pageNumber,
				DataPerPage = dataPerPage,
				TotalCount = totalCount,
				Data = data,
			};

		}

		public async Task<T?> FindByIdAsync(int id)
		{

			return await _dbSet.Where(entity => entity.DeletedAt == null).SingleOrDefaultAsync(entity => entity.id == id);
		}

		public async Task<T?> FindOne(Expression<Func<T, bool>> predicate)
		{
			return await _dbSet.Where(entity => entity.DeletedAt == null).FirstOrDefaultAsync(predicate);
		}

		public async Task<T> CreateAsync(T entity, bool UseTransaction = false)
		{
			if (UseTransaction)
			{
				using var transaction = await _context.Database.BeginTransactionAsync();
				try
				{
					await _dbSet.AddAsync(entity);
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();

					return entity;
				}
				catch
				{
					await transaction.RollbackAsync();
					throw;
				}
			}
			else
			{
				await _dbSet.AddAsync(entity);
				await _context.SaveChangesAsync();
				return entity;
			}
		}

		public async Task<T> UpdateAsync(T entity, bool useTransaction = false)
		{
			if (useTransaction)
			{
				using var transaction = await _context.Database.BeginTransactionAsync();
				try
				{
					_dbSet.Update(entity);
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();

					return entity;
				}
				catch
				{
					await transaction.RollbackAsync();
					throw;
				}
			}
			else
			{
				_dbSet.Update(entity);
				await _context.SaveChangesAsync();
				return entity;
			}
		}
		public async Task<T> DeleteAsync(T entity, bool useTransaction = false)
		{
			if (useTransaction)
			{
				using var transaction = await _context.Database.BeginTransactionAsync();
				try
				{
					_dbSet.Remove(entity);
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();

					return entity;
				}
				catch
				{
					await transaction.RollbackAsync();
					throw;
				}
			}
			else
			{
				_dbSet.Remove(entity);
				await _context.SaveChangesAsync();
				return entity;
			}
		}

		// public async Task<T> SoftDeleteAsync(T entity)
		// {
		//     return await UpdateAsync(entity);
		// }

		public async Task<T> SoftDeleteAsync(T entity, bool useTransaction = false)
		{
			if (useTransaction)
			{
				using var transaction = await _context.Database.BeginTransactionAsync();
				try
				{
					_context.Update(entity);
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
				}
				catch
				{
					await transaction.RollbackAsync();
					throw;
				}
			}
			else
			{
				_context.Update(entity);
				await _context.SaveChangesAsync();

			}
			return entity;
		}

		public async Task<T?> FindByIdIncludingDeletedAsync(int id)
		{
			return await _dbSet.SingleOrDefaultAsync(entity => entity.id == id);
		}


	}
}