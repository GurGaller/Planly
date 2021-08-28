using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Planly.Persistence.TypeConfigurations
{
	internal class DomainEventTypeConfiguration : IEntityTypeConfiguration<DomainEventWrapper>
	{
		public void Configure(EntityTypeBuilder<DomainEventWrapper> builder)
		{
			builder.ToTable("DomainEvents");
		}
	}
}