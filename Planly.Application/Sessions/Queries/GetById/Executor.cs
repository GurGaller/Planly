using System.Threading;
using System.Threading.Tasks;
using Planly.Application.Common.Exceptions;
using Planly.Application.Identity;
using Planly.DomainModel;
using Planly.DomainModel.Sessions;

namespace Planly.Application.Sessions.Queries.GetById
{
	internal class Executor : IQueryExecutor<GetSessionByIdQuery, SessionDto>
	{
		private readonly IIdentityProvider identityProvider;
		private readonly ISessionRepository sessionRepository;

		public Executor(ISessionRepository sessionRepository, IIdentityProvider identityProvider)
		{
			this.sessionRepository = sessionRepository;
			this.identityProvider = identityProvider;
		}

		public async Task<SessionDto> ExecuteAsync(GetSessionByIdQuery query, CancellationToken cancellationToken)
		{
			var sessionId = new Identifier<Session>(query.SessionId);
			var session = await sessionRepository.FindByIdAsync(sessionId, cancellationToken);
			if (session is null)
				throw new ResourceNotFoundException();

			var userId = identityProvider.GetCurrentUserId();
			if (session.ScheduleId.ToGuid() != userId)
				throw new UnauthorizedRequestException();

			return SessionDto.From(session);
		}
	}
}