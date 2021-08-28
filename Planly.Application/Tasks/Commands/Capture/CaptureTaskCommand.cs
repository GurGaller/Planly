using System;

namespace Planly.Application.Tasks.Commands.Capture
{
	/// <summary>
	/// A command that captures a new task into the current user's schedule.
	/// It returns the ID of the new task.
	/// </summary>
	public record CaptureTaskCommand(
		string Title,
		TimeSpan TotalTimeRequired,
		TimeSpan IdealSessionDuration,
		DateTimeOffset Deadline) : ICommand<Guid>;
}