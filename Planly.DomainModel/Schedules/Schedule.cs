using Planly.DomainModel.Sessions;
using Planly.DomainModel.Tasks;
using Planly.DomainModel.Time;

namespace Planly.DomainModel.Schedules
{
	/// <summary>
	/// A collection of <see cref="Session"/>s on a time line.
	/// </summary>
	public class Schedule : AggregateRoot<Schedule>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Schedule"/> class with an active hours range of all day.
		/// </summary>
		/// <param name="id">The unique identifier of the <see cref="Schedule"/>.</param>
		public Schedule(Identifier<Schedule> id) : base(id)
		{
			ActiveHours = HourRange.AllDay;
		}

		/// <summary>
		/// Gets the range of hours in which sessions can be scheduled.
		/// </summary>
		public HourRange ActiveHours { get; private set; }

		/// <summary>
		/// Captures a new <see cref="Task"/>.
		/// </summary>
		/// <param name="description">The description of the new <see cref="Task"/>.</param>
		/// <param name="deadline">
		/// The <see cref="Deadline"/> until which the <see cref="Task"/> must be completed.
		/// </param>
		/// <param name="idealSessionDuration">
		/// The ideal <see cref="Duration"/> of <see cref="Session"/>s for working on the <see cref="Task"/>
		/// </param>
		/// <param name="totalTimeRequired">
		/// The total work time required for the completion of the <see cref="Task"/>.
		/// </param>
		/// <returns>The newly created <see cref="Task"/></returns>
		public Task CaptureTask(
			TaskDescription description,
			Deadline deadline,
			Duration idealSessionDuration,
			Duration totalTimeRequired)
		{
			var taskId = Identifier<Task>.GenerateNew();
			var progress = new TaskProgress(totalTimeRequired, timeCompleted: Duration.Zero);
			return new Task(taskId, Id, description, deadline, idealSessionDuration, progress);
		}

		/// <summary>
		/// Changes the active hours range.
		/// </summary>
		/// <param name="activeHours">The new hour range.</param>
		public void ChangeActiveHours(HourRange activeHours)
		{
			ActiveHours = activeHours;
		}

		/// <summary>
		/// Imports an existing session.
		/// </summary>
		/// <param name="iCalendarId">The calendar's global ID.</param>
		/// <param name="description">The description of the <see cref="Session"/>.</param>
		/// <param name="time">The time of the <see cref="Session"/>.</param>
		/// <returns>The imported <see cref="Session"/>.</returns>
		public Session ImportSession(ICalendarIdentifier iCalendarId, SessionDescription description, TimeSlot time)
		{
			var sessionId = Identifier<Session>.GenerateNew();
			return new Session(sessionId, Id, description, time, iCalendarId: iCalendarId);
		}

		/// <summary>
		/// Schedules a new session.
		/// </summary>
		/// <param name="description">The description of the <see cref="Session"/>.</param>
		/// <param name="time">The time of the <see cref="Session"/>.</param>
		/// <returns>The newly created <see cref="Session"/></returns>
		public Session ScheduleSession(SessionDescription description, TimeSlot time)
		{
			var sessionId = Identifier<Session>.GenerateNew();
			return new Session(sessionId, Id, description, time);
		}
	}
}