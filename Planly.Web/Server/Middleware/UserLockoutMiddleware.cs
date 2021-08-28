using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Planly.Web.Server.Identity;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Http;

namespace Planly.Web.Server.Middleware
{
	/// <summary>
	/// A middleware that blocks requests from locked-out users.
	/// </summary>
	public class UserLockoutMiddleware
	{
		private readonly RequestDelegate next;

		public UserLockoutMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task InvokeAsync(HttpContext context, CustomUserManager userManager)
		{
			if (context.User.IsAuthenticated())
			{
				var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
				var user = await userManager.FindByIdAsync(userId);
				if (user is not null)
				{
					if (await userManager.IsLockedOutAsync(user))
					{
						context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
						return;
					}
				}
			}

			await next(context);
		}
	}
}