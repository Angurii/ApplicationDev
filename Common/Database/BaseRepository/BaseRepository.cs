using System.Collections.Generic;
using System.Linq.Expressions;
using ApplicationDev.Common.Database.BaseEntity;
using ApplicationDev.Common.Database.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApplicationDev.Common.Database.BaseRepository
{
	public class BaseRepository<T> : IDatabaseBaseInterface<T> where T : class
	{
		protected readonly DbContext _context;
		private readonly DbSet<T> _dbSet;

		public BaseRepository(DbContext context)
		{
			_context = context;
			_dbSet = context.Set<T>();
		}
		//CRUD Operation
		public async Task<IEnumerable<T>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}

		public async Task<T?> FindByIdAsync(int id)
		{
			return await _dbSet.FindAsync(id);
		}

		public async Task<T?> FindOne(Expression<Func<T, bool>> predicate)
		{
			return await _dbSet.FirstOrDefaultAsync(predicate);
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

	}
}
