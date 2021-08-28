using System.Threading;
using Planly.DomainModel.ExternalCalendars;

namespace Planly.Application.ExternalCalendars
{
	/// <summary>
	/// Creates <see cref="ICalendarSynchronizer"/> instances for communicating
	/// with different external calendar providers.
	/// </summary>
	public interface ICalendarSynchronizerFactory
	{
		/// <summary>
		/// Gets a synchronizer for an <see cref="ExternalCalendar"/>.
		/// </summary>
		/// <param name="externalCalendar">The external calendar.</param>
		/// <param name="cancellationToken">A token for canceling the operation.</param>
		/// <returns>A synchronizer for communicating with the <paramref name="externalCalendar"/>.</returns>
		System.Threading.Tasks.Task<ICalendarSynchronizer> GetSynchronizerForAsync(
			ExternalCalendar externalCalendar, CancellationToken cancellationToken = default);
	}
}