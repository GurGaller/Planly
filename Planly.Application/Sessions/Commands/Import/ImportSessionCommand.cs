using System;

namespace Planly.Application.Sessions.Commands.Import
{
	/// <summary>
	/// Imports an existing session to the current user's schedule.
	/// </summary>
	public record ImportSessionCommand(
		string Title,
		DateTimeOffset StartTime,
		DateTimeOffset EndTime,
		string ICalendarId) : ICommand;
}