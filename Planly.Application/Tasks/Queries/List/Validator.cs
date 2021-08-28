using FluentValidation;

namespace Planly.Application.Tasks.Queries.List
{
	internal class Validator : AbstractValidator<ListTasksQuery>
	{
		public Validator()
		{
			RuleFor(q => q.Limit).InclusiveBetween(0, 256);
			RuleFor(q => q.Offset).GreaterThanOrEqualTo(0);
		}
	}
}