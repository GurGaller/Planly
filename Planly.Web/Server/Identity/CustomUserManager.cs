using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Planly.Application.ExternalCalendars.Commands.Connect;
using Planly.Application.ExternalCalendars.Commands.Disconnect;
using Planly.Application.Schedules.Commands.Create;
using Planly.DomainModel.Schedules;
using Planly.Web.Server.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Planly.Web.Server.Identity
{
	public class CustomUserManager : UserManager<ApplicationUser>
	{
		private readonly ISender requestSender;
		private readonly IScheduleRepository scheduleRepository;

		public CustomUserManager(
			IUserStore<ApplicationUser> store,
			IOptions<IdentityOptions> optionsAccessor,
			IPasswordHasher<ApplicationUser> passwordHasher,
			IEnumerable<IUserValidator<ApplicationUser>> userValidators,
			IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
			ILookupNormalizer keyNormalizer,
			IdentityErrorDescriber errors,
			IServiceProvider services,
			ILogger<UserManager<ApplicationUser>> logger,
			IScheduleRepository scheduleRepository,
			ISender requestSender)
			: base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators,
				  keyNormalizer, errors, services, logger)
		{
			this.scheduleRepository = scheduleRepository;
			this.requestSender = requestSender;
		}

		public override async Task<IdentityResult> AddLoginAsync(ApplicationUser user, UserLoginInfo login)
		{
			var result = await base.AddLoginAsync(user, login);

			if (result.Succeeded)
			{
				if (login is ExternalLoginInfo externalLogin)
				{
					foreach (var token in externalLogin.AuthenticationTokens)
						await SetAuthenticationTokenAsync(user, externalLogin.LoginProvider, token.Name, token.Value);
				}

				var userId = Guid.Parse(user.Id);
				await requestSender.Send(new ConnectExternalCalendarCommand(userId, login.LoginProvider));
			}

			return result;
		}

		public override async Task<IdentityResult> CreateAsync(ApplicationUser user)
		{
			var result = await base.CreateAsync(user);

			if (result.Succeeded)
			{
				var userId = Guid.Parse(user.Id);
				await requestSender.Send(new CreateScheduleCommand(userId));
			}

			return result;
		}

		public override async Task<IdentityResult> RemoveLoginAsync(
			ApplicationUser user, string loginProvider, string providerKey)
		{
			var result = await base.RemoveLoginAsync(user, loginProvider, providerKey);

			if (result.Succeeded)
				await requestSender.Send(new DisconnectExternalCalendarCommand(loginProvider));

			return result;
		}
	}
}