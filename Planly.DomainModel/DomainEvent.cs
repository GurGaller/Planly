using System;

namespace Planly.DomainModel
{
	/// <summary>
	/// An event that happened to a Domain Entity in the past.
	/// </summary>
	public abstract record DomainEvent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DomainEvent"/> class
		/// that represents an event that happened just now.
		/// </summary>
		public DomainEvent()
		{
			OccurrenceTime = DateTimeOffset.UtcNow;
			EventId = Guid.NewGuid();
		}

		/// <summary>
		/// Gets or sets the point in time in which the event has occurred.
		/// </summary>
		public DateTimeOffset OccurrenceTime { get; init; }
		/// <summary>
		/// Gets or sets the unique identifier of the event.
		/// </summary>
		public Guid EventId { get; init; }
	}
}