using Planly.DomainModel.Schedules;

namespace Planly.DomainModel.ExternalCalendars
{
	/// <summary>
	/// An event that indicates that an external calendar was connected to a schedule.
	/// </summary>
	public record ExternalCalendarConnected(
		Identifier<ExternalCalendar> CalendarId,
		Identifier<Schedule> ScheduleId,
		CalendarProvider Provider) : DomainEvent;
}