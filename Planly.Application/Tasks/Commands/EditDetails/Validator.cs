using System;
using FluentValidation;

namespace Planly.Application.Tasks.Commands.EditDetails
{
	internal class Validator : AbstractValidator<EditTaskDetailsCommand>
	{
		public Validator()
		{
			RuleFor(c => c.Title)
				.NotEmpty()
				.MaximumLength(256);

			RuleFor(c => c.IdealSessionDuration)
				.GreaterThan(TimeSpan.Zero)
				.LessThanOrEqualTo(c => c.TotalTimeRequired);

			RuleFor(c => c.Deadline).GreaterThan(DateTimeOffset.Now);
		}
	}
}