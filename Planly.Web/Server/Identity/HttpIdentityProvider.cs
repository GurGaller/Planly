using System;
using System.Security.Claims;
using Planly.Application.Identity;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Http;

namespace Planly.Web.Server.Identity
{
	internal class HttpIdentityProvider : IIdentityProvider
	{
		private readonly IHttpContextAccessor httpContextAccessor;

		public HttpIdentityProvider(IHttpContextAccessor httpContextAccessor)
		{
			this.httpContextAccessor = httpContextAccessor;
		}

		internal Guid? DefaultUserId { private get; set; }

		public Guid GetCurrentUserId()
		{
			var httpContext = httpContextAccessor.HttpContext;

			if (httpContext is null)
				return DefaultUserId ?? throw new Exception("Cannot find the current user.");

			if (!httpContext.User.IsAuthenticated())
				throw new UnauthenticatedRequestException();

			var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			userId ??= httpContext.User.GetSubjectId();
			return Guid.Parse(userId);
		}
	}
}