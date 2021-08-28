using Planly.DomainModel.Time;

namespace Planly.DomainModel.Tasks
{
	/// <summary>
	/// An event that states that the ideal duration of sessions for working on a <see cref="Task"/> has changed.
	/// </summary>
	public record TaskSessionDurationChanged(
		Identifier<Task> TaskId,
		Duration NewDuration) : DomainEvent;
}