using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Planly.Application.Identity;
using Planly.DomainModel;
using Planly.DomainModel.Schedules;
using Planly.DomainModel.Sessions;

namespace Planly.Application.Sessions.Queries.List
{
	internal class Executor : IQueryExecutor<ListSessionsQuery, IReadOnlyList<SessionDto>>
	{
		private readonly IIdentityProvider identityProvider;
		private readonly ISessionRepository sessionRepository;

		public Executor(IIdentityProvider identityProvider, ISessionRepository sessionRepository)
		{
			this.identityProvider = identityProvider;
			this.sessionRepository = sessionRepository;
		}

		public async Task<IReadOnlyList<SessionDto>> ExecuteAsync(
			ListSessionsQuery query, CancellationToken cancellationToken)
		{
			var userId = identityProvider.GetCurrentUserId();
			var scheduleId = new Identifier<Schedule>(userId);
			var sessions = await sessionRepository.GetByScheduleIdAsync(
				scheduleId,
				query.Offset,
				query.Limit,
				query.RangeStartTime,
				cancellationToken: cancellationToken);

			var dto = sessions.Select(SessionDto.From);

			return dto.ToList().AsReadOnly();
		}
	}
}