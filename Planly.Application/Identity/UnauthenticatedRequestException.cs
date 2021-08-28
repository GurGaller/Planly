using System;

namespace Planly.Application.Identity
{
	/// <summary>
	/// The exception that is thrown when an unauthenticated user makes a request that requires authentication.
	/// </summary>
	/// <seealso cref="Exception" />
	public class UnauthenticatedRequestException : Exception
	{
		/// <inheritdoc/>
		public UnauthenticatedRequestException()
		{
		}

		/// <inheritdoc/>
		public UnauthenticatedRequestException(string? message) : base(message)
		{
		}

		/// <inheritdoc/>
		public UnauthenticatedRequestException(string? message, Exception? innerException) : base(message, innerException)
		{
		}
	}
}