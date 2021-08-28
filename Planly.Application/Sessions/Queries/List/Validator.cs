using FluentValidation;

namespace Planly.Application.Sessions.Queries.List
{
	internal class Validator : AbstractValidator<ListSessionsQuery>
	{
		public Validator()
		{
			RuleFor(q => q.Limit)
				.GreaterThanOrEqualTo(0)
				.LessThanOrEqualTo(256);

			RuleFor(q => q.Offset).GreaterThanOrEqualTo(0);
		}
	}
}