using System;

namespace Planly.Application.Sessions.Commands.MarkAsDone
{
	/// <summary>
	/// A command that marks a session as completed.
	/// </summary>
	/// <remarks>
	/// This command is idempotent.
	/// </remarks>
	public record MarkSessionAsDoneCommand(Guid SessionId) : ICommand;
}