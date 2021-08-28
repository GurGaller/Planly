using FluentValidation;

namespace Planly.Application.Sessions.Commands.Import
{
	internal class Validator : AbstractValidator<ImportSessionCommand>
	{
		public Validator()
		{
			RuleFor(c => c.EndTime).GreaterThanOrEqualTo(c => c.StartTime);
			RuleFor(c => c.Title)
				.NotEmpty()
				.MaximumLength(256);
			RuleFor(c => c.ICalendarId)
				.NotEmpty()
				.MaximumLength(512);
		}
	}
}