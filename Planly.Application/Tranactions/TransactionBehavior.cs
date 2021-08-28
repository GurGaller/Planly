using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Planly.Application.Tranactions
{
	/// <summary>
	/// Wraps every request with a <see cref="IUnitOfWork"/>.
	/// </summary>
	public class TransactionBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
		where TRequest : ICommand<TResult>
	{
		private readonly IUnitOfWorkFactory unitOfWorkFactory;

		/// <summary>
		/// Initializes a new <see cref="TransactionBehavior{TRequest, TResult}"/> instance.
		/// </summary>
		/// <param name="unitOfWorkFactory">An object that creates units of work.</param>
		public TransactionBehavior(IUnitOfWorkFactory unitOfWorkFactory)
		{
			this.unitOfWorkFactory = unitOfWorkFactory;
		}

		/// <inheritdoc/>
		public async Task<TResult> Handle(
			TRequest request,
			CancellationToken cancellationToken,
			RequestHandlerDelegate<TResult> next)
		{
			await using var unitOfWork = await unitOfWorkFactory.CreateAsync(cancellationToken);

			var result = await next();

			await unitOfWork.CompleteAsync(cancellationToken);

			return result;
		}
	}
}