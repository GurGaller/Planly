using Planly.DomainModel.Schedules;

namespace Planly.DomainModel.ExternalCalendars
{
	/// <summary>
	/// A calendar managed by an external provider.
	/// </summary>
	/// <seealso cref="AggregateRoot{ExternalCalendar}" />
	public class ExternalCalendar : AggregateRoot<ExternalCalendar>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExternalCalendar"/> class.
		/// </summary>
		/// <param name="id">A unique identifier of this calendar.</param>
		/// <param name="schedule">The <see cref="Schedule"/> associated with this calendar.</param>
		/// <param name="provider">The external service that manages this calendar.</param>
		internal ExternalCalendar(
			Identifier<ExternalCalendar> id,
			Schedule schedule,
			CalendarProvider provider) : base(id)
		{
			ScheduleId = schedule.Id;
			Provider = provider;
			Connected = true;
			RegisterEvent(new ExternalCalendarConnected(Id, ScheduleId, Provider));
		}

		private ExternalCalendar(Identifier<ExternalCalendar> id) : base(id)
		{
			// For reconstruction
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="ExternalCalendar"/>
		/// is connected to its local <see cref="Schedule"/>.
		/// </summary>
		public bool Connected { get; private set; }

		/// <summary>
		/// Gets the external service that manages this calendar.
		/// </summary>
		public CalendarProvider Provider { get; }

		/// <summary>
		/// Gets the ID of the <see cref="Schedule"/> associated with this calendar.
		/// </summary>
		public Identifier<Schedule> ScheduleId { get; }

		/// <summary>
		/// Connects this <see cref="ExternalCalendar"/> to its local <see cref="Schedule"/>.
		/// </summary>
		public void Connect()
		{
			if (!Connected)
			{
				Connected = true;
				RegisterEvent(new ExternalCalendarConnected(Id, ScheduleId, Provider));
			}
		}

		/// <summary>
		/// Disconnects this <see cref="ExternalCalendar"/> from its local <see cref="Schedule"/>.
		/// </summary>
		public void Disconnect()
		{
			Connected = false;
		}
	}
}