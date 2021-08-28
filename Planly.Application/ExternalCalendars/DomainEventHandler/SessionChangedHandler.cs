using System;
using System.Threading;
using Planly.DomainModel;
using Planly.DomainModel.ExternalCalendars;
using Planly.DomainModel.Sessions;

namespace Planly.Application.ExternalCalendars.DomainEventHandler
{
	internal class SessionChangedHandler
		: IDomainEventHandler<SessionDescriptionEdited>, IDomainEventHandler<SessionRescheduled>
	{
		private readonly IExternalCalendarRepository externalCalendarRepository;
		private readonly ISessionRepository sessionRepository;
		private readonly ICalendarSynchronizerFactory synchronizerFactory;

		public SessionChangedHandler(
			ISessionRepository sessionRepository,
			IExternalCalendarRepository externalCalendarRepository,
			ICalendarSynchronizerFactory synchronizerFactory)
		{
			this.sessionRepository = sessionRepository;
			this.externalCalendarRepository = externalCalendarRepository;
			this.synchronizerFactory = synchronizerFactory;
		}

		public System.Threading.Tasks.Task HandleAsync(
			SessionDescriptionEdited domainEvent, CancellationToken cancellationToken)
		{
			return SyncChangesOfAsync(domainEvent.SessionId, cancellationToken);
		}

		public System.Threading.Tasks.Task HandleAsync(
			SessionRescheduled domainEvent, CancellationToken cancellationToken)
		{
			return SyncChangesOfAsync(domainEvent.SessionId, cancellationToken);
		}

		private async System.Threading.Tasks.Task<Session> GetSessionAsync(
			Identifier<Session> sessionId, CancellationToken cancellationToken)
		{
			var session = await sessionRepository.FindByIdAsync(sessionId, cancellationToken);
			if (session is null)
				throw new InvalidOperationException("No such session was found.");
			return session;
		}

		private async System.Threading.Tasks.Task SyncChangesOfAsync(
					Identifier<Session> sessionId, CancellationToken cancellationToken)
		{
			var session = await GetSessionAsync(sessionId, cancellationToken);

			var externalCalendars = await externalCalendarRepository.GetByScheduleIdAsync(
				session.ScheduleId,
				cancellationToken);

			foreach (var externalCalendar in externalCalendars)
				await SyncSessionToAsync(externalCalendar, session, cancellationToken);
		}

		private async System.Threading.Tasks.Task SyncSessionToAsync(
			ExternalCalendar externalCalendar, Session session, CancellationToken cancellationToken)
		{
			var synchronizer = await synchronizerFactory.GetSynchronizerForAsync(externalCalendar, cancellationToken);

			await synchronizer.UpdateEntryAsync(session, cancellationToken);
		}
	}
}