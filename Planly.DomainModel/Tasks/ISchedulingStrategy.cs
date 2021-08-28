using System.Collections.Generic;
using Planly.DomainModel.Time;

namespace Planly.DomainModel.Tasks
{
	/// <summary>
	/// A way of finding time slots for sessions of a task.
	/// </summary>
	public interface ISchedulingStrategy
	{
		/// <summary>
		/// Gets <paramref name="slotCount"/> time slots of a specified length, until a <paramref name="deadline"/>,
		/// that are ideal for scheduling sessions for a task, according to this strategy.
		/// The slots are ordered by their start time, and then by their end time.
		/// </summary>
		/// <param name="deadline">The deadline.</param>
		/// <param name="slotLength">The length of the slots.</param>
		/// <param name="slotCount">The number of slots.</param>
		/// <returns>The requested slots, timed according to this scheduling strategy.</returns>
		IEnumerable<TimeSlot> GetTimeSlots(Deadline deadline, Duration slotLength, int slotCount);
	}
}