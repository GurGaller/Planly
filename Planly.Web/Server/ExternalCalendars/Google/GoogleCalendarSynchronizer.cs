using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Planly.Application.ExternalCalendars;
using Planly.DomainModel;
using Planly.DomainModel.Sessions;

namespace Planly.Web.Server.ExternalCalendars.Google
{
	internal class GoogleCalendarSynchronizer : ICalendarSynchronizer
	{
		private const string CalendarId = "primary";
		private const string SessionIdEventField = "Planly_SessionId";
		private readonly CalendarService calendarService;
		private readonly GoogleApiCommandBuffer commandBuffer;

		public GoogleCalendarSynchronizer(CalendarService calendarService, GoogleApiCommandBuffer commandBuffer)
		{
			this.calendarService = calendarService;
			this.commandBuffer = commandBuffer;
		}

		public async Task CreateEntriesAsync(IEnumerable<Session> sessions, CancellationToken cancellationToken = default)
		{
			foreach (var session in sessions)
				await CreateEntryAsync(session, cancellationToken);
		}

		public Task CreateEntryAsync(Session session, CancellationToken cancellationToken = default)
		{
			var @event = SessionToEvent(session);

			var importCommand = calendarService.Events.Import(@event, CalendarId);
			commandBuffer.Add(importCommand);

			return Task.CompletedTask;
		}

		public async Task RemoveEntryAssociatedWithSessionAsync(
			Identifier<Session> sessionId, CancellationToken cancellationToken = default)
		{
			var @event = await FindEventBySessionIdAsync(sessionId, cancellationToken);

			if (@event is not null)
			{
				var deleteCommand = calendarService.Events.Delete(CalendarId, @event.Id);
				commandBuffer.Add(deleteCommand);
			}
		}

		public async Task UpdateEntryAsync(Session session, CancellationToken cancellationToken = default)
		{
			var @event = await FindEventBySessionIdAsync(session.Id, cancellationToken);

			if (@event is null)
			{
				await CreateEntryAsync(session, cancellationToken);
			}
			else
			{
				UpdateEvent(@event, session);

				var updateCommand = calendarService.Events.Patch(@event, CalendarId, @event.Id);
				commandBuffer.Add(updateCommand);
			}
		}

		private static Event SessionToEvent(Session session)
		{
			return new Event
			{
				ICalUID = session.ICalendarId.Id,
				Start = session.Time.StartTime.ToGoogleEventTime(),
				End = session.Time.EndTime.ToGoogleEventTime(),
				Summary = session.Description.Title,
				ExtendedProperties = new()
				{
					Private__ = new Dictionary<string, string>()
					{
						{ SessionIdEventField, session.Id.ToGuid().ToString() }
					}
				}
			};
		}

		private static void UpdateEvent(Event @event, Session session)
		{
			@event.Start = session.Time.StartTime.ToGoogleEventTime();
			@event.End = session.Time.EndTime.ToGoogleEventTime();
			@event.Summary = session.Description.Title;
			@event.ExtendedProperties = new()
			{
				Private__ = new Dictionary<string, string>()
				{
					{ SessionIdEventField, session.Id.ToGuid().ToString() }
				}
			};
		}

		private async Task<Event?> FindEventBySessionIdAsync(
					Identifier<Session> sessionId, CancellationToken cancellationToken)
		{
			var listRequest = calendarService.Events.List(CalendarId);
			listRequest.PrivateExtendedProperty = $"{SessionIdEventField}={sessionId.ToGuid()}";
			listRequest.SingleEvents = true;

			var eventList = await listRequest.ExecuteAsync(cancellationToken);
			var @event = eventList.Items.SingleOrDefault();
			if (@event is not null)
			{
				@event.Start.TimeZone ??= eventList.TimeZone;
				@event.End.TimeZone ??= eventList.TimeZone;
			}
			return @event;
		}
	}
}