using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Planly.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace Planly.Persistence
{
	/// <summary>
	/// The main database context of the application. Takes care of custom infrastructure like domain event persistence.
	/// </summary>
	/// <seealso cref="DbContext" />
	public class CustomDbContext : DbContext
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CustomDbContext"/> class.
		/// </summary>
		/// <param name="options">The options.</param>
		public CustomDbContext(DbContextOptions<CustomDbContext> options) : base(options)
		{
		}

		internal void StoreDomainEventsOfChangedEntities()
		{
			var entities = GetChangedEntities();
			foreach (var entity in entities)
			{
				var method = GetType()
					.GetMethod(nameof(StoreDomainEvents), BindingFlags.Instance | BindingFlags.NonPublic)!
					.MakeGenericMethod(entity.GetType());

				method.Invoke(this, new object[] { entity });
			}
		}

		/// <inheritdoc/>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		}

		private static bool IsDomainEntity(object obj)
		{
			try
			{
				var entityType = typeof(Entity<>).MakeGenericType(obj.GetType());
				return obj.GetType().IsAssignableTo(entityType);
			}
			catch (ArgumentException)
			{
				return false;
			}
		}

		private List<object> GetChangedEntities()
		{
			return base.ChangeTracker.Entries()
				.Select(e => e.Entity)
				.Where(IsDomainEntity)
				.ToList();
		}

		private void StoreDomainEvents<TEntity>(TEntity entity)
			where TEntity : Entity<TEntity>
		{
			foreach (var domainEvent in entity.RecentEvents)
			{
				var wrapper = DomainEventWrapper.Wrap(domainEvent);
				base.Set<DomainEventWrapper>().Add(wrapper);
			}

			entity.ClearRecentEvents();
		}
	}
}