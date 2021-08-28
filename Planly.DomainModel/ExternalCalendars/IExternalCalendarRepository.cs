using System.Collections.Generic;
using System.Threading;
using Planly.DomainModel.Schedules;

namespace Planly.DomainModel.ExternalCalendars
{
	/// <summary>
	/// A collection of <see cref="ExternalCalendar"/>s.
	/// </summary>
	/// <seealso cref="IRepository{ExternalCalendar}" />
	public interface IExternalCalendarRepository : IRepository<ExternalCalendar>
	{
		/// <summary>
		/// Finds the <see cref="ExternalCalendar"/> of a specified provider for a particular <see cref="Schedule"/>.
		/// </summary>
		/// <param name="scheduleId">The ID of the schedule.</param>
		/// <param name="calendarProvider">The external calendar's provider.</param>
		/// <param name="cancellationToken">A token for canceling the operation.</param>
		/// <returns>The matching calendar, or <see langword="null"/> if none were found.</returns>
		System.Threading.Tasks.Task<ExternalCalendar?> FindAsync(
			Identifier<Schedule> scheduleId,
			CalendarProvider calendarProvider,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets the connected <see cref="ExternalCalendar"/>s of a <see cref="Schedule"/>.
		/// </summary>
		/// <param name="scheduleId">The ID of the schedule.</param>
		/// <param name="cancellationToken">A token for canceling the operation.</param>
		/// <returns>The matching calendars, or an empty collection if none were found.</returns>
		System.Threading.Tasks.Task<IReadOnlyCollection<ExternalCalendar>> GetByScheduleIdAsync(
			Identifier<Schedule> scheduleId, CancellationToken cancellationToken = default);
	}
}