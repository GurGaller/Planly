using Planly.DomainModel.Schedules;
using Planly.DomainModel.Tasks;
using Planly.Persistence.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Planly.Persistence.TypeConfigurations
{
	internal class TaskTypeConfiguration : IEntityTypeConfiguration<Task>
	{
		public void Configure(EntityTypeBuilder<Task> builder)
		{
			builder.ToTable("Tasks");

			builder.HasIdentifier();

			builder.HasIdentifier(t => t.ScheduleId);

			builder.HasOne<Schedule>()
				.WithMany()
				.HasForeignKey(t => t.ScheduleId);

			builder.Property(t => t.Deadline)
				.HasConversion(deadline => deadline.Time, time => Deadline.Until(time));

			builder.Property(t => t.IdealSessionDuration)
				.HasConversion(new DurationToTicksConverter());

			builder.OwnsOne(t => t.Description, b =>
			{
				b.Property(d => d.Title)
					.HasMaxLength(256)
					.HasColumnName("Title");
			});

			builder.OwnsOne(t => t.Progress, b =>
			{
				b.Property(time => time.TimeRequired)
					.HasConversion(new DurationToTicksConverter())
					.HasColumnName("TimeRequired");
				b.Property(time => time.TimeCompleted)
					.HasConversion(new DurationToTicksConverter())
					.HasColumnName("TimeCompleted");
			});
		}
	}
}