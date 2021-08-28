using MediatR;

namespace Planly.Application
{
	/// <summary>
	/// Represents a write request to the system. Commands should be serializable.
	/// </summary>
	public interface ICommand : ICommand<Unit>, IRequest { }

	/// <summary>
	/// Represents a write request to the system. Commands should be serializable.
	/// Note that commands are not meant for returning data. The result should only include
	/// data generated as a result of the command's operation, that is likely needed by most
	/// of the callers of the command.
	/// </summary>
	/// <typeparam name="TResult">The type of the result this command produces.</typeparam>
	public interface ICommand<TResult> : IRequest<TResult> { }
}