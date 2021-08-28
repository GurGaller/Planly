using FluentValidation;

namespace Planly.Application.Sessions.Commands.EditDetails
{
	internal class Validator : AbstractValidator<EditSessionDetailsCommand>
	{
		public Validator()
		{
			RuleFor(c => c.Title)
				.NotEmpty()
				.MaximumLength(256);
			RuleFor(c => c.EndTime).GreaterThanOrEqualTo(c => c.StartTime);
		}
	}
}