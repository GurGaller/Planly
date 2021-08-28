using System;

namespace Planly.Application.Sessions.Commands.Schedule
{
	/// <summary>
	/// A command that schedules new sessions in the current user's schedule.
	/// Returns the ID of the newly scheduled session.
	/// </summary>
	public record ScheduleSessionCommand(
		string Title,
		DateTimeOffset StartTime,
		DateTimeOffset EndTime) : ICommand<Guid>;
}