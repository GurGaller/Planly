using System.Threading;
using System.Threading.Tasks;
using Planly.Application.Common.Exceptions;
using Planly.Application.Identity;
using Planly.Application.Validation;
using Planly.DomainModel;
using Planly.DomainModel.Sessions;
using Planly.DomainModel.Time;

namespace Planly.Application.Sessions.Commands.EditDetails
{
	internal class Executor : ICommandExecutor<EditSessionDetailsCommand>
	{
		private readonly IIdentityProvider identityProvider;
		private readonly ISessionRepository sessionRepository;

		public Executor(ISessionRepository sessionRepository, IIdentityProvider identityProvider)
		{
			this.sessionRepository = sessionRepository;
			this.identityProvider = identityProvider;
		}

		public async Task ExecuteAsync(EditSessionDetailsCommand command, CancellationToken cancellationToken)
		{
			var sessionId = new Identifier<Session>(command.Id);
			var session = await sessionRepository.FindByIdAsync(sessionId, cancellationToken);
			if (session is null)
				throw new ResourceNotFoundException();

			var userId = identityProvider.GetCurrentUserId();
			if (session.ScheduleId.ToGuid() != userId)
				throw new UnauthorizedRequestException();

			if (session.Done || session.Canceled)
			{
				var error = new RequestValidationError(
					Code: "SessionCannotBeEdited",
					Message: "Only sessions that were not canceled or marked as done can be edited.",
					Target: nameof(command.Id));
				throw new InvalidRequestException(error);
			}

			var description = session.Description with { Title = command.Title };
			session.EditDescription(description);
			var time = TimeSlot.Between(command.StartTime, command.EndTime);
			session.Reschedule(time);
		}
	}
}