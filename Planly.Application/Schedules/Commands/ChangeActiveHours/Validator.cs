using System;
using FluentValidation;

namespace Planly.Application.Schedules.Commands.ChangeActiveHours
{
	internal class Validator : AbstractValidator<ChangeActiveHoursCommand>
	{
		public Validator()
		{
			RuleFor(c => c.End)
				.GreaterThan(c => c.Start)
				.Must((command, end) => (end - command.Start) < TimeSpan.FromHours(24));
		}
	}
}