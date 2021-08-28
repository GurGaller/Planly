namespace Planly.DomainModel.Tasks
{
	/// <summary>
	/// An event that states the <see cref="Deadline"/> for a <see cref="Task"/> was changed.
	/// </summary>
	public record TaskDeadlineMoved(Identifier<Task> TaskId, Deadline NewDeadline) : DomainEvent;
}