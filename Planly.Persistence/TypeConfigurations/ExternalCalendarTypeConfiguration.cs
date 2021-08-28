using Planly.DomainModel.ExternalCalendars;
using Planly.DomainModel.Schedules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Planly.Persistence.TypeConfigurations
{
	internal class ExternalCalendarTypeConfiguration : IEntityTypeConfiguration<ExternalCalendar>
	{
		public void Configure(EntityTypeBuilder<ExternalCalendar> builder)
		{
			builder.ToTable("ExternalCalendars");

			builder.HasIdentifier();

			builder.HasIdentifier(c => c.ScheduleId);
			builder.HasOne<Schedule>()
				.WithMany()
				.HasForeignKey(c => c.ScheduleId);

			builder.OwnsOne(c => c.Provider, provider =>
			{
				provider.Property(p => p.Name)
					.HasMaxLength(64)
					.HasColumnName("Provider");
			});
		}
	}
}