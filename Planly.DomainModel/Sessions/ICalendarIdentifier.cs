namespace Planly.DomainModel.Sessions
{
	/// <summary>
	/// A global identifier for a calendar item, as per RFC 5545 (iCalendar).
	/// See https://datatracker.ietf.org/doc/html/rfc5545#section-3.8.4.7.
	/// </summary>
	public record ICalendarIdentifier(string Id);
}