using System;
using Planly.DomainModel.Time;

namespace Planly.Application.Schedules
{
	/// <summary>
	/// A range of active hours in which sessions can be scheduled automatically.
	/// Hours are represented by a time offset from midnight (00:00) at UTC.
	/// </summary>
	public record ActiveHoursDto(TimeSpan Start, TimeSpan End)
	{
		internal static ActiveHoursDto From(HourRange activeHours)
		{
			return new ActiveHoursDto(ToTimeSpan(activeHours.Start), ToTimeSpan(activeHours.End));
		}

		private static TimeSpan ToTimeSpan(TimeOfDay timeOfDay)
		{
			return timeOfDay.DurationSinceMidnight.ToTimeSpan();
		}
	}
}