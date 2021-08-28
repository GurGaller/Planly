using Planly.Application.ExternalCalendars;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Planly.Persistence.TypeConfigurations
{
	internal class ExternalCalendarKeyTypeConfiguration : IEntityTypeConfiguration<ExternalCalendarKey>
	{
		public void Configure(EntityTypeBuilder<ExternalCalendarKey> builder)
		{
			builder.ToTable("ExternalCalendarKeys");

			builder.HasKey(k => new { k.CalendarId, k.Name });

			builder.Property(k => k.Name)
				.HasMaxLength(256);
		}
	}
}