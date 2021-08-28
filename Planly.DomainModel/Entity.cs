using System;
using System.Collections.Generic;

namespace Planly.DomainModel
{
	/// <summary>
	/// A domain concept that can change state over time.
	/// </summary>
	/// <typeparam name="TEntity">The concrete type of the entity.</typeparam>
	public abstract class Entity<TEntity> where TEntity : Entity<TEntity>
	{
		private readonly List<DomainEvent> recentEvents;

		/// <summary>
		/// Initializes a new instance of the <see cref="Entity{TEntity}"/> class using a unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		protected Entity(Identifier<TEntity> id)
		{
			Id = id;
			recentEvents = new List<DomainEvent>();
		}

		/// <summary>
		/// Gets the unique identifier of this <see cref="Entity{TEntity}"/>.
		/// </summary>
		public Identifier<TEntity> Id { get; }

		/// <summary>
		/// Gets the recent domain events that have occurred to this Entity.
		/// </summary>
		public IReadOnlyList<DomainEvent> RecentEvents => recentEvents.AsReadOnly();

		/// <summary>
		/// Implements the operator != for determining if two Entities does not share the same ID.
		/// </summary>
		/// <param name="left">The first <see cref="Entity{TEntity}"/>.</param>
		/// <param name="right">The second <see cref="Entity{TEntity}"/>.</param>
		/// <returns>
		/// <see langword="true"/> if the instances represent two distinct Entities; <see langword="false"/> otherwise.
		/// </returns>
		public static bool operator !=(Entity<TEntity>? left, Entity<TEntity>? right) => !(left == right);

		/// <summary>
		/// Implements the operator != for determining if two Entities share the same ID.
		/// </summary>
		/// <param name="left">The first <see cref="Entity{TEntity}"/>.</param>
		/// <param name="right">The second <see cref="Entity{TEntity}"/>.</param>
		/// <returns>
		/// <see langword="true"/> if the instances represent the same Entity; <see langword="false"/> otherwise.
		/// </returns>
		public static bool operator ==(Entity<TEntity>? left, Entity<TEntity>? right) => Equals(left, right);

		/// <summary>
		/// Clears the list of recent domain events.
		/// </summary>
		public void ClearRecentEvents()
		{
			recentEvents.Clear();
		}

		/// <inheritdoc/>
		public override bool Equals(object? obj)
		{
			return obj is Entity<TEntity> entity && Id == entity.Id;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return HashCode.Combine(Id);
		}

		/// <summary>
		/// Registers a domain event that occurred within this Aggregate.
		/// </summary>
		/// <param name="domainEvent">The domain event.</param>
		protected void RegisterEvent(DomainEvent domainEvent)
		{
			recentEvents.Add(domainEvent);
		}
	}
}