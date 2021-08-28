namespace Planly.DomainModel.Tasks
{
	/// <summary>
	/// An event that states that a description of a <see cref="Task"/> was changed.
	/// </summary>
	public record TaskDescriptionEdited(Identifier<Task> TaskId, TaskDescription NewDescription) : DomainEvent;
}