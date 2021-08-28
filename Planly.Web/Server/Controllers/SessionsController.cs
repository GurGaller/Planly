using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Planly.Application.Sessions.Commands.Cancel;
using Planly.Application.Sessions.Commands.EditDetails;
using Planly.Application.Sessions.Commands.MarkAsDone;
using Planly.Application.Sessions.Commands.Schedule;
using Planly.Application.Sessions.Queries;
using Planly.Application.Sessions.Queries.GetById;
using Planly.Application.Sessions.Queries.List;
using Planly.Application.Validation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Planly.Web.Server.Controllers
{
	/// <summary>
	/// Exposes endpoints for managing Sessions of schedules.
	/// </summary>
	/// <seealso cref="ControllerBase" />
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class SessionsController : ControllerBase
	{
		private readonly ISender requestSender;

		public SessionsController(ISender requestSender)
		{
			this.requestSender = requestSender;
		}

		/// <summary>
		/// Cancels a future session.
		/// </summary>
		/// <param name="id">The session's ID.</param>
		/// <response code="204">The session was canceled.</response>
		[HttpDelete("{id}")]
		public async Task<IActionResult> CancelAsync(Guid id)
		{
			var command = new CancelSessionCommand(id);

			await requestSender.Send(command);

			return NoContent();
		}

		/// <summary>
		/// Schedules a new session in the current user's schedule.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <response code="201">
		/// The session was successfully created, and its location is available in the Location header of the response.
		/// </response>
		[HttpPost]
		public async Task<ActionResult<SessionDto>> CreateAsync([FromBody] ScheduleSessionCommand command)
		{
			var sessionId = await requestSender.Send(command);

			var dto = new SessionDto(sessionId, command.Title, command.StartTime, command.EndTime);

			return CreatedAtAction("GetById", "Sessions", new { dto.Id }, dto);
		}

		/// <summary>
		/// Gets a list of sessions from the current user's schedule.
		/// </summary>
		/// <param name="offset">The number of sessions to skip before starting the result range.</param>
		/// <param name="limit">The maximum number of sessions to return.</param>
		/// <param name="firstDate">The inclusive start date of the search.</param>
		/// <response code="200">The list of sessions is encoded in the response's body.</response>
		[HttpGet]
		public async Task<ActionResult<IReadOnlyList<SessionDto>>> GetAsync(
			int offset = 0,
			int limit = 10,
			DateTimeOffset? firstDate = null)
		{
			var query = new ListSessionsQuery(limit, offset, firstDate);

			var sessions = await requestSender.Send(query);

			return Ok(sessions);
		}

		/// <summary>
		/// Gets information about a session by its ID.
		/// </summary>
		/// <param name="id">The session's ID.</param>
		/// <response code="200">The session's representation is included in the response's body.</response>
		[HttpGet("{id}")]
		public async Task<ActionResult<SessionDto>> GetByIdAsync(Guid id)
		{
			var query = new GetSessionByIdQuery(id);

			var session = await requestSender.Send(query);

			return session;
		}

		/// <summary>
		/// Marks a session as done.
		/// </summary>
		/// <param name="id">The session's ID.</param>
		/// <response code="204">The session was marked as done.</response>
		[HttpPut("{id}/completeness")]
		public async Task<IActionResult> MarkAsDoneAsync(Guid id)
		{
			var command = new MarkSessionAsDoneCommand(id);

			await requestSender.Send(command);

			return NoContent();
		}

		/// <summary>
		/// Edits the details of a session.
		/// </summary>
		/// <param name="id">The session's ID.</param>
		/// <param name="command">The command.</param>
		/// <response code="204">The details were edited.</response>
		/// <response code="400"> The request is invalid. e.g. the URL and body contain different IDs.</response>
		[HttpPut("{id}")]
		public async Task<IActionResult> PutAsync(Guid id, [FromBody] EditSessionDetailsCommand command)
		{
			if (id != command.Id)
			{
				var error = new RequestValidationError(
					Code: "PathConflictsWithBody",
					Message: "The path and body of the request contain different ID values",
					Target: nameof(command.Id));
				throw new InvalidRequestException(error);
			}

			await requestSender.Send(command);

			return NoContent();
		}
	}
}