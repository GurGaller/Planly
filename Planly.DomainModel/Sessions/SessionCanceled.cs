namespace Planly.DomainModel.Sessions
{
	/// <summary>
	/// An event that states a session was canceled.
	/// </summary>
	public record SessionCanceled(Identifier<Session> SessionId)
		: DomainEvent;
}