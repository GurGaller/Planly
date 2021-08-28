using System.Threading.Tasks;
using Planly.Web.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Planly.Web.Server.Identity
{
	public class CustomSignInManager : SignInManager<ApplicationUser>
	{
		public CustomSignInManager(
			UserManager<ApplicationUser> userManager,
			IHttpContextAccessor contextAccessor,
			IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
			IOptions<IdentityOptions> optionsAccessor,
			ILogger<SignInManager<ApplicationUser>> logger,
			IAuthenticationSchemeProvider schemes,
			IUserConfirmation<ApplicationUser> confirmation)
			: base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
		{
		}

		public override async Task<SignInResult> ExternalLoginSignInAsync(
			string loginProvider,
			string providerKey,
			bool isPersistent,
			bool bypassTwoFactor)
		{
			var result = await base.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent, bypassTwoFactor);
			if (result.Succeeded)
			{
				var loginInfo = await GetExternalLoginInfoAsync();
				await UpdateExternalAuthenticationTokensAsync(loginInfo);
			}

			return result;
		}
	}
}