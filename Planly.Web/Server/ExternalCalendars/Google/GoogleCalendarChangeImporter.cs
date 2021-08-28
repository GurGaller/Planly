using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Google;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Planly.Application.ExternalCalendars;
using Planly.Application.Identity;
using Planly.Application.Sessions.Commands.Cancel;
using Planly.Application.Sessions.Commands.EditDetails;
using Planly.Application.Sessions.Commands.Import;
using Planly.Application.Sessions.Commands.Schedule;
using Planly.DomainModel.ExternalCalendars;
using Planly.Web.Server.Identity;

namespace Planly.Web.Server.ExternalCalendars.Google
{
	/// <summary>
	/// Syncs changes in an external Google calendar to its matching local schedule.
	/// </summary>
	internal class GoogleCalendarChangeImporter
	{
		private const string CalendarId = "primary";
		private const string CancelledStatus = "cancelled";
		private const string SessionIdEventField = "Planly_SessionId";
		private const string SyncTokenName = "SyncToken";
		private static readonly SemaphoreSlim Semaphore = new(initialCount: 100);
		private readonly GoogleCalendarServiceFactory calendarServiceFactory;
		private readonly IExternalCalendarKeyStore keyStore;
		private readonly IServiceProvider serviceProvider;

		public GoogleCalendarChangeImporter(
			IExternalCalendarKeyStore keyStore,
			GoogleCalendarServiceFactory calendarServiceFactory,
			IServiceProvider serviceProvider)
		{
			this.keyStore = keyStore;
			this.calendarServiceFactory = calendarServiceFactory;
			this.serviceProvider = serviceProvider;
		}

		/// <summary>
		/// Imports the changes from an external Google calendar to the local store.
		/// </summary>
		/// <param name="calendar">The external calendar. The provider of the calendar must be Google.</param>
		/// <param name="cancellationToken">A token for canceling the operation.</param>
		public async Task ImportChangesAsync(ExternalCalendar calendar, CancellationToken cancellationToken = default)
		{
			var syncToken = await keyStore.FindAsync(calendar.Id.ToGuid(), SyncTokenName, cancellationToken);

			var userId = calendar.ScheduleId.ToGuid();

			var calendarService = await calendarServiceFactory.CreateAsync(userId.ToString(), cancellationToken);
			var listRequest = PrepareListRequest(calendarService, syncToken);
			await ImportEventListAsync(listRequest, calendar, userId, syncToken, cancellationToken);
		}

		private static IBaseRequest GetCommand(Event @event)
		{
			if (@event.Status is CancelledStatus)
				return new CancelSessionCommand(Guid.Parse(@event.ICalUID));

			if (@event.ExtendedProperties?.Private__ is not null)
			{
				if (@event.ExtendedProperties.Private__.TryGetValue(SessionIdEventField, out var textualSessionId))
				{
					var sessionId = Guid.Parse(textualSessionId);
					return new EditSessionDetailsCommand(
						sessionId,
						@event.Summary,
						@event.Start.ToDateTimeOffset(),
						@event.End.ToDateTimeOffset());
				}
			}

			return new ImportSessionCommand(
				@event.Summary,
				@event.Start.ToDateTimeOffset(),
				@event.End.ToDateTimeOffset(),
				@event.ICalUID);
		}

		private static EventsResource.ListRequest PrepareListRequest(
			CalendarService calendarService,
			ExternalCalendarKey? syncToken)
		{
			var listRequest = calendarService.Events.List(CalendarId);

			if (syncToken is not null)
			{
				listRequest.SyncToken = syncToken.Value;
			}
			else
			{
				listRequest.TimeZone = "Etc/UTC";
				listRequest.TimeMin = DateTime.UtcNow;
			}

			listRequest.SingleEvents = true;

			return listRequest;
		}

		private static Task<Events> RequestNextPageAsync(
			EventsResource.ListRequest listRequest,
			Task<Events> previousResponseTask,
			Events listResponse,
			CancellationToken cancellationToken)
		{
			if (listResponse.NextPageToken is null)
				return previousResponseTask;

			listRequest.PageToken = listResponse.NextPageToken;
			return listRequest.ExecuteAsync(cancellationToken);
		}

		private async Task ImportEventAsync(Event @event, Guid userId, CancellationToken cancellationToken)
		{
			if (@event.Start?.DateTime is null)
				return; // Ignore all-day events
			await Semaphore.WaitAsync(cancellationToken);
			try
			{
				using var serviceScope = serviceProvider.CreateScope();
				var identityProvider = (HttpIdentityProvider)serviceScope.ServiceProvider
					.GetRequiredService(typeof(IIdentityProvider));
				identityProvider.DefaultUserId = userId;

				var requestSender = serviceScope.ServiceProvider.GetRequiredService<ISender>();

				var command = GetCommand(@event);

				var result = await requestSender.Send(command, cancellationToken);
				if (command is ScheduleSessionCommand)
				{
					var sessionId = (Guid)result!;
					await UpdateRemoteSessionIdAsync(@event, userId, sessionId, cancellationToken);
				}
			}
			finally
			{
				Semaphore.Release();
			}
		}

		private async Task ImportEventListAsync(
			EventsResource.ListRequest listRequest,
			ExternalCalendar calendar,
			Guid userId,
			ExternalCalendarKey? syncToken,
			CancellationToken cancellationToken)
		{
			var nextResponseTask = listRequest.ExecuteAsync(cancellationToken);
			Events listResponse;
			do
			{
				try
				{
					listResponse = await nextResponseTask;
				}
				catch (GoogleApiException ex)
				{
					if (ex.Error.Code == (int)HttpStatusCode.Gone && syncToken is not null)
						keyStore.Remove(syncToken);

					return;
				}
				nextResponseTask = RequestNextPageAsync(listRequest, nextResponseTask, listResponse, cancellationToken);

				StoreNextSyncToken(calendar, syncToken, listResponse);

				var importTasks = listResponse.Items.Select(i => ImportEventAsync(i, userId, cancellationToken));
				await Task.WhenAll(importTasks);
			} while (listResponse.NextPageToken is not null);
		}

		private void StoreNextSyncToken(ExternalCalendar calendar, ExternalCalendarKey? syncToken, Events listResponse)
		{
			if (listResponse.NextSyncToken is null)
				return;

			if (syncToken is null)
			{
				syncToken = new ExternalCalendarKey(
					calendar.Id.ToGuid(),
					SyncTokenName,
					listResponse.NextSyncToken);
				keyStore.Store(syncToken);
			}
			else
			{
				syncToken.Value = listResponse.NextSyncToken;
			}
		}

		private async Task UpdateRemoteSessionIdAsync(
			Event @event,
			Guid userId,
			Guid sessionId,
			CancellationToken cancellationToken)
		{
			var commandBuffer = serviceProvider.GetRequiredService<GoogleApiCommandBuffer>();
			var calendarService = await calendarServiceFactory.CreateAsync(userId.ToString(), cancellationToken);
			var patchBody = new Event
			{
				ExtendedProperties = new()
				{
					Private__ = new Dictionary<string, string>
					{
						{ SessionIdEventField, sessionId.ToString() }
					}
				}
			};
			var updateCommand = calendarService.Events.Patch(patchBody, CalendarId, @event.Id);
			commandBuffer.Add(updateCommand);
		}
	}
}