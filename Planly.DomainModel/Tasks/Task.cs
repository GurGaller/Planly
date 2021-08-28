using System;
using Planly.DomainModel.Schedules;
using Planly.DomainModel.Time;

namespace Planly.DomainModel.Tasks
{
	/// <summary>
	/// A goal acheived by a group of <see cref="Sessions.Session"/>.
	/// </summary>
	public class Task : AggregateRoot<Task>
	{
		internal Task(
			Identifier<Task> id,
			Identifier<Schedule> scheduleId,
			TaskDescription description,
			Deadline deadline,
			Duration idealSessionDuration,
			TaskProgress progress) : this(id)
		{
			ScheduleId = scheduleId;
			Description = description;
			Deadline = deadline;
			IdealSessionDuration = idealSessionDuration;
			Progress = progress;

			RegisterEvent(new TaskCaptured(id, scheduleId, description, deadline, idealSessionDuration, progress));
		}

		private Task(Identifier<Task> id) : base(id)
		{
			// Just for reconstruction.
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Task"/> was abandoned.
		/// </summary>
		public bool Abandoned { get; private set; }

		/// <summary>
		/// Gets the <see cref="Tasks.Deadline"/> until which this <see cref="Task"/> should be completed.
		/// </summary>
		public Deadline Deadline { get; private set; }

		/// <summary>
		/// Gets a description of this <see cref="Task"/>.
		/// </summary>
		public TaskDescription Description { get; private set; }

		/// <summary>
		/// Gets the ideal <see cref="Duration"/> of <see cref="Sessions.Session"/>s
		/// for working on this <see cref="Task"/>
		/// </summary>
		public Duration IdealSessionDuration { get; private set; }

		/// <summary>
		/// Gets the work progress of this <see cref="Task"/>.
		/// </summary>
		public TaskProgress Progress { get; private set; }

		/// <summary>
		/// Gets the unique identifier of the <see cref="Schedule"/> to which this <see cref="Task"/> belongs.
		/// </summary>
		public Identifier<Schedule> ScheduleId { get; }

		/// <summary>
		/// Abandons this <see cref="Task"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException">When the <see cref="Task"/> is already completed.</exception>
		public void Abandon()
		{
			if (Progress.Completed)
				throw new InvalidOperationException("Cannot abandon a task that was already completed.");

			if (!Abandoned)
			{
				Abandoned = true;
				RegisterEvent(new TaskAbandoned(Id));
			}
		}

		/// <summary>
		/// Changes the duration of the sessions that are scheduled for working on this <see cref="Task"/>.
		/// </summary>
		/// <param name="idealSessionDuration">
		/// The ideal <see cref="Duration"/> of sessions for working on this <see cref="Task"/>.
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// When the <see cref="Task"/> is abandoned or completed.
		/// </exception>
		public void ChangeSessionDuration(Duration idealSessionDuration)
		{
			if (Abandoned || Progress.Completed)
				throw new InvalidOperationException("Only tasks that were not completed or abandoned can change their session duration.");

			if (idealSessionDuration != IdealSessionDuration)
			{
				IdealSessionDuration = idealSessionDuration;
				RegisterEvent(new TaskSessionDurationChanged(Id, idealSessionDuration));
			}
		}

		/// <summary>
		/// Edits the description.
		/// </summary>
		/// <param name="description">The new description.</param>
		public void EditDescription(TaskDescription description)
		{
			if (description != Description)
			{
				Description = description;
				RegisterEvent(new TaskDescriptionEdited(Id, description));
			}
		}

		/// <summary>
		/// Moves the <see cref="Tasks.Deadline"/>.
		/// </summary>
		/// <param name="deadline">The new <see cref="Tasks.Deadline"/>.</param>
		/// <exception cref="InvalidOperationException">
		/// When the <see cref="Task"/> is abandoned or completed.
		/// </exception>
		public void MoveDeadline(Deadline deadline)
		{
			if (Abandoned || Progress.Completed)
				throw new InvalidOperationException("Only tasks that were not completed or abandoned can change their deadline.");

			if (deadline != Deadline)
			{
				Deadline = deadline;
				RegisterEvent(new TaskDeadlineMoved(Id, deadline));
			}
		}

		/// <summary>
		/// Changes the time requirement of this <see cref="Task"/>.
		/// </summary>
		/// <param name="timeRequired">The new time requirement.</param>
		/// <exception cref="InvalidOperationException">When the <see cref="Task"/> is abandoned.</exception>
		public void RequireTime(Duration timeRequired)
		{
			if (Abandoned)
				throw new InvalidOperationException("Cannot change the time requirements for an abandoned task.");

			if (timeRequired != Progress.TimeRequired)
			{
				Progress = Progress.WithTimeRequirement(timeRequired);
				RegisterEvent(new TaskTimeRequirementChanged(Id, timeRequired));
			}
		}

		internal void TrackProgress(Duration additionalTimeSpent)
		{
			Progress = Progress.AddTime(additionalTimeSpent);
		}
	}
}