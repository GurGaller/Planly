using System;

namespace Planly.Application.Tasks.Commands.EditDetails
{
	/// <summary>
	/// A command that updates the details of a task.
	/// </summary>
	/// <remarks>
	/// This command is idempotent.
	/// </remarks>
	public record EditTaskDetailsCommand(
		Guid Id,
		string Title,
		TimeSpan TotalTimeRequired,
		TimeSpan IdealSessionDuration,
		DateTimeOffset Deadline) : ICommand;
}