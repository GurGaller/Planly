using System.Threading;
using System.Threading.Tasks;
using Planly.Application.Tranactions;

namespace Planly.Persistence
{
	/// <summary>
	/// Creates unit of work that wrap DB transactions.
	/// </summary>
	/// <seealso cref="IUnitOfWorkFactory" />
	internal class EFCoreUnitOfWorkFactory : IUnitOfWorkFactory
	{
		private readonly CustomDbContext dbContext;

		/// <summary>
		/// Initializes a new instance of the <see cref="EFCoreUnitOfWorkFactory"/> class.
		/// </summary>
		/// <param name="dbContext">The database context.</param>
		public EFCoreUnitOfWorkFactory(CustomDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		/// <inheritdoc/>
		public async Task<IUnitOfWork> CreateAsync(CancellationToken cancellationToken = default)
		{
			var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
			return new EFCoreUnitOfWork(dbContext, transaction);
		}
	}
}