using System;
using System.Diagnostics.CodeAnalysis;
using Planly.DomainModel.Schedules;
using Planly.DomainModel.Tasks;
using Planly.DomainModel.Time;

namespace Planly.DomainModel.Sessions
{
	/// <summary>
	/// An item in a <see cref="Schedule"/>.
	/// </summary>
	/// <seealso cref="AggregateRoot{Session}" />
	public class Session : AggregateRoot<Session>
	{
		internal Session(
			Identifier<Session> id,
			Identifier<Schedule> scheduleId,
			SessionDescription description,
			TimeSlot time,
			Identifier<Task>? taskId = null,
			ICalendarIdentifier? iCalendarId = null) : base(id)
		{
			ScheduleId = scheduleId;
			Description = description;
			Time = time;
			TaskId = taskId;
			ICalendarId = iCalendarId ?? new ICalendarIdentifier(Id.ToGuid().ToString());
			RegisterEvent(new SessionScheduled(id, scheduleId, description, time, taskId, iCalendarId));
		}

		private Session(Identifier<Session> id) : base(id)
		{
			// Just for reconstruction
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Session"/> belongs to a <see cref="Task"/>.
		/// </summary>
		[MemberNotNullWhen(true, nameof(TaskId))]
		public bool BelongsToTask => TaskId is not null;

		/// <summary>
		/// Gets a value indicating whether this <see cref="Session"/> is canceled.
		/// </summary>
		public bool Canceled { get; private set; }

		/// <summary>
		/// Gets a description of this <see cref="Session"/>.
		/// </summary>
		public SessionDescription Description { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="Session"/> is completed or not.
		/// </summary>
		public bool Done { get; private set; }

		/// <summary>
		/// Gets the global iCalendar ID.
		/// </summary>
		public ICalendarIdentifier ICalendarId { get; }

		/// <summary>
		/// Gets the unique identifier of the <see cref="Schedule"/> to which this <see cref="Session"/> belongs.
		/// </summary>
		public Identifier<Schedule> ScheduleId { get; }

		/// <summary>
		/// Gets the unique identifier of the <see cref="Task"/> associated with this <see cref="Session"/>,
		/// or <see langword="null"/> if this <see cref="Session"/> is not associated with any <see cref="Task"/>.
		/// </summary>
		public Identifier<Task>? TaskId { get; }

		/// <summary>
		/// Gets the time frame in which this <see cref="Session"/> is scheduled.
		/// </summary>
		public TimeSlot Time { get; private set; }

		/// <summary>
		/// Cancels this <see cref="Session"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// When trying to cancel a done <see cref="Session"/>, or a session from the past.
		/// </exception>
		public void Cancel()
		{
			if (Done)
				throw new InvalidOperationException("Cannot cancel a session that is already done.");

			if (Canceled)
				return;

			if (Time.EndTime < DateTimeOffset.UtcNow)
				throw new InvalidOperationException("Cannot cancel a session from the past.");

			Canceled = true;
			RegisterEvent(new SessionCanceled(Id));
		}

		/// <summary>
		/// Edits the description.
		/// </summary>
		/// <param name="description">The new description.</param>
		public void EditDescription(SessionDescription description)
		{
			if (description != Description)
			{
				Description = description;
				RegisterEvent(new SessionDescriptionEdited(Id, description));
			}
		}

		/// <summary>
		/// Marks this <see cref="Session"/> as completed.
		/// </summary>
		/// <exception cref="InvalidOperationException">When the <see cref="Session"/> is canceled.</exception>
		public void MarkAsDone()
		{
			if (Canceled)
				throw new InvalidOperationException("Cannot mark a canceled session as done.");

			if (!Done)
			{
				Done = true;
				RegisterEvent(new SessionMarkedAsDone(Id));
			}
		}

		/// <summary>
		/// Reschedules the session to a different time frame.
		/// </summary>
		/// <param name="time">The new time frame.</param>
		/// <exception cref="InvalidOperationException">When the <see cref="Session"/> is canceled or done.</exception>
		public void Reschedule(TimeSlot time)
		{
			if (Canceled || Done)
				throw new InvalidOperationException("Cannot reschedule a canceled or done session.");

			if (time != Time)
			{
				Time = time;
				RegisterEvent(new SessionRescheduled(Id, time));
			}
		}
	}
}