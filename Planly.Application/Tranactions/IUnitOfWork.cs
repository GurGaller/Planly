using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planly.Application.Tranactions
{
	/// <summary>
	/// Represents a transaction of the persistence mechanism.
	/// </summary>
	public interface IUnitOfWork : IAsyncDisposable
	{
		/// <summary>
		/// Commits all the changes to the persistence mechanism.
		/// </summary>
		Task CompleteAsync(CancellationToken cancellationToken = default);
	}
}