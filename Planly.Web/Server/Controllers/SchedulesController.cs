using System;
using System.Threading.Tasks;
using Planly.Application.Schedules;
using Planly.Application.Schedules.Commands.ChangeActiveHours;
using Planly.Application.Schedules.Queries.GetActiveHours;
using Planly.Application.Validation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Planly.Web.Server.Controllers
{
	/// <summary>
	/// Exposes endpoints for interacting with Schedules of users.
	/// </summary>
	/// <seealso cref="ControllerBase" />
	[Route("api/[controller]")]
	[ApiController]
	public class SchedulesController : ControllerBase
	{
		private readonly ISender requestSender;

		public SchedulesController(ISender requestSender)
		{
			this.requestSender = requestSender;
		}

		/// <summary>
		/// Gets the range of active hours of a schedule, in which sessions can
		/// be schedules when automatically scheduling sessions for a task.
		/// </summary>
		/// <param name="scheduleId">The schedule's ID.</param>
		/// <response code="200">The active hours range is present in the response's body.</response>
		[HttpGet("{scheduleId}/active-hours")]
		public async Task<ActionResult<ActiveHoursDto>> GetActiveHoursAsync([FromRoute] Guid scheduleId)
		{
			var query = new GetActiveHoursQuery(scheduleId);
			return await requestSender.Send(query);
		}

		/// <summary>
		/// Sets the range of active hours of a schedule, in which sessions can
		/// be schedules when automatically scheduling sessions for a task.
		/// </summary>
		/// <param name="scheduleId">The schedule's ID.</param>
		/// <param name="command">The command.</param>
		/// <response code="204">The active hours range was set.</response>
		/// <response code="400">The request is invalid. e.g. the URL and body contain different IDs.</response>
		[HttpPut("{scheduleId}/active-hours")]
		public async Task<IActionResult> SetActiveHoursAsync(
			[FromRoute] Guid scheduleId, [FromBody] ChangeActiveHoursCommand command)
		{
			if (scheduleId != command.ScheduleId)
			{
				var error = new RequestValidationError(
					Code: "PathConflictsWithBody",
					Message: "The path and body of the request contain different ID values",
					Target: nameof(command.ScheduleId));
				throw new InvalidRequestException(error);
			}

			await requestSender.Send(command);

			return NoContent();
		}
	}
}