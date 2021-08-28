using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planly.Application.ExternalCalendars
{
	/// <summary>
	/// Manages keys for interacting with external calendar providers.
	/// </summary>
	public interface IExternalCalendarKeyStore
	{
		/// <summary>
		/// Finds a key by its name, for a specific external calendar.
		/// </summary>
		/// <param name="calendarId">The external calendar's ID.</param>
		/// <param name="keyName">The name of the key.</param>
		/// <param name="cancellationToken">A token for canceling the operation.</param>
		/// <returns>The key, or <see langword="null"/> if it wasn't found.</returns>
		Task<ExternalCalendarKey?> FindAsync(
			Guid calendarId,
			string keyName,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Removes the specified key from the store.
		/// </summary>
		/// <param name="key">The key.</param>
		void Remove(ExternalCalendarKey key);

		/// <summary>
		/// Stores the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		void Store(ExternalCalendarKey key);
	}
}