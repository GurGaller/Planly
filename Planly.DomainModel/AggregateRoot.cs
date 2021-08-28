namespace Planly.DomainModel
{
	/// <summary>
	/// A marker interface that indicates that an entity is the root of an
	/// object graph that defines a transaction boundary (an Aggregate).
	/// </summary>
	public abstract class AggregateRoot<TEntity> : Entity<TEntity> where TEntity : Entity<TEntity>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AggregateRoot{TEntity}"/> class.
		/// </summary>
		/// <param name="id">A unique identifier of this entity.</param>
		protected AggregateRoot(Identifier<TEntity> id) : base(id)
		{
		}
	}
}