using System;
using System.Threading;
using System.Threading.Tasks;
using Planly.Application.Common.Exceptions;
using Planly.Application.Identity;
using Planly.Application.Validation;
using Planly.DomainModel;
using Planly.DomainModel.Sessions;

namespace Planly.Application.Sessions.Commands.Cancel
{
	internal class Executor : ICommandExecutor<CancelSessionCommand>
	{
		private readonly IIdentityProvider identityProvider;
		private readonly ISessionRepository sessionRepository;

		public Executor(ISessionRepository sessionRepository, IIdentityProvider identityProvider)
		{
			this.sessionRepository = sessionRepository;
			this.identityProvider = identityProvider;
		}

		public async Task ExecuteAsync(CancelSessionCommand command, CancellationToken cancellationToken)
		{
			var session = await GetSessionAsync(command, cancellationToken);

			VerifyAuthorization(session);

			try
			{
				session.Cancel();
			}
			catch (InvalidOperationException)
			{
				var error = new RequestValidationError(
					Code: "SessionCancelationNotPossible",
					Message: "This session cannot be canceled.",
					Target: nameof(command.SessionId));
				throw new InvalidRequestException(error);
			}
		}

		private async Task<Session> GetSessionAsync(CancelSessionCommand command, CancellationToken cancellationToken)
		{
			var sessionId = new Identifier<Session>(command.SessionId);
			var session = await sessionRepository.FindByIdAsync(sessionId, cancellationToken);
			if (session is null)
				throw new ResourceNotFoundException();
			return session;
		}

		private void VerifyAuthorization(Session session)
		{
			var userId = identityProvider.GetCurrentUserId();
			if (session.ScheduleId.ToGuid() != userId)
				throw new UnauthorizedRequestException();
		}
	}
}