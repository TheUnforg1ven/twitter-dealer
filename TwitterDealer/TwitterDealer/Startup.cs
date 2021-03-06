using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using TwitterDealer.Data;
using TwitterDealer.Data.Entities;
using TwitterDealer.Helpers;
using TwitterDealer.Interfaces;
using TwitterDealer.Models;
using TwitterDealer.Repositories;
using TwitterDealer.Services;
using TwitterDealer.TwitterApi;

namespace TwitterDealer
{
	public class Startup
	{
		public Startup(IWebHostEnvironment webHostEnvironment)
		{
			Configuration = new ConfigurationBuilder()
				.SetBasePath(webHostEnvironment.ContentRootPath)
				.AddJsonFile("appsettings.json")
				.Build();
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddTransient<Seed>();
			services.AddTransient<ISaveThreadRepository, SaveThreadRepository>();

			services.AddTransient<IUserService, UserService>();
			services.AddTransient<ITweetDataService, TweetDataService>();
			services.AddTransient<ITweetThreadService, TweetThreadService>();
			
			services.AddScoped<IAuthRepository, AuthRepository>();
			services.AddScoped<IDealerRepository, DealerRepository>();

			services.AddHttpContextAccessor();

			// inject app settings
			services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));

			services.AddControllers().AddNewtonsoftJson(o =>
			{
				o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
			});

			services.AddDbContext<AppDbContext>(options =>
			options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

			services.Configure<IdentityOptions>(options => 
			{
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireDigit = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireUppercase = false;
				options.Password.RequiredLength = 5;
			});

			services.AddCors();
			services.AddAutoMapper(typeof(Startup));

			// jwt web token auth

			var key = Encoding.UTF8.GetBytes(Configuration["ApplicationSettings:JwtSecret"].ToString());

			services.AddAuthentication(x => 
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(x => 
			{
				x.RequireHttpsMetadata = false;
				x.SaveToken = false;
				x.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = false,
					ValidateAudience = false,
					ClockSkew = TimeSpan.Zero
				};
			});

		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Seed seeder)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler(builder =>
				{
					builder.Run(async context => 
					{
						context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

						var error = context.Features.Get<IExceptionHandlerFeature>();
						if (error != null)
						{
							context.Response.AddApplicationError(error.Error.Message);
							await context.Response.WriteAsync(error.Error.Message);
						}
					});
				});
			}

			//seeder.SeedUsers();

			app.UseCors(options => options.WithOrigins(Configuration["ApplicationSettings:ClientUrl"].ToString())
														.AllowAnyMethod()
														.AllowAnyHeader()
														.AllowCredentials());
			app.UseAuthentication();
			app.UseHttpsRedirection();
			app.UseRouting();
			app.UseAuthorization();
		

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			AuthInit.AuthenticateTwitter();
		}
	}
}
