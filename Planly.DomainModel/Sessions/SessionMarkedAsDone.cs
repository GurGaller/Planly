namespace Planly.DomainModel.Sessions
{
	/// <summary>
	/// An event that states that a <see cref="Session"/> was marked as done.
	/// </summary>
	public record SessionMarkedAsDone(Identifier<Session> SessionId) : DomainEvent;
}