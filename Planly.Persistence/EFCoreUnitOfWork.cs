using System.Threading;
using System.Threading.Tasks;
using Planly.Application.Tranactions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Planly.Persistence
{
	/// <summary>
	/// Manages DB transactions.
	/// </summary>
	/// <seealso cref="IUnitOfWork" />
	internal class EFCoreUnitOfWork : IUnitOfWork
	{
		private readonly CustomDbContext dbContext;
		private readonly IDbContextTransaction transaction;

		/// <summary>
		/// Initializes a new instance of the <see cref="EFCoreUnitOfWork"/> class.
		/// </summary>
		/// <param name="dbContext">The database context.</param>
		/// <param name="transaction">A transaction.</param>
		public EFCoreUnitOfWork(CustomDbContext dbContext, IDbContextTransaction transaction)
		{
			this.dbContext = dbContext;
			this.transaction = transaction;
		}

		/// <summary>
		/// Commits all the changes to the database.
		/// </summary>
		/// <param name="cancellationToken">A token for canceling the operation.</param>
		public async Task CompleteAsync(CancellationToken cancellationToken = default)
		{
			dbContext.StoreDomainEventsOfChangedEntities();
			await dbContext.SaveChangesAsync(cancellationToken);
			await transaction.CommitAsync(cancellationToken);
		}

		/// <inheritdoc/>
		public ValueTask DisposeAsync()
		{
			return transaction.DisposeAsync();
		}
	}
}