using Planly.DomainModel.Time;

namespace Planly.DomainModel.Tasks
{
	/// <summary>
	/// An event that states that the time requirements for a <see cref="Task"/> has changed.
	/// </summary>
	public record TaskTimeRequirementChanged(
		Identifier<Task> TaskId,
		Duration NewTimeRequirement) : DomainEvent;
}