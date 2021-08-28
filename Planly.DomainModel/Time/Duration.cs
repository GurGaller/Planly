using System;

namespace Planly.DomainModel.Time
{
	/// <summary>
	/// A non-negative amount of time.
	/// </summary>
	public record Duration
	{
		private readonly TimeSpan timeSpan;

		/// <summary>
		/// Initializes a new instance of the <see cref="Duration"/> class using a <see cref="TimeSpan"/>.
		/// </summary>
		/// <param name="timeSpan">The amount of time.</param>
		/// <exception cref="ArgumentException">When a negative amount of time is passed.</exception>
		public Duration(TimeSpan timeSpan)
		{
			if (timeSpan < TimeSpan.Zero)
				throw new ArgumentException("A duration of time must be positive.", nameof(timeSpan));

			this.timeSpan = timeSpan;
		}
		/// <summary>
		/// Gets an object that represents a zero amount of time.
		/// </summary>
		public static readonly Duration Zero = new(TimeSpan.Zero);
		/// <summary>
		/// Determines whether this <see cref="Duration"/> is longer than another specified <see cref="Duration"/>.
		/// </summary>
		/// <param name="other">The other <see cref="Duration"/>.</param>
		/// <returns>
		///   <c>true</c> if this <see cref="Duration"/> is longer than the other
		///   specified <see cref="Duration"/>; otherwise, <c>false</c>.
		/// </returns>
		public bool IsLongerThan(Duration other)
		{
			return timeSpan > other.timeSpan;
		}

		/// <summary>
		/// Converts to <see cref="TimeSpan"/>.
		/// </summary>
		/// <returns></returns>
		public TimeSpan ToTimeSpan()
		{
			return timeSpan;
		}
		/// <summary>
		/// Implements the operator for dividing two <see cref="Duration"/>s.
		/// </summary>
		/// <param name="left">The numerator.</param>
		/// <param name="right">The denominator.</param>
		/// <returns>
		/// The result of the division.
		/// </returns>
		public static double operator /(Duration left, Duration right) => left.timeSpan / right.timeSpan;
		/// <summary>
		/// Implements the operator for dividing a <see cref="Duration"/> by a real number.
		/// </summary>
		/// <param name="left">The numerator.</param>
		/// <param name="right">The denominator.</param>
		/// <returns>
		/// The result of the division.
		/// </returns>
		public static Duration operator /(Duration left, double right) => new(left.timeSpan / right);

		/// <summary>
		/// Determines whether this <see cref="Duration"/> is longer than
		/// or equal to another specified <see cref="Duration"/>.
		/// </summary>
		/// <param name="other">The other <see cref="Duration"/>.</param>
		/// <returns>
		///   <c>true</c> if this <see cref="Duration"/> is longer than or equal to the other
		///   specified <see cref="Duration"/>; otherwise, <c>false</c>.
		/// </returns>
		public bool IsLongerOrEqualTo(Duration other)
		{
			return timeSpan >= other.timeSpan;
		}

		/// <summary>
		/// Implements the operator for adding <see cref="Duration"/>s.
		/// </summary>
		/// <param name="left">The first <see cref="Duration"/>.</param>
		/// <param name="right">The second <see cref="Duration"/>.</param>
		/// <returns>
		/// The sum of the two <see cref="Duration"/>s.
		/// </returns>
		public static Duration operator +(Duration left, Duration right) => new(left.timeSpan + right.timeSpan);
	}
}