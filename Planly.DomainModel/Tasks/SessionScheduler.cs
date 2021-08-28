using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Planly.DomainModel.Schedules;
using Planly.DomainModel.Sessions;
using Planly.DomainModel.Time;
using Planly.DomainModel.Utility;

namespace Planly.DomainModel.Tasks
{
	/// <summary>
	/// Schedules sessions for fulfilling a task.
	/// </summary>
	public class SessionScheduler
	{
		private readonly IScheduleRepository scheduleRepository;
		private readonly ISchedulingStrategy schedulingStrategy;
		private readonly ISessionRepository sessionRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="SessionScheduler"/> class.
		/// </summary>
		/// <param name="sessionRepository">A session repository.</param>
		/// <param name="scheduleRepository">A schedule repository.</param>
		/// <param name="schedulingStrategy"></param>
		public SessionScheduler(
			ISessionRepository sessionRepository,
			IScheduleRepository scheduleRepository,
			ISchedulingStrategy schedulingStrategy)
		{
			this.sessionRepository = sessionRepository;
			this.scheduleRepository = scheduleRepository;
			this.schedulingStrategy = schedulingStrategy;
		}

		/// <summary>
		/// Schedules sessions according to the work time left of a <paramref name="task"/>.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <param name="cancellationToken">A token for canceling the operation.</param>
		public async System.Threading.Tasks.Task ScheduleSessionsAsync(Task task, CancellationToken cancellationToken)
		{
			var schedule = await GetScheduleAsync(task, cancellationToken);

			var sessionCount = CalculateSessionCount(task);
			if (sessionCount is 0)
				return;

			var sessionLength = task.Progress.TimeLeft / sessionCount;

			var emptySlots = await GetEmptyTimeSlotsUntilAsync(
				task.Deadline,
				schedule,
				sessionLength,
				cancellationToken);

			ScheduleInEmptyTimeSlots(task, schedule, sessionCount, sessionLength, emptySlots);
		}

		private static int CalculateSessionCount(Task task)
		{
			var timeLeftToSessionTimeRatio = task.Progress.TimeLeft / task.IdealSessionDuration;
			return (int)Math.Round(timeLeftToSessionTimeRatio);
		}

		private async System.Threading.Tasks.Task<IEnumerable<TimeSlot>> GetEmptyTimeSlotsUntilAsync(
			Deadline deadline,
			Schedule schedule,
			Duration slotSize,
			CancellationToken cancellationToken)
		{
			var takenSlots = await GetUnavailableTimeSlotsUntilAsync(deadline, schedule, cancellationToken);

			return takenSlots
				.Zip(takenSlots.Skip(1), (first, second) => TimeSlot.Between(first, second))
				.Where(slot => slot is not null)
				.Where(slot => slot!.EndTime > DateTimeOffset.UtcNow)
				.Select(slot =>
				{
					if (slot!.StartTime > DateTimeOffset.UtcNow)
						return slot;

					return TimeSlot.Between(DateTimeOffset.UtcNow, slot.EndTime);
				})
				.Where(slot => slot.Duration.IsLongerOrEqualTo(slotSize))
				.SelectMany(slot =>
				{
					var count = (int)(slot.Duration.ToTimeSpan() / slotSize.ToTimeSpan());
					return slot.Split(count);
				});
		}

		private async System.Threading.Tasks.Task<Schedule> GetScheduleAsync(
			Task task, CancellationToken cancellationToken)
		{
			var schedule = await scheduleRepository.FindByIdAsync(task.ScheduleId, cancellationToken);

			if (schedule is null)
				throw new InvalidOperationException("A schedule with the specified ID does not exist.");

			return schedule;
		}

		private async System.Threading.Tasks.Task<List<TimeSlot>> GetUnavailableTimeSlotsUntilAsync(
			Deadline deadline, Schedule schedule, CancellationToken cancellationToken)
		{
			var today = DateTimeOffset.UtcNow.Date;
			var daysTillDeadline = (int)(deadline.Time.Date - today).TotalDays;
			var nights = Enumerable.Range(0, count: daysTillDeadline + 1)
				.Select(i =>
				{
					var previousDayEnd = schedule.ActiveHours.End.Of(today.AddDays(i - 1));
					var currentDayStart = schedule.ActiveHours.Start.Of(today.AddDays(i));
					return TimeSlot.Between(previousDayEnd, currentDayStart);
				});

			var sessions = await sessionRepository.GetByScheduleIdAsync(
				schedule.Id,
				offset: 0,
				limit: int.MaxValue,
				firstDate: DateTimeOffset.UtcNow,
				lastDate: deadline.Time,
				cancellationToken);

			var takenSlots = new List<TimeSlot>(capacity: sessions.Count + daysTillDeadline + 1);
			takenSlots.AddRange(sessions.Select(s => s.Time).Merge(nights));
			return takenSlots;
		}

		private void ScheduleIn(TimeSlot availableSlot, Task task, Schedule schedule, Duration sessionLength)
		{
			var sessionTime = new TimeSlot(availableSlot.StartTime, sessionLength);
			var sessionDescription = task.Description.DescribeSession();
			var sessionId = Identifier<Session>.GenerateNew();
			var session = new Session(sessionId, schedule.Id, sessionDescription, sessionTime, task.Id);

			sessionRepository.Add(session);
		}

		private void ScheduleInEmptyTimeSlots(
			Task task,
			Schedule schedule,
			int sessionCount,
			Duration sessionLength,
			IEnumerable<TimeSlot> emptySlots)
		{
			var idealTimeSlots = schedulingStrategy.GetTimeSlots(task.Deadline, sessionLength, sessionCount);
			var idealSlotsEnumerator = idealTimeSlots.GetEnumerator();
			if (!idealSlotsEnumerator.MoveNext())
				return;
			var closestSlot = TimeSlot.Between(DateTimeOffset.MaxValue, DateTimeOffset.MaxValue);
			foreach (var emptySlot in emptySlots)
			{
				var closestDiff = closestSlot.DistanceFrom(idealSlotsEnumerator.Current);
				var currentDiff = emptySlot.DistanceFrom(idealSlotsEnumerator.Current);

				if (currentDiff.IsLongerThan(closestDiff))
				{
					ScheduleIn(closestSlot, task, schedule, sessionLength);
					if (!idealSlotsEnumerator.MoveNext())
						return;
				}

				closestSlot = emptySlot;
			}
		}
	}
}