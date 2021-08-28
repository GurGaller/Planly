using Planly.DomainModel.Schedules;
using Planly.Persistence.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Planly.Persistence.TypeConfigurations
{
	internal class ScheduleTypeConfiguration : IEntityTypeConfiguration<Schedule>
	{
		public void Configure(EntityTypeBuilder<Schedule> builder)
		{
			builder.ToTable("Schedules");

			builder.HasIdentifier();

			builder.OwnsOne(s => s.ActiveHours, b =>
			{
				b.OwnsOne(r => r.Start, x =>
				{
					x.Property(t => t.DurationSinceMidnight)
						.HasConversion(new DurationToTicksConverter());
				});

				b.OwnsOne(r => r.End, x =>
				{
					x.Property(t => t.DurationSinceMidnight)
						.HasConversion(new DurationToTicksConverter());
				});
			});
		}
	}
}