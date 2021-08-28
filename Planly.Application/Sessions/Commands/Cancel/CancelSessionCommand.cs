using System;

namespace Planly.Application.Sessions.Commands.Cancel
{
	/// <summary>
	/// A command for deleting existing sessions.
	/// </summary>
	/// <remarks>
	/// This command is idempotent.
	/// </remarks>
	public record CancelSessionCommand(Guid SessionId) : ICommand;
}