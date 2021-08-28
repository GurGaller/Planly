using System;

namespace Planly.Application.Tasks.Commands.Abandon
{
	/// <summary>
	/// A command that abandons a task.
	/// </summary>
	/// <remarks>
	/// This command is idempotent.
	/// </remarks>
	public record AbandonTaskCommand(Guid TaskId) : ICommand;
}