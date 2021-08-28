using System;
using System.Threading;
using System.Threading.Tasks;
using Planly.Application.Identity;
using Planly.DomainModel;
using Planly.DomainModel.Schedules;
using Planly.DomainModel.Sessions;
using Planly.DomainModel.Time;

namespace Planly.Application.Sessions.Commands.Schedule
{
	internal class Executor : ICommandExecutor<ScheduleSessionCommand, Guid>
	{
		private readonly IIdentityProvider identityProvider;
		private readonly IScheduleRepository scheduleRepository;
		private readonly ISessionRepository sessionRepository;

		public Executor(
			IIdentityProvider identityProvider,
			IScheduleRepository scheduleRepository,
			ISessionRepository sessionRepository)
		{
			this.identityProvider = identityProvider;
			this.scheduleRepository = scheduleRepository;
			this.sessionRepository = sessionRepository;
		}

		public async Task<Guid> ExecuteAsync(ScheduleSessionCommand command, CancellationToken cancellationToken)
		{
			var userId = identityProvider.GetCurrentUserId();
			var scheduleId = new Identifier<DomainModel.Schedules.Schedule>(userId);
			var schedule = await scheduleRepository.FindByIdAsync(scheduleId, cancellationToken);
			if (schedule is null)
				throw new Exception("The current user does not have an associated schedule.");

			var sessionDescription = new SessionDescription(command.Title);
			var sessionTime = TimeSlot.Between(command.StartTime, command.EndTime);
			var session = schedule.ScheduleSession(sessionDescription, sessionTime);
			sessionRepository.Add(session);

			return session.Id.ToGuid();
		}
	}
}