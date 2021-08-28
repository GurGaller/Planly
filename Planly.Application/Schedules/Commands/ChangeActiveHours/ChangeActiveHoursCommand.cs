using System;

namespace Planly.Application.Schedules.Commands.ChangeActiveHours
{
	/// <summary>
	/// A command that changes the active hours range of a schedule.
	/// The times are relative to midnight at UTC.
	/// </summary>
	/// <remarks>
	/// This command is idempotent.
	/// </remarks>
	public record ChangeActiveHoursCommand(Guid ScheduleId, TimeSpan Start, TimeSpan End) : ICommand;
}