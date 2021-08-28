using System.Threading;
using System.Threading.Tasks;
using Planly.Application.Identity;
using Planly.DomainModel;
using Planly.DomainModel.ExternalCalendars;
using Planly.DomainModel.Schedules;

namespace Planly.Application.ExternalCalendars.Commands.Disconnect
{
	internal class Executor : ICommandExecutor<DisconnectExternalCalendarCommand>
	{
		private readonly IExternalCalendarRepository externalCalendarRepository;
		private readonly IIdentityProvider identityProvider;

		public Executor(IExternalCalendarRepository externalCalendarRepository, IIdentityProvider identityProvider)
		{
			this.externalCalendarRepository = externalCalendarRepository;
			this.identityProvider = identityProvider;
		}

		public async Task ExecuteAsync(DisconnectExternalCalendarCommand command, CancellationToken cancellationToken)
		{
			var provider = new CalendarProvider(command.Provider);

			var userId = identityProvider.GetCurrentUserId();
			var scheduleId = new Identifier<Schedule>(userId);
			var externalCalendar = await externalCalendarRepository.FindAsync(scheduleId, provider, cancellationToken);

			externalCalendar?.Disconnect();
		}
	}
}