using System.Threading;
using System.Threading.Tasks;
using Planly.Application.Common.Exceptions;
using Planly.Application.Identity;
using Planly.DomainModel;
using Planly.DomainModel.Schedules;

namespace Planly.Application.Schedules.Queries.GetActiveHours
{
	internal class Executor : IQueryExecutor<GetActiveHoursQuery, ActiveHoursDto>
	{
		private readonly IIdentityProvider identityProvider;
		private readonly IScheduleRepository scheduleRepository;

		public Executor(IScheduleRepository scheduleRepository, IIdentityProvider identityProvider)
		{
			this.scheduleRepository = scheduleRepository;
			this.identityProvider = identityProvider;
		}

		public async Task<ActiveHoursDto> ExecuteAsync(GetActiveHoursQuery query, CancellationToken cancellationToken)
		{
			VerifyAuthorization(query);

			var scheduleId = new Identifier<Schedule>(query.ScheduleId);
			var schedule = await scheduleRepository.FindByIdAsync(scheduleId, cancellationToken);
			if (schedule is null)
				throw new ResourceNotFoundException();

			return ActiveHoursDto.From(schedule.ActiveHours);
		}

		private void VerifyAuthorization(GetActiveHoursQuery query)
		{
			var userId = identityProvider.GetCurrentUserId();
			if (userId != query.ScheduleId)
				throw new UnauthorizedRequestException();
		}
	}
}