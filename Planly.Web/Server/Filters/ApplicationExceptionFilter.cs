using Planly.Application.Common.Exceptions;
using Planly.Application.Identity;
using Planly.Application.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Planly.Web.Server.Filters
{
	internal class ApplicationExceptionFilter : IExceptionFilter
	{
		public void OnException(ExceptionContext context)
		{
			context.Result = context.Exception switch
			{
				ResourceNotFoundException => new NotFoundResult(),
				UnauthorizedRequestException => new ForbidResult(),
				UnauthenticatedRequestException => new UnauthorizedResult(),
				InvalidRequestException ex => new BadRequestObjectResult(new { ex.Errors }),
				_ => null
			};
			context.ExceptionHandled = context.Result is not null;
		}
	}
}