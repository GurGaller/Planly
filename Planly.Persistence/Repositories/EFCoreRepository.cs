using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Planly.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace Planly.Persistence.Repositories
{
	internal abstract class EFCoreRepository<TEntity> : IRepository<TEntity>
		where TEntity : AggregateRoot<TEntity>
	{
		private readonly DbSet<TEntity> dbSet;

		public EFCoreRepository(CustomDbContext dbContext)
		{
			dbSet = dbContext.Set<TEntity>();
			DbContext = dbContext;
			Entities = new AggregateSet<TEntity>(dbContext);
		}

		protected CustomDbContext DbContext { get; }

		/// <summary>
		/// A queryable collection of <typeparamref name="TEntity"/>s, used to perform queries against
		/// the underlying database.
		/// This queryable includes all the related entities of the aggregates, and flushes
		/// the database automatically to avoid stale data in queries.
		/// </summary>
		/// <remarks>
		/// All queries made by subclasses of this class, must be done against this collection.
		/// Do not query a <see cref="DbContext"/> or a <see cref="DbSet{TEntity}"/> directly.
		/// </remarks>
		protected IQueryable<TEntity> Entities { get; }

		public void Add(TEntity entity)
		{
			dbSet.Add(entity);
		}

		public async Task<TEntity?> FindByIdAsync(Identifier<TEntity> id, CancellationToken cancellationToken = default)
		{
			return await Entities.SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
		}
	}
}