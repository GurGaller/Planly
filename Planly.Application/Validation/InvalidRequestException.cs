using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace Planly.Application.Validation
{
	/// <summary>
	/// The exception that is thrown when a request with logical errors is made
	/// (a request that is always invalid, regardless of the state of the system).
	/// </summary>
	[Serializable]
	public class InvalidRequestException : Exception
	{
		/// <summary>
		/// Initializes a new <see cref="InvalidRequestException"/> with a collection of errors.
		/// </summary>
		/// <param name="errors">The errors in the request that caused it to be invalid.</param>
		public InvalidRequestException(params RequestValidationError[] errors)
		{
			SetErrors(errors);
		}

		/// <summary>
		/// Initializes a new <see cref="InvalidRequestException"/> with a collection of errors and a message.
		/// </summary>
		/// <param name="errors">The errors in the request that caused it to be invalid.</param>
		/// <param name="message">The exception's message.</param>
		public InvalidRequestException(IEnumerable<RequestValidationError> errors, string message) : base(message)
		{
			SetErrors(errors);
		}

		/// <summary>
		/// Initializes a new <see cref="InvalidRequestException"/> with a collection of errors.
		/// </summary>
		/// <param name="errors">The errors in the request that caused it to be invalid.</param>
		protected InvalidRequestException(IEnumerable<RequestValidationError> errors, SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
			SetErrors(errors);
		}

		/// <summary>
		/// The errors in the request that caused it to be invalid.
		/// </summary>
		public IReadOnlyCollection<RequestValidationError> Errors { get; private set; }

		[MemberNotNull(nameof(Errors))]
		private void SetErrors(IEnumerable<RequestValidationError> errors)
		{
			Errors = errors.ToList().AsReadOnly();
		}
	}
}