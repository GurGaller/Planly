using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Planly.Web.Server.Identity;
using Planly.Web.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Planly.Web.Server.Controllers
{
	/// <summary>
	/// Exposes endpoints for managing users of the system.
	/// </summary>
	/// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "Administrator")]
	public class UsersController : ControllerBase
	{
		private readonly CustomUserManager userManager;

		public UsersController(CustomUserManager userManager)
		{
			this.userManager = userManager;
		}

		/// <summary>
		/// Removes a user from the "Administrator" role.
		/// </summary>
		/// <param name="userId">The user's ID.</param>
		/// <response code="204">The user is no longer an administrator.</response>
		/// <response code="404">There is no such user.</response>
		[HttpDelete("administrators/{userId}")]
		public async Task<IActionResult> DemoteAdminAsync([FromRoute] Guid userId)
		{
			var user = await userManager.FindByIdAsync(userId.ToString());
			if (user is null)
				return NotFound();

			await userManager.RemoveFromRoleAsync(user, role: "Administrator");

			return NoContent();
		}

		/// <summary>
		/// Gets a paginated list of the users in the system.
		/// </summary>
		/// <param name="limit">The maximum number of users to return in the current page.</param>
		/// <param name="offset">The number of users to skip before starting the page.</param>
		/// <response code="200">The response's body contains the paginated list of users.</response>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<UserDto>>> GetAllAsync(int limit, int offset)
		{
			var admins = await userManager.GetUsersInRoleAsync("Administrator");
			var users = (await userManager.Users
				.Skip(offset)
				.Take(limit)
				.ToListAsync())
				.Select(u =>
				{
					return new UserDto
					{
						Id = Guid.Parse(u.Id),
						EmailAddress = u.Email,
						Admin = admins.Contains(u),
						LockedOut = u.LockoutEnabled && u.LockoutEnd < DateTimeOffset.UtcNow
					};
				});
			return Ok(users);
		}

		/// <summary>
		/// Locks a user out.
		/// </summary>
		/// <param name="id">The user's ID.</param>
		/// <response code="204">The user is locked out.</response>
		/// <response code="404">There is no such user.</response>
		[HttpPut("{id}/lock")]
		public Task<IActionResult> LockAsync(Guid id)
		{
			return SetLockoutAsync(id, lockedOut: true);
		}

		/// <summary>
		/// Makes a user an administrator.
		/// </summary>
		/// <param name="request">The request's body.</param>
		/// <response code="204">The user is now an administrator.</response>
		/// <response code="404">There is no such user.</response>
		[HttpPost("administrators")]
		public async Task<IActionResult> MakeAdminAsync([FromBody] MakeAdminRequest request)
		{
			var user = await userManager.FindByIdAsync(request.UserId.ToString());
			if (user is null)
				return NotFound();

			await userManager.AddToRoleAsync(user, role: "Administrator");

			return NoContent();
		}

		/// <summary>
		/// A request to make a user an administrator.
		/// </summary>
		/// <param name="UserId">The user's ID.</param>
		public record MakeAdminRequest(Guid UserId);

		/// <summary>
		/// Unlocks a user.
		/// </summary>
		/// <param name="id">The user's ID.</param>
		/// <response code="204">The user's account was unlocked.</response>
		/// <response code="404">There is no such user.</response>
		[HttpDelete("{id}/lock")]
		public Task<IActionResult> UnlockAsync(Guid id)
		{
			return SetLockoutAsync(id, lockedOut: false);
		}

		private async Task<IActionResult> SetLockoutAsync(Guid id, bool lockedOut)
		{
			var user = await userManager.FindByIdAsync(id.ToString());
			if (user is null)
				return NotFound();

			await userManager.SetLockoutEnabledAsync(user, lockedOut);
			var lockoutEndDate = lockedOut ? DateTimeOffset.MaxValue : null as DateTimeOffset?;
			await userManager.SetLockoutEndDateAsync(user, lockoutEndDate);

			return NoContent();
		}
	}
}