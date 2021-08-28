using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Planly.DomainModel;

namespace Planly.Application.DomainEvents
{
	/// <summary>
	/// Accesses an external event store.
	/// </summary>
	public interface IDomainEventStore
	{
		/// <summary>
		/// Gets <paramref name="limit"/> unprocessed events using unspecified order.
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="cancellationToken">A token for canceling the operation.</param>
		/// <returns>The list of unprocessed events.</returns>
		Task<IReadOnlyList<DomainEvent>> GetUnprocessedEventsAsync(int limit, CancellationToken cancellationToken = default);

		/// <summary>
		/// Marks an event as processed.
		/// </summary>
		/// <param name="domainEvent">The domain event.</param>
		/// <remarks>
		/// The event instance doesn't have to come from this store.
		/// </remarks>
		void MarkAsProcessed(DomainEvent domainEvent);
	}
}