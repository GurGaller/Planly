using System;

namespace Planly.Application.Schedules.Commands.Create
{
	/// <summary>
	/// Creates a schedule for a specific user.
	/// </summary>
	public record CreateScheduleCommand(Guid UserId) : ICommand;
}