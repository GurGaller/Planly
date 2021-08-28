using System;

namespace Planly.Application.ExternalCalendars.Commands.Connect
{
	/// <summary>
	/// Connects an external calendar from a given <paramref name="Provider" /> to the certain user's Schedule.
	/// </summary>
	/// <remarks>
	/// This command is idempotent.
	/// </remarks>
	public record ConnectExternalCalendarCommand(Guid UserId, string Provider) : ICommand;
}