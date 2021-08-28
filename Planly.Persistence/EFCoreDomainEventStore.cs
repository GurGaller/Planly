using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Planly.Application.DomainEvents;
using Planly.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace Planly.Persistence
{
	/// <summary>
	/// An implementation of an event store using EF Core.
	/// </summary>
	/// <seealso cref="IDomainEventStore" />
	internal class EFCoreDomainEventStore : IDomainEventStore
	{
		private readonly CustomDbContext dbContext;

		/// <summary>
		/// Initializes a new instance of the <see cref="EFCoreDomainEventStore"/> class.
		/// </summary>
		/// <param name="dbContext">The database context.</param>
		public EFCoreDomainEventStore(CustomDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		/// <inheritdoc/>
		public async Task<IReadOnlyList<DomainEvent>> GetUnprocessedEventsAsync(
			int limit, CancellationToken cancellationToken = default)
		{
			var wrappers = await dbContext.Set<DomainEventWrapper>()
				.AsNoTracking()
				.Where(w => !w.SuccessfullyProcessed)
				.Take(limit)
				.ToListAsync(cancellationToken);

			return wrappers
				.Select(w => w.GetDomainEvent())
				.ToList()
				.AsReadOnly();
		}

		///<inheritdoc/>
		public void MarkAsProcessed(DomainEvent domainEvent)
		{
			var wrapper = DomainEventWrapper.Wrap(domainEvent);
			dbContext.Attach(wrapper);
			wrapper.SuccessfullyProcessed = true;
		}
	}
}