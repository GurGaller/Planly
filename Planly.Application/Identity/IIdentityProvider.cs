using System;

namespace Planly.Application.Identity
{
	/// <summary>
	/// An object that provides the identity of users who make requests to the system.
	/// </summary>
	public interface IIdentityProvider
	{
		/// <summary>
		/// Gets the ID of the user making the current request.
		/// </summary>
		/// <returns>The user's ID.</returns>
		/// <exception cref="UnauthenticatedRequestException">When the current user is not authenticated.</exception>
		Guid GetCurrentUserId();
	}
}