using Carrotware.CMS.Interface;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web;

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

	public enum ManageMessageId {
		AddPhoneSuccess,
		ChangePasswordSuccess,
		SetTwoFactorSuccess,
		SetPasswordSuccess,
		RemoveLoginSuccess,
		RemovePhoneSuccess,
		Error
	}

	public enum LogonStatus {
		NotFound,
		PendingEmail,
		LockedOut,
		InvalidCredentials,
		Authenticated
	}

	public enum RequestReset {
		InvalidCredentials,
		OK
	}

	//================

	public class ManageSecurity {
		private CarrotSecurityConfig _config;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly Controller _controller;

		public ManageSecurity() {
			_userManager = CarrotHttpHelper.ServiceProvider.GetService<UserManager<IdentityUser>>();
			_signInManager = CarrotHttpHelper.ServiceProvider.GetService<SignInManager<IdentityUser>>();
			LoadSettings();
		}

		public ManageSecurity(Controller controller) {
			_controller = controller;
			_userManager = _controller.HttpContext.RequestServices.GetService<UserManager<IdentityUser>>();
			_signInManager = _controller.HttpContext.RequestServices.GetService<SignInManager<IdentityUser>>();

			LoadSettings();
		}

		public ManageSecurity(HttpContext context) {
			_userManager = context.RequestServices.GetService<UserManager<IdentityUser>>();
			_signInManager = context.RequestServices.GetService<SignInManager<IdentityUser>>();

			LoadSettings();
		}

		public ManageSecurity(UserManager<IdentityUser> userManager,
								SignInManager<IdentityUser> signInManager) {
			_userManager = userManager;
			_signInManager = signInManager;
			LoadSettings();
		}

		protected void LoadSettings() {
			_config = CarrotSecurityConfig.GetConfig(CarrotHttpHelper.Configuration);
		}

		public async Task<LogonStatus> LoginAsync(string username, string password) {
			var status = LogonStatus.InvalidCredentials;

			var user = await _userManager.FindByNameAsync(username);

			if (user != null && !user.EmailConfirmed) {
				status = LogonStatus.PendingEmail;
				return status;
			}

			if (user == null || await _userManager.CheckPasswordAsync(user, password) == false) {
				status = LogonStatus.InvalidCredentials;
				return status;
			}

			var result = await _signInManager.PasswordSignInAsync(username, password, true, true);

			if (result.Succeeded) {
				status = LogonStatus.Authenticated;
			} else if (result.IsLockedOut) {
				status = LogonStatus.LockedOut;
			} else {
				status = LogonStatus.InvalidCredentials;
			}

			return status;
		}

		public async Task<RequestReset> ForgotPassword(string email) {
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null) {
				return RequestReset.InvalidCredentials;
			}

			//TODO: message body etc.

			var token = await _userManager.GeneratePasswordResetTokenAsync(user);

			var host = CarrotWebHelper.BuildHttpHost();
			var baseRoute = _config.AdditionalSettings.ResetPath;

			var uri = $"{host}{baseRoute}?token={HttpUtility.UrlEncode(token)}&email={HttpUtility.UrlEncode(user.Email)}";

			string body = $"<p>Hello {user.UserName}, your password reset link is <a href=\"{uri}\">here</a> ";

			var message = MailRequest.Create();
			message.ConfigureMessage(user.Email, "Reset password token", body);

			await message.SendEmailAsync();

			return RequestReset.OK;
		}

		public async Task<IdentityUser> FindByUsernameAsync(string userName) {
			return await this.UserManager.FindByNameAsync(userName);
		}

		public async Task<Guid> GetIdByUsernameAsync(string userName) {
			var u = await this.FindByUsernameAsync(userName);
			return new Guid(u.Id);
		}

		public async Task<IdentityUser> FindByEmailAsync(string email) {
			return await this.UserManager.FindByEmailAsync(email);
		}

		public async Task<Guid> GetIdByEmailAsync(string email) {
			var u = await this.FindByEmailAsync(email);
			return new Guid(u.Id);
		}

		public async Task<bool> IsInRoleAsync(string userName, string roleName) {
			var user = await this.UserManager.FindByNameAsync(userName);
			return await this.UserManager.IsInRoleAsync(user, roleName);
		}

		public async Task<IList<string>> GetRolesAsync(string userName) {
			var user = await this.UserManager.FindByNameAsync(userName);

			return await this.UserManager.GetRolesAsync(user);
		}

		public UserManager<IdentityUser> UserManager {
			get {
				return _controller == null ? _userManager : _controller.HttpContext.RequestServices.GetService<UserManager<IdentityUser>>();
			}
		}

		public SignInManager<IdentityUser> SignInManager {
			get {
				return _controller == null ? _signInManager : _controller.HttpContext.RequestServices.GetService<SignInManager<IdentityUser>>();
			}
		}

		public void LogoutSession(HttpContext context) {
			this.SignInManager.SignOutAsync();
			context.Session.Clear();
		}

		public async Task<IdentityUser> FindByNameAsync(string userName) {
			return await this.UserManager.FindByNameAsync(userName);
		}

		public async Task<bool> SimpleLogInAsync(string userName, string password, bool rememberMe) {
			var user = await this.UserManager.FindByNameAsync(userName);

			if (user == null) {
				return false;
			}

			var result = await this.SignInManager.PasswordSignInAsync(userName, password, rememberMe, true);

			return result?.Succeeded == true;
		}
	}
}