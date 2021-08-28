using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Planly.Web.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Planly.Web.Server.Identity.OAuth
{
	internal class TokenStore
	{
		private const string AccessTokenType = "access_token";
		private const string ExpirationTokenType = "expires_at";
		private const string RefreshTokenType = "refresh_token";
		private readonly ApplicationDbContext dbContext;

		public TokenStore(ApplicationDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public async Task<ExternalToken> GetTokenAsync(
			string localUserId, string tokenProvider, CancellationToken cancellationToken = default)
		{
			var tokens = await GetAuthenticationTokensAsync(localUserId, tokenProvider, cancellationToken);

			var accessToken = tokens.Single(t => t.Name == AccessTokenType).Value;
			var expiration = DateTimeOffset.Parse(tokens.Single(t => t.Name == ExpirationTokenType).Value);
			var refreshToken = tokens.Single(t => t.Name == RefreshTokenType).Value;

			return new ExternalToken(tokenProvider, localUserId, refreshToken, accessToken, expiration);
		}

		public Task UpdateTokenAsync(ExternalToken token, CancellationToken cancellationToken = default)
		{
			dbContext.UserTokens.Update(new()
			{
				LoginProvider = token.Provider,
				Name = AccessTokenType,
				UserId = token.UserId,
				Value = token.AccessToken
			});

			dbContext.UserTokens.Update(new()
			{
				LoginProvider = token.Provider,
				Name = RefreshTokenType,
				UserId = token.UserId,
				Value = token.RefreshToken
			});

			dbContext.UserTokens.Update(new()
			{
				LoginProvider = token.Provider,
				Name = ExpirationTokenType,
				UserId = token.UserId,
				Value = token.ExpirationTime.ToString()
			});

			return dbContext.SaveChangesAsync(cancellationToken);
		}

		private Task<List<IdentityUserToken<string>>> GetAuthenticationTokensAsync(
			string userId, string authenticationScheme, CancellationToken cancellationToken = default)
		{
			return dbContext.UserTokens
				.AsNoTracking()
				.Where(t => t.UserId == userId)
				.Where(t => t.LoginProvider == authenticationScheme)
				.ToListAsync(cancellationToken);
		}
	}
}