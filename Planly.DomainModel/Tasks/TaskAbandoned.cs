namespace Planly.DomainModel.Tasks
{
	/// <summary>
	/// An event that states a <see cref="Task"/> was abandoned.
	/// </summary>
	public record TaskAbandoned(Identifier<Task> TaskId) : DomainEvent;
}