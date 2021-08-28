using System;

namespace Planly.Application.Sessions.Commands.EditDetails
{
	/// <summary>
	/// A command that edits the details of a session.
	/// </summary>
	/// <remarks>
	/// This command is idempotent.
	/// </remarks>
	public record EditSessionDetailsCommand(
		Guid Id,
		string Title,
		DateTimeOffset StartTime,
		DateTimeOffset EndTime) : ICommand;
}