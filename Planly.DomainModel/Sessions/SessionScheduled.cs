using Planly.DomainModel.Schedules;
using Planly.DomainModel.Tasks;
using Planly.DomainModel.Time;

namespace Planly.DomainModel.Sessions
{
	/// <summary>
	/// An event that states that a new <see cref="Session"/> was scheduled.
	/// </summary>
	public record SessionScheduled(
		Identifier<Session> SessionId,
		Identifier<Schedule> ScheduleId,
		SessionDescription Description,
		TimeSlot Time,
		Identifier<Task>? TaskId,
		ICalendarIdentifier ICalendarId) : DomainEvent;
}