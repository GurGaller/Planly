namespace Planly.DomainModel.Sessions
{
	/// <summary>
	/// An event that states that the description of a <see cref="Session"/> was edited.
	/// </summary>
	public record SessionDescriptionEdited(
		Identifier<Session> SessionId,
		SessionDescription NewDescription) : DomainEvent;
}