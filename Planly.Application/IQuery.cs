using MediatR;

namespace Planly.Application
{
	/// <summary>
	/// Represents a read request to the system. Queries should be serializable.
	/// Note that queries should not hold any data that is not related to the filtering/formatting
	/// of it's result. Also, queries are not allowed to change the state of the system.
	/// </summary>
	/// <typeparam name="TResult">The type of the result this query produces.</typeparam>
	public interface IQuery<TResult> : IRequest<TResult> { }
}