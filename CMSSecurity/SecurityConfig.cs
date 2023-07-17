using Carrotware.CMS.Data.Models;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.Security {

	public static class SecurityConfig {

		public static void ConfigureCmsAuth(this IServiceCollection services, IConfigurationRoot config) {
			var securitySettings = CarrotSecurityConfig.GetConfig(config);

			services.AddDbContext<AppIdentityDbContext>(opt => opt.UseSqlServer(config.GetConnectionString("CarrotwareCMS")));

			// inc here because of password recovery - part of auth
			var emailConfig = SmtpSettings.GetEMailSettings(config);
			services.AddSingleton(emailConfig);

			bool setCookieExpireTimeSpan = securitySettings.AdditionalSettings.SetCookieExpireTimeSpan;
			string loginPath = securitySettings.AdditionalSettings.LoginPath;
			string unauthorized = securitySettings.AdditionalSettings.Unauthorized;
			double expireTimeSpan = securitySettings.AdditionalSettings.ExpireTimeSpan;
			double validateInterval = securitySettings.AdditionalSettings.ValidateInterval;

			if (expireTimeSpan < 5) {
				expireTimeSpan = 5;
			}
			if (validateInterval < 5) {
				validateInterval = 5;
			}

			//because otherwise you'll get constantly logged out
			if (expireTimeSpan < validateInterval) {
				expireTimeSpan = validateInterval + 3;
			}

			double cookieLife = (setCookieExpireTimeSpan ? expireTimeSpan : validateInterval) + 2;

			services.ConfigureApplicationCookie(opt => {
				opt.ExpireTimeSpan = TimeSpan.FromMinutes(cookieLife);
				opt.SlidingExpiration = true;
				opt.Events.OnSigningIn = context => {
					context.Properties.IsPersistent = true;
					context.Properties.AllowRefresh = true;

					if (context.Properties.IsPersistent) {
						var issued = context.Properties.IssuedUtc ?? DateTimeOffset.UtcNow.AddMinutes(-15);
						context.Properties.ExpiresUtc = issued.AddDays(7);
					}
					return Task.FromResult(0);
				};
			});

			//services.ConfigureApplicationCookie(options =>
			//{
			//	options.ExpireTimeSpan = TimeSpan.FromMinutes(cookieLife);
			//	options.SlidingExpiration = true;
			//});

			//services.AddIdentity<IdentityUser, IdentityRole>();

			services.Configure<IdentityOptions>(opt => {
				opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(securitySettings.AdditionalSettings.DefaultLockoutTimeSpan);
				opt.Lockout.MaxFailedAccessAttempts = securitySettings.AdditionalSettings.MaxFailedAccessAttempts;
				opt.Password.RequireDigit = securitySettings.PasswordValidator.RequireDigit;
				opt.Password.RequiredLength = securitySettings.PasswordValidator.RequiredLength;
				opt.Password.RequireLowercase = securitySettings.PasswordValidator.RequireLowercase;
				opt.Password.RequireNonAlphanumeric = securitySettings.PasswordValidator.RequireNonAlphanumeric;
				opt.Password.RequireUppercase = securitySettings.PasswordValidator.RequireUppercase;
				opt.SignIn.RequireConfirmedAccount = false;
				opt.SignIn.RequireConfirmedEmail = false;
				opt.SignIn.RequireConfirmedPhoneNumber = false;
				opt.User.AllowedUserNameCharacters = securitySettings.UserValidator.AllowedUserNameCharacters;
				opt.User.RequireUniqueEmail = securitySettings.UserValidator.RequireUniqueEmail;
			});

			services.Configure<DataProtectionTokenProviderOptions>(opt =>
					opt.TokenLifespan = TimeSpan.FromHours(securitySettings.AdditionalSettings.TokenLifespan));

			services.AddDefaultIdentity<IdentityUser>()
				.AddRoles<IdentityRole>()
				.AddDefaultTokenProviders()
				.AddEntityFrameworkStores<AppIdentityDbContext>();

			services.ConfigureApplicationCookie(opt => {
				opt.LoginPath = loginPath;
				opt.AccessDeniedPath = unauthorized;
			});

			services.AddTransient<UserManager<IdentityUser>>();
			services.AddTransient<SignInManager<IdentityUser>>();
		}

	}
}