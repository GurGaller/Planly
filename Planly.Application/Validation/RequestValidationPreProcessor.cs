using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using MediatR.Pipeline;

namespace Planly.Application.Validation
{
	/// <summary>
	/// Validates every request before it is handled.
	/// </summary>
	public class RequestValidationPreProcessor<TRequest> : IRequestPreProcessor<TRequest>
		where TRequest : IBaseRequest
	{
		private readonly List<IValidator<TRequest>> validators;

		/// <summary>
		/// Initializes a <see cref="RequestValidationPreProcessor{TRequest}"/> using a collection of validators.
		/// </summary>
		/// <param name="validators">A collection of objects that validates a <typeparamref name="TRequest"/>.</param>
		/// <remarks>If no validators were passed, every request is considered valid.</remarks>
		public RequestValidationPreProcessor(IEnumerable<IValidator<TRequest>> validators)
		{
			this.validators = validators.ToList();
		}

		/// <inheritdoc/>
		public async Task Process(TRequest request, CancellationToken cancellationToken)
		{
			var errors = (await ValidateAsync(request, cancellationToken)).ToArray();
			if (errors.Any())
				throw new InvalidRequestException(errors);
		}

		private static RequestValidationError FailureToError(ValidationFailure failure)
		{
			return new RequestValidationError(
				Code: failure.ErrorCode,
				Message: failure.ErrorMessage,
				Target: failure.PropertyName);
		}

		private static IEnumerable<RequestValidationError> GetErrors(ValidationResult validationResult)
		{
			return validationResult.Errors
				.Select(FailureToError);
		}

		private static async Task<IEnumerable<RequestValidationError>> ValidateUsingAsync(IValidator<TRequest> validator, TRequest request, CancellationToken cancellationToken)
		{
			var validationResult = await validator.ValidateAsync(request, cancellationToken);
			return GetErrors(validationResult);
		}

		private async Task<IEnumerable<RequestValidationError>> ValidateAsync(TRequest request, CancellationToken cancellationToken)
		{
			var errors = new List<RequestValidationError>();
			foreach (var validator in validators)
			{
				var validationErrors = await ValidateUsingAsync(validator, request, cancellationToken);
				errors.AddRange(validationErrors);
			}
			return errors;
		}
	}
}