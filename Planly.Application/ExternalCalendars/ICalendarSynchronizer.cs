using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Planly.DomainModel;
using Planly.DomainModel.Sessions;

namespace Planly.Application.ExternalCalendars
{
	/// <summary>
	/// Used for syncing with an external calendar.
	/// </summary>
	public interface ICalendarSynchronizer
	{
		/// <summary>
		/// Creates entries in the external calendar that match a collection of local <see cref="Session"/>s.
		/// </summary>
		/// <param name="sessions">The local sessions.</param>
		/// <param name="cancellationToken">A token for canceling the operation.</param>
		/// <remarks>
		/// This method is idempotent. If an entry already exists, no changes are made to it.
		/// </remarks>
		Task CreateEntriesAsync(IEnumerable<Session> sessions, CancellationToken cancellationToken = default);

		/// <summary>
		/// Creates an entry in the external calendar that matches a local <see cref="Session"/>.
		/// </summary>
		/// <param name="session">The <see cref="Session"/>.</param>
		/// <param name="cancellationToken">A token for canceling the operation.</param>
		/// <remarks>
		/// This method is idempotent. If the entry already exists, no changes are made.
		/// </remarks>
		Task CreateEntryAsync(Session session, CancellationToken cancellationToken = default);

		/// <summary>
		/// Removes the entry associated with a <see cref="Session"/> from the external calendar.
		/// </summary>
		/// <param name="sessionId">The session's ID.</param>
		/// <param name="cancellationToken">A token for canceling the operation.</param>
		Task RemoveEntryAssociatedWithSessionAsync(
			Identifier<Session> sessionId,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Updates the entry for <paramref name="session"/> in the external calendar.
		/// If an entry is missing for the session, it is created.
		/// </summary>
		/// <param name="session">The session whose entry should be updated.</param>
		/// <param name="cancellationToken">A token for canceling the operation.</param>
		/// <remarks>
		/// This operation is idempotent.
		/// </remarks>
		Task UpdateEntryAsync(Session session, CancellationToken cancellationToken = default);
	}
}