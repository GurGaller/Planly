using System;

namespace Planly.DomainModel.Time
{
	/// <summary>
	/// A range of hours in a day.
	/// </summary>
	public record HourRange
	{
		/// <summary>
		/// A time range of a full day (00:00 - 23:59)
		/// </summary>
		public static readonly HourRange AllDay;

		static HourRange()
		{
			var start = new TimeOfDay(Duration.Zero);
			var end = new TimeOfDay(new Duration(TimeSpan.FromHours(24) - TimeSpan.FromTicks(1)));
			AllDay = new HourRange(start, end);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HourRange"/> class using the
		/// <paramref name="start"/> and <paramref name="end"/> of the range.
		/// </summary>
		/// <param name="start">The start of the range.</param>
		/// <param name="end">The end of range.</param>
		public HourRange(TimeOfDay start, TimeOfDay end)
		{
			Start = start;
			End = end;
		}

		private HourRange()
		{
			// For reconstruction
		}
		/// <summary>
		/// Gets the start of the range.
		/// </summary>
		public TimeOfDay Start { get; }
		/// <summary>
		/// Gets the end of the range.
		/// </summary>
		public TimeOfDay End { get; }
	}
}