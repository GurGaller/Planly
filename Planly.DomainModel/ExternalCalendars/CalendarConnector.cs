using System.Threading;
using Planly.DomainModel.Schedules;

namespace Planly.DomainModel.ExternalCalendars
{
	/// <summary>
	/// Connects existing external calendars to a local <see cref="Schedule"/>.
	/// </summary>
	public class CalendarConnector
	{
		private readonly IExternalCalendarRepository externalCalendarRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="CalendarConnector"/> class.
		/// </summary>
		/// <param name="externalCalendarRepository">An external calendar repository.</param>
		public CalendarConnector(IExternalCalendarRepository externalCalendarRepository)
		{
			this.externalCalendarRepository = externalCalendarRepository;
		}

		/// <summary>
		/// Idempotently connects an external calendar to a <see cref="Schedule"/>.
		/// </summary>
		/// <param name="calendarProvider">The calendar provider.</param>
		/// <param name="schedule">The schedule.</param>
		/// <param name="cancellationToken">A token for canceling the operation.</param>
		/// <returns>The generated <see cref="ExternalCalendar"/> instance.</returns>
		public async System.Threading.Tasks.Task<ExternalCalendar> ConnectAsync(
			CalendarProvider calendarProvider,
			Schedule schedule,
			CancellationToken cancellationToken = default)
		{
			var externalCalendar = await externalCalendarRepository.FindAsync(
				schedule.Id,
				calendarProvider,
				cancellationToken);

			if (externalCalendar is null)
				externalCalendar = CreateNewCalendar(calendarProvider, schedule);
			else
				externalCalendar.Connect();

			return externalCalendar;
		}

		private ExternalCalendar CreateNewCalendar(CalendarProvider calendarProvider, Schedule schedule)
		{
			var externalCalendarId = Identifier<ExternalCalendar>.GenerateNew();
			var externalCalendar = new ExternalCalendar(externalCalendarId, schedule, calendarProvider);
			externalCalendarRepository.Add(externalCalendar);
			return externalCalendar;
		}
	}
}