using System;
using System.Runtime.Serialization;

namespace Planly.Application.Common.Exceptions
{
	/// <summary>
	/// The exception that is thrown when a request is made, that involves a resource that does not exist.
	/// </summary>
	[Serializable]
	public class ResourceNotFoundException : Exception
	{
		/// <inheritdoc/>
		public ResourceNotFoundException()
		{
		}

		/// <inheritdoc/>
		public ResourceNotFoundException(string? message) : base(message)
		{
		}

		/// <inheritdoc/>
		public ResourceNotFoundException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		/// <inheritdoc/>
		protected ResourceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}