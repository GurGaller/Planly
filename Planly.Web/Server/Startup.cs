using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Google.Apis.Calendar.v3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Planly.Application;
using Planly.Application.ExternalCalendars;
using Planly.Application.Identity;
using Planly.Persistence;
using Planly.Web.Server.BackgroundJobs;
using Planly.Web.Server.Configuration;
using Planly.Web.Server.Data;
using Planly.Web.Server.ExternalCalendars;
using Planly.Web.Server.ExternalCalendars.Google;
using Planly.Web.Server.Filters;
using Planly.Web.Server.Identity;
using Planly.Web.Server.Identity.OAuth;
using Planly.Web.Server.JsonConverters;
using Planly.Web.Server.Middleware;
using Planly.Web.Server.Models;

namespace Planly.Web.Server
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint("/swagger/v1/swagger.json", "Planly API V1");
				options.RoutePrefix = "api";
			});

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseMigrationsEndPoint();
				app.UseWebAssemblyDebugging();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseBlazorFrameworkFiles();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseIdentityServer();
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseMiddleware<UserLockoutMiddleware>();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();
				endpoints.MapControllers();
				endpoints.MapFallbackToFile("index.html");
			});
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddApplicationLayer();

			services.AddTransient<ICalendarSynchronizerFactory, DefaultSynchronizerFactory>();
			services.AddTransient<GoogleSynchronizerFactory>();
			services.AddTransient<GoogleCalendarServiceFactory>();
			services.AddSingleton<GoogleApiCommandBuffer>();
			services.AddTransient<GoogleCalendarChangeImporter>();
			services.AddTransient<TokenStore>();

			services.AddScoped<IIdentityProvider, HttpIdentityProvider>();

			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(
					Configuration.GetConnectionString("DefaultConnection")));

			services.AddEFCorePersistence();

			services.AddDatabaseDeveloperPageExceptionFilter();

			services.AddDefaultIdentity<ApplicationUser>(options =>
			{
				options.SignIn.RequireConfirmedAccount = false;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequiredLength = 8;
				options.Password.RequireUppercase = false;
				options.Password.RequireDigit = false;
			})
			.AddRoles<IdentityRole>()
			.AddUserManager<CustomUserManager>()
			.AddEntityFrameworkStores<ApplicationDbContext>();

			services.AddIdentityServer()
				.AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
				{
					options.IdentityResources["openid"].UserClaims.Add("role");
					options.ApiResources.Single().UserClaims.Add("role");
				});
			System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("role");

			services.AddAuthentication()
				.AddIdentityServerJwt()
				.AddGoogle(options =>
				{
					options.ClientId = Configuration["Google:ClientId"];
					options.ClientSecret = Configuration["Google:ClientSecret"];
					options.Scope.Add(CalendarService.ScopeConstants.Calendar);
					options.SaveTokens = true;
					options.AccessType = "offline";
				});

			services.AddControllersWithViews(options =>
			{
				options.Filters.Add<ApplicationExceptionFilter>();
				options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
			})
			.AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
			});
			services.AddRazorPages();

			services.AddHostedService<ProcessDomainEvents>();
			services.AddHostedService<SendGoogleCommands>();
			services.AddHostedService<ImportChangesFromGoogle>();

			services.AddSwaggerGen(options =>
			{
				options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
				{
					Description = "JWT Authorization header using the Bearer scheme. Syntax: \"Bearer {token}\"",
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey
				});
				options.OperationFilter<ErrorResponseOperationFilter>();

				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				options.IncludeXmlComments(xmlPath);
			});
		}
	}
}