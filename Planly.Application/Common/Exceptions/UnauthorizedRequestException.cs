using System;

namespace Planly.Application.Common.Exceptions
{
	/// <summary>
	/// The exception that is thrown when a request was made by a user that is not authorized to make it.
	/// </summary>
	/// <seealso cref="Exception" />
	public class UnauthorizedRequestException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UnauthorizedRequestException"/> class.
		/// </summary>
		public UnauthorizedRequestException()
		{
		}

		/// <inheritdoc/>
		public UnauthorizedRequestException(string? message) : base(message)
		{
		}

		/// <inheritdoc/>
		public UnauthorizedRequestException(string? message, Exception? innerException) : base(message, innerException)
		{
		}
	}
}