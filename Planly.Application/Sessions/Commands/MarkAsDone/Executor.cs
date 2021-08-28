using System.Threading;
using System.Threading.Tasks;
using Planly.Application.Common.Exceptions;
using Planly.Application.Identity;
using Planly.Application.Validation;
using Planly.DomainModel;
using Planly.DomainModel.Sessions;

namespace Planly.Application.Sessions.Commands.MarkAsDone
{
	internal class Executor : ICommandExecutor<MarkSessionAsDoneCommand>
	{
		private readonly IIdentityProvider identityProvider;
		private readonly ISessionRepository sessionRepository;

		public Executor(ISessionRepository sessionRepository, IIdentityProvider identityProvider)
		{
			this.sessionRepository = sessionRepository;
			this.identityProvider = identityProvider;
		}

		public async Task ExecuteAsync(MarkSessionAsDoneCommand command, CancellationToken cancellationToken)
		{
			var session = await GetSessionAsync(command, cancellationToken);

			CheckAuthorization(session);

			if (session.Canceled)
			{
				var error = new RequestValidationError(
					Code: "SessionCanceled",
					Message: "Cannot mark the session as done because it was canceled",
					Target: nameof(command.SessionId));
				throw new InvalidRequestException(error);
			}

			session.MarkAsDone();
		}

		private void CheckAuthorization(Session session)
		{
			var userId = identityProvider.GetCurrentUserId();
			if (session.ScheduleId.ToGuid() != userId)
				throw new UnauthorizedRequestException();
		}

		private async Task<Session> GetSessionAsync(MarkSessionAsDoneCommand command, CancellationToken cancellationToken)
		{
			var sessionId = new Identifier<Session>(command.SessionId);
			var session = await sessionRepository.FindByIdAsync(sessionId, cancellationToken);
			if (session is null)
				throw new ResourceNotFoundException();
			return session;
		}
	}
}