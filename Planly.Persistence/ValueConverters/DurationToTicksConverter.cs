using System;
using Planly.DomainModel.Time;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Planly.Persistence.ValueConverters
{
	internal class DurationToTicksConverter : ValueConverter<Duration, long>
	{
		public DurationToTicksConverter() : base(
			duration => duration.ToTimeSpan().Ticks,
			ticks => new Duration(TimeSpan.FromTicks(ticks)))
		{
		}
	}
}