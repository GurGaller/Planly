using FluentValidation;

namespace Planly.Application.Sessions.Commands.Schedule
{
	internal class Validator : AbstractValidator<ScheduleSessionCommand>
	{
		public Validator()
		{
			RuleFor(c => c.EndTime).GreaterThanOrEqualTo(c => c.StartTime);
			RuleFor(c => c.Title)
				.NotEmpty()
				.MaximumLength(256);
		}
	}
}