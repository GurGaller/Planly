using System;
using System.Collections.Generic;

namespace Planly.DomainModel.Time
{
	/// <summary>
	/// A slot of time in a Schedule.
	/// </summary>
	public record TimeSlot : IComparable<TimeSlot>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TimeSlot"/> class.
		/// </summary>
		/// <param name="startTime">The start of the slot.</param>
		/// <param name="duration">The duration of the slot.</param>
		public TimeSlot(DateTimeOffset startTime, Duration duration)
			: this(startTime, startTime + duration.ToTimeSpan())
		{
		}

		private TimeSlot(DateTimeOffset startTime, DateTimeOffset endTime)
		{
			StartTime = startTime;
			EndTime = endTime;
		}
		/// <summary>
		/// Gets the point in time in which the slot ends.
		/// </summary>
		public DateTimeOffset EndTime { get; }
		/// <summary>
		/// Gets the point in time in which the slot starts.
		/// </summary>
		public DateTimeOffset StartTime { get; }
		/// <summary>
		/// Gets the <see cref="Time.Duration"/> of the slot.
		/// </summary>
		public Duration Duration => new Duration(EndTime - StartTime);

		/// <summary>
		/// Gets a slot between two points in time.
		/// </summary>
		/// <param name="startTime">The start time.</param>
		/// <param name="endTime">The end time.</param>
		/// <returns>The new slot.</returns>
		public static TimeSlot Between(DateTimeOffset startTime, DateTimeOffset endTime)
		{
			return new(startTime, endTime);
		}

		/// <inheritdoc/>
		public int CompareTo(TimeSlot? other)
		{
			if (other is null)
				return 1;

			var startComparison = StartTime.CompareTo(other.StartTime);
			if (startComparison is not 0)
				return startComparison;

			return EndTime.CompareTo(other.EndTime);
		}

		/// <summary>
		/// The <see cref="TimeSlot"/> between two other slots.
		/// </summary>
		/// <param name="first">The first slot.</param>
		/// <param name="second">The second slot.</param>
		/// <returns>
		/// The created slot, or <see langword="null"/> if the <paramref name="first"/>
		/// slot does not end before the <paramref name="second"/> slot starts.
		/// </returns>
		public static TimeSlot? Between(TimeSlot first, TimeSlot second)
		{
			if (first.EndTime >= second.StartTime)
				return null;

			return Between(first.EndTime, second.StartTime);
		}

		internal Duration DistanceFrom(TimeSlot other)
		{
			var distanceFromStart = DistanceFrom(other.StartTime);
			var distanceFromEnd = DistanceFrom(other.EndTime);

			if (distanceFromEnd.IsLongerThan(distanceFromStart))
				return distanceFromStart;

			return distanceFromEnd;
		}

		internal IEnumerable<TimeSlot> Split(int count)
		{
			var duration = Duration / count;
			for (var i = 0; i < count; i++)
			{
				var start = StartTime + (duration.ToTimeSpan() * i);
				yield return new TimeSlot(start, duration);
			}
		}

		private Duration DistanceFrom(DateTimeOffset time)
		{
			if (time < StartTime)
				return new Duration(StartTime - time);

			if (time > EndTime)
				return new Duration(time - EndTime);

			return Duration.Zero;
		}
	}
}