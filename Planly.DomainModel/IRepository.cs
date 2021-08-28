using System.Threading;

namespace Planly.DomainModel
{
	/// <summary>
	/// A collection of entities of the same type.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entities in the repository.</typeparam>
	public interface IRepository<TEntity> where TEntity : AggregateRoot<TEntity>
	{
		/// <summary>
		/// Adds an entity to the repository.
		/// </summary>
		/// <param name="entity">The new entity.</param>
		void Add(TEntity entity);

		/// <summary>
		/// Finds an entity in this repository by its unique identifier.
		/// </summary>
		/// <param name="id">The entity's unique identifier.</param>
		/// <param name="cancellationToken">A token used to cancel the operation.</param>
		/// <returns>The requested entity, or <c>null</c> if no such entity was found.</returns>

		System.Threading.Tasks.Task<TEntity?> FindByIdAsync(
			Identifier<TEntity> id,
			CancellationToken cancellationToken = default);
	}
}