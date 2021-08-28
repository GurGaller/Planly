using System;
using System.Collections.Generic;
using Planly.Application.Sessions.Queries;
using Planly.Application.Tasks.Commands.Abandon;
using Planly.Application.Tasks.Commands.Capture;
using Planly.Application.Tasks.Commands.EditDetails;
using Planly.Application.Tasks.Queries;
using Planly.Application.Tasks.Queries.GetById;
using Planly.Application.Tasks.Queries.List;
using Planly.Application.Validation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Planly.Web.Server.Controllers
{
	/// <summary>
	/// Exposed endpoints for managing Tasks.
	/// </summary>
	/// <seealso cref="ControllerBase" />
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class TasksController : ControllerBase
	{
		private readonly ISender requestSender;

		public TasksController(ISender requestSender)
		{
			this.requestSender = requestSender;
		}

		/// <summary>
		/// Captures a new task of the current user.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <response code="201">The task was captured.</response>
		[HttpPost]
		public async System.Threading.Tasks.Task<ActionResult<TaskDto>> CreateAsync(
			[FromBody] CaptureTaskCommand command)
		{
			var taskId = await requestSender.Send(command);

			var taskDto = new TaskDto(
				taskId,
				command.Title,
				command.TotalTimeRequired,
				TimeCompleted: TimeSpan.Zero,
				command.IdealSessionDuration,
				command.Deadline);

			return CreatedAtAction("GetById", "Tasks", new { taskDto.Id }, taskDto);
		}

		/// <summary>
		/// Abandons a task.
		/// </summary>
		/// <param name="id">The task's ID.</param>
		/// <response code="204">The task was abandoned.</response>
		[HttpDelete("{id}")]
		public async System.Threading.Tasks.Task<IActionResult> DeleteAsync(Guid id)
		{
			var command = new AbandonTaskCommand(id);

			await requestSender.Send(command);

			return NoContent();
		}

		/// <summary>
		/// Gets a list of tasks of the current user.
		/// </summary>
		/// <param name="offset">The number of tasks to skip before starting the current page.</param>
		/// <param name="limit">The maximum number of tasks to return in this page.</param>
		/// <response code="200">The response's body contain the paginated list of tasks.</response>
		[HttpGet]
		public async System.Threading.Tasks.Task<ActionResult<IEnumerable<SessionDto>>> GetAsync(
			int offset = 0, int limit = 10)
		{
			var query = new ListTasksQuery(limit, offset);

			var tasks = await requestSender.Send(query);

			return Ok(tasks);
		}

		/// <summary>
		/// Gets a task by its ID.
		/// </summary>
		/// <param name="id">The task's ID.</param>
		/// <response code="200">The task is encoded in the response's body.</response>
		[HttpGet("{id}")]
		public async System.Threading.Tasks.Task<ActionResult<TaskDto>> GetByIdAsync(Guid id)
		{
			var query = new GetTaskByIdQuery(id);

			var task = await requestSender.Send(query);

			return task;
		}

		/// <summary>
		/// Edits the details of a task.
		/// </summary>
		/// <param name="id">The task's ID.</param>
		/// <param name="command">The command.</param>
		/// <response code="400">The request is invalid. e.g. the URL and body contain different IDs.</response>
		[HttpPut("{id}")]
		public async System.Threading.Tasks.Task<IActionResult> PutAsync(
			Guid id, [FromBody] EditTaskDetailsCommand command)
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