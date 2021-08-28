using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Planly.Web.Server.Identity.OAuth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Util;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Configuration;

namespace Planly.Web.Server.ExternalCalendars.Google
{
	internal class GoogleCalendarServiceFactory
	{
		private readonly IConfiguration configuration;
		private readonly HttpClient httpClient;
		private readonly TokenStore tokenStore;

		public GoogleCalendarServiceFactory(TokenStore tokenStore, IConfiguration configuration, HttpClient httpClient)
		{
			this.tokenStore = tokenStore;
			this.configuration = configuration;
			this.httpClient = httpClient;
		}

		public async Task<CalendarService> CreateAsync(string userId, CancellationToken cancellationToken)
		{
			var token = await GetAuthenticationTokenAsync(userId, cancellationToken);
			var credential = GoogleCredential.FromAccessToken(token.AccessToken);

			return new CalendarService(new()
			{
				HttpClientInitializer = credential
			});
		}

		private Task<TokenResponse> FetchNewTokenAsync(ExternalToken token, CancellationToken cancellationToken)
		{
			var refreshRequest = new RefreshTokenRequest
			{
				ClientId = configuration["Google:ClientId"],
				ClientSecret = configuration["Google:ClientSecret"],
				RefreshToken = token.RefreshToken
			};

			return refreshRequest.ExecuteAsync(
				httpClient,
				GoogleAuthConsts.OidcTokenUrl,
				cancellationToken,
				SystemClock.Default);
		}

		private async Task<ExternalToken> GetAuthenticationTokenAsync(string userId, CancellationToken cancellationToken)
		{
			var token = await tokenStore.GetTokenAsync(userId, GoogleDefaults.AuthenticationScheme, cancellationToken);

			if (token.AboutToExpire)
				token = await RefreshAccessTokenAsync(token, cancellationToken);

			return token;
		}

		private async Task<ExternalToken> RefreshAccessTokenAsync(ExternalToken token, CancellationToken cancellationToken)
		{
			var response = await FetchNewTokenAsync(token, cancellationToken);

			token = token with
			{
				AccessToken = response.AccessToken,
				ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(response.ExpiresInSeconds!.Value)
			};

			await tokenStore.UpdateTokenAsync(token, cancellationToken);

			return token;
		}
	}
}