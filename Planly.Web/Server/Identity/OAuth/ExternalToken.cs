using System;

namespace Planly.Web.Server.Identity.OAuth
{
	internal record ExternalToken(
		string Provider,
		string UserId,
		string RefreshToken,
		string AccessToken,
		DateTimeOffset ExpirationTime)
	{
		public bool AboutToExpire => (ExpirationTime - DateTimeOffset.Now) < TimeSpan.FromMinutes(1);
	}
}