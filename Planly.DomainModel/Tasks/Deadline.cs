using System;

namespace Planly.DomainModel.Tasks
{
	/// <summary>
	/// A time limit for a <see cref="Task"/>.
	/// </summary>
	public record Deadline
	{
		private Deadline(DateTimeOffset time)
		{
			Time = time;
		}
		/// <summary>
		/// Gets the point in time of this <see cref="Deadline"/>.
		/// </summary>
		public DateTimeOffset Time { get; }
		/// <summary>
		/// Constructs a <see cref="Deadline"/> using a specific point in time.
		/// </summary>
		/// <param name="time">The point in time.</param>
		/// <returns>The newly created <see cref="Deadline"/>.</returns>
		public static Deadline Until(DateTimeOffset time) => new(time);
		/// <summary>
		/// Constructs a <see cref="Deadline"/> using a specific point in time.
		/// </summary>
		/// <param name="duration">The amount of time left until the <see cref="Deadline"/>.</param>
		/// <returns>The newly created <see cref="Deadline"/>.</returns>
		public static Deadline In(TimeSpan duration) => new(DateTimeOffset.UtcNow + duration);
	}
}