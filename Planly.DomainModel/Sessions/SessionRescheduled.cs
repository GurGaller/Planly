using Planly.DomainModel.Time;

namespace Planly.DomainModel.Sessions
{
	/// <summary>
	/// An event that states a <see cref="Session"/> was rescheduled, meaning
	/// its time was changed or it was re-activated after cancellation.
	/// </summary>
	public record SessionRescheduled(
		Identifier<Session> SessionId,
		TimeSlot NewTime) : DomainEvent;
}