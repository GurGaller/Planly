using Planly.DomainModel.Schedules;
using Planly.DomainModel.Sessions;
using Planly.DomainModel.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Planly.Persistence.TypeConfigurations
{
	internal class SessionTypeConfiguration : IEntityTypeConfiguration<Session>
	{
		public void Configure(EntityTypeBuilder<Session> builder)
		{
			builder.ToTable("Sessions");

			builder.HasIdentifier();

			builder.HasIdentifier(s => s.ScheduleId);

			builder.HasOne<Schedule>()
				.WithMany()
				.HasForeignKey(s => s.ScheduleId);

			builder.OwnsOne(s => s.Description, b =>
			{
				b.Property(d => d.Title)
					.HasMaxLength(256)
					.HasColumnName("Title");
			});

			builder.OwnsOne(s => s.Time, b =>
			{
				b.Property(t => t.StartTime)
					.HasColumnName("StartTime");

				b.Property(t => t.EndTime)
					.HasColumnName("EndTime");
			});

			builder.HasOne<Task>()
				.WithMany()
				.IsRequired(false)
				.HasForeignKey(s => s.TaskId)
				.OnDelete(DeleteBehavior.ClientCascade);

			builder.Property(s => s.ICalendarId)
				.HasConversion(id => id.Id, str => new ICalendarIdentifier(str))
				.HasMaxLength(512);
		}
	}
}