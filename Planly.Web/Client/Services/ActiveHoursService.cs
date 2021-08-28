using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Planly.Web.Client.JsonConverters;
using Planly.Web.Client.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace Planly.Web.Client.Services
{
	public class ActiveHoursService
	{
		private static readonly JsonSerializerOptions JsonOptions = new()
		{
			Converters = { new TimeSpanConverter() },
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		private readonly AuthenticationStateProvider authProvider;
		private readonly HttpClient httpClient;

		public ActiveHoursService(AuthenticationStateProvider authProvider, HttpClient httpClient)
		{
			this.authProvider = authProvider;
			this.httpClient = httpClient;
		}

		public async Task<ActiveHoursDto> GetActiveHoursAsync()
		{
			var userId = await GetCurrentUserIdAsync();
			var endpointUrl = GetActiveHoursEndpointUrl(userId);
			var activeHours = await httpClient.GetFromJsonAsync<ActiveHoursDto>(endpointUrl, JsonOptions);
			var currentTimeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
			activeHours.Start += currentTimeZoneOffset;
			activeHours.End += currentTimeZoneOffset;

			return activeHours;
		}

		public async Task SetActiveHoursAsync(TimeSpan start, TimeSpan end)
		{
			var userId = await GetCurrentUserIdAsync();
			await SetActiveHoursAsync(userId, start, end);
		}

		private static string GetActiveHoursEndpointUrl(string scheduleId)
		{
			return $"/api/schedules/{scheduleId}/active-hours";
		}

		private async Task<string> GetCurrentUserIdAsync()
		{
			var authState = await authProvider.GetAuthenticationStateAsync();
			if (!authState.User.Identity.IsAuthenticated)
				throw new InvalidOperationException("No user is logged in.");

			return authState.User.FindFirst("sub").Value;
		}

		private Task SetActiveHoursAsync(string scheduleId, TimeSpan start, TimeSpan end)
		{
			var currentTimeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
			var activeHours = new ActiveHoursDto
			{
				ScheduleId = scheduleId,
				Start = start - currentTimeZoneOffset,
				End = end - currentTimeZoneOffset
			};

			return httpClient.PutAsJsonAsync(GetActiveHoursEndpointUrl(scheduleId), activeHours, JsonOptions);
		}
	}
}