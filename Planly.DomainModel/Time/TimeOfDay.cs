using System;

namespace Planly.DomainModel.Time
{
	/// <summary>
	/// An hour of a day.
	/// </summary>
	public record TimeOfDay
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TimeOfDay"/> class.
		/// </summary>
		/// <param name="durationSinceMidnight">
		/// The <see cref="Duration"/> of time between midnight and this <see cref="TimeOfDay"/>.
		/// </param>
		public TimeOfDay(Duration durationSinceMidnight)
		{
			DurationSinceMidnight = durationSinceMidnight;
		}

		private TimeOfDay()
		{
			// For reconstruction
		}

		/// <summary>
		/// Gets the <see cref="Duration"/> of time between midnight and this <see cref="TimeOfDay"/>.
		/// </summary>
		public Duration DurationSinceMidnight { get; }

		/// <summary>
		/// Finds the <see cref="TimeOfDay"/> for a given <see cref="DateTimeOffset"/>.
		/// </summary>
		/// <param name="day">The <see cref="DateTimeOffset"/>.</param>
		/// <param name="relativeTo">
		/// A different <see cref="DateTimeOffset"/> to use for reference instead of midnight (UTC).
		/// </param>
		public static TimeOfDay Of(DateTimeOffset day, DateTimeOffset? relativeTo = null)
		{
			var duration = new Duration(day.UtcDateTime.TimeOfDay);
			if (relativeTo is not null)
				duration = new Duration(day.UtcDateTime - relativeTo.Value.UtcDateTime);
			return new TimeOfDay(duration);
		}

		/// <summary>
		/// Gets this <see cref="TimeOfDay"/> on a specified <paramref name="date"/>.
		/// </summary>
		/// <param name="date">The date.</param>
		/// <returns>
		/// The exact point in time of this <see cref="TimeOfDay"/> on the specified <paramref name="date"/>
		/// </returns>
		public DateTimeOffset Of(DateTimeOffset date)
		{
			var newDate = date.Date.Add(DurationSinceMidnight.ToTimeSpan());
			return new DateTimeOffset(newDate, offset: TimeSpan.Zero);
		}

		/// <summary>
		/// Determines whether the specified hour is after this <see cref="TimeOfDay"/>.
		/// </summary>
		/// <param name="hour">The hour.</param>
		/// <returns>
		/// <see langword="true"/> if the specified hour is after this
		/// <see cref="TimeOfDay"/>; otherwise, <see langword="false"/>.
		/// </returns>
		public bool IsBefore(TimeOfDay hour)
		{
			return hour.DurationSinceMidnight.IsLongerThan(DurationSinceMidnight);
		}
	}
}