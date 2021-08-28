using System;
using System.Collections.Generic;
using Planly.DomainModel.Time;

namespace Planly.DomainModel.Tasks
{
	/// <summary>
	/// An <see cref="ISchedulingStrategy"/> that schedules sessions with linearly reduced margins.
	/// </summary>
	public class LinearSchedulingStrategy : ISchedulingStrategy
	{
		/// <inheritdoc/>
		public IEnumerable<TimeSlot> GetTimeSlots(Deadline deadline, Duration slotLength, int slotCount)
		{
			var startTime = DateTimeOffset.UtcNow;
			var slope = GetLinearSlope(deadline, slotCount, slotLength.ToTimeSpan());
			for (var i = slotCount; i >= 1; i--)
			{
				startTime += i * slope;
				yield return new TimeSlot(startTime, slotLength);
			}
		}

		private static TimeSpan GetLinearSlope(Deadline deadline, int sessionCount, TimeSpan sessionLength)
		{
			var sessionsTime = sessionCount * sessionLength;
			var timeTillDeadline = GetTimeTillDeadline(deadline, DateTimeOffset.UtcNow);
			var totalMargin = timeTillDeadline - sessionsTime;
			if (totalMargin < TimeSpan.Zero)
				throw new InvalidOperationException("Not enough time!");

			return 2 * totalMargin / (Math.Pow(sessionCount, 2) + (3 * sessionCount) + 2);
		}

		private static TimeSpan GetTimeTillDeadline(Deadline deadline, DateTimeOffset now)
		{
			var timeTillDeadline = deadline.Time - now;
			if (timeTillDeadline > TimeSpan.FromDays(2))
				timeTillDeadline -= deadline.Time.TimeOfDay;
			return timeTillDeadline;
		}
	}
}