using System;

namespace Planly.DomainModel
{
	/// <summary>
	/// Uniquely identifies an entity of a certain type.
	/// </summary>
	/// <typeparam name="TEntity">The entity's type.</typeparam>
	public record Identifier<TEntity> where TEntity : Entity<TEntity>
	{
		private readonly Guid internalId;

		/// <summary>
		/// Initializes a new instance of the <see cref="Identifier{TEntity}"/> class using a <see cref="Guid"/>.
		/// </summary>
		/// <param name="internalId">The internal identifier.</param>
		public Identifier(Guid internalId)
		{
			this.internalId = internalId;
		}

		/// <summary>
		/// Generates a new, unique identifier of a <typeparamref name="TEntity"/>.
		/// </summary>
		/// <returns>The generated identifier.</returns>
		public static Identifier<TEntity> GenerateNew()
		{
			return new Identifier<TEntity>(Guid.NewGuid());
		}

		/// <summary>
		/// Gets the internal object used for identification.
		/// </summary>
		public Guid ToGuid()
		{
			return internalId;
		}
	}
}