using System;
using System.Linq.Expressions;
using Planly.DomainModel;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Planly.Persistence
{
	internal static class EntityTypeBuilderExtensions
	{
		public static PropertyBuilder<Identifier<TEntity>> HasIdentifier<TEntity>(this EntityTypeBuilder<TEntity> builder)
			where TEntity : AggregateRoot<TEntity>
		{
			builder.Ignore(e => e.RecentEvents);
			return builder.HasIdentifier(e => e.Id);
		}

		public static PropertyBuilder<Identifier<TEntity>> HasIdentifier<T, TEntity>(
			this EntityTypeBuilder<T> builder,
			Expression<Func<T, Identifier<TEntity>>> propertySelector)
			where T : class
			where TEntity : AggregateRoot<TEntity>
		{
			return builder.Property(propertySelector)
				.HasConversion(id => id.ToGuid(), guid => new Identifier<TEntity>(guid));
		}

		public static PropertyBuilder<Identifier<TEntity>> HasIdentifier<T, TEntity>(
			this EntityTypeBuilder<T> builder,
			string propertyName)
			where T : class
			where TEntity : AggregateRoot<TEntity>
		{
			return builder.Property<Identifier<TEntity>>(propertyName)
				.HasConversion(id => id.ToGuid(), guid => new Identifier<TEntity>(guid));
		}
	}
}