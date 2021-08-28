namespace Planly.Application.ExternalCalendars.Commands.Disconnect
{
	/// <summary>
	/// A command that disconnects an external calendar from the current user's schedule.
	/// </summary>
	/// <remarks>
	/// This command is idempotent.
	/// </remarks>
	public record DisconnectExternalCalendarCommand(string Provider) : ICommand;
}