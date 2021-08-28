using System;

namespace Planly.Application.ExternalCalendars
{
	/// <summary>
	/// A key for interacting with an external calendar.
	/// </summary>
	public class ExternalCalendarKey
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExternalCalendarKey"/> class.
		/// </summary>
		/// <param name="calendarId">The ID of the external calendar this key to which this key belongs.</param>
		/// <param name="name">The name of the key.</param>
		/// <param name="value">The value of the key.</param>
		public ExternalCalendarKey(Guid calendarId, string name, string value)
		{
			CalendarId = calendarId;
			Name = name;
			Value = value;
		}

		/// <summary>
		/// Gets the ID of the external calendar this key to which this key belongs.
		/// </summary>
		public Guid CalendarId { get; }

		/// <summary>
		/// Gets the name of the key.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets or sets the value of the key.
		/// </summary>
		public string Value { get; set; }
	}
}