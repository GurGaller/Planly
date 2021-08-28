using Planly.DomainModel.Schedules;
using Planly.DomainModel.Time;

namespace Planly.DomainModel.Tasks
{
	/// <summary>
	/// An event that states a <see cref="Task"/> was captured.
	/// </summary>
	public record TaskCaptured(
		Identifier<Task> TaskId,
		Identifier<Schedule> ScheduleId,
		TaskDescription Description,
		Deadline Deadline,
		Duration IdealSessionDuration,
		TaskProgress Progress)
		: DomainEvent;
}