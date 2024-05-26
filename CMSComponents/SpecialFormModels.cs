﻿using Carrotware.CMS.Core;
using Carrotware.CMS.Interface;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Xml.Serialization;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.UI.Components {

	//==================================================
	public class SiteSearch {

		public SiteSearch() {
		}

		public void RestoreQueryString() {
			if (CarrotHttpHelper.QueryString(SiteData.SearchQueryParameter) != null) {
				this.query = CarrotHttpHelper.QueryString(SiteData.SearchQueryParameter).ToString();
			}
		}

		[StringLength(128)]
		public string query { get; set; }
	}

	//==================================================

	public class ContactInfo : FormModelBase {

		public ContactInfo()
			: base() {
			ReconstructSettings();
		}

		public static string Key {
			get {
				return "CMS_contactform";
			}
		}

		public void ReconstructSettings() {
			base.GetSettings(typeof(ContactInfoSettings));
			this.Settings = null;

			if (this.ValidateSettings != null && this.ValidateSettings is ContactInfoSettings) {
				this.Settings = this.ValidateSettings as ContactInfoSettings;
			}
		}

		public Guid Root_ContentID { get; set; }
		public DateTime CreateDate { get; set; }

		[StringLength(32)]
		[Display(Name = "IP")]
		public string? CommenterIP { get; set; }

		[StringLength(256)]
		[Required]
		[Display(Name = "Name")]
		public string CommenterName { get; set; }

		[StringLength(256)]
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string CommenterEmail { get; set; }

		[StringLength(256)]
		[Display(Name = "URL")]
		public string? CommenterURL { get; set; }

		[StringLength(4096)]
		[Required]
		[Display(Name = "Comment")]
		public string PostCommentText { get; set; }

		public ContactInfoSettings? Settings { get; set; }
		public bool IsSaved { get; set; }

		public void SendMail(PostComment pc, ContentPage page) {
			HttpRequest request = CarrotHttpHelper.Request;

			if (this.Settings.NotifyEditors || !string.IsNullOrEmpty(this.Settings.DirectEmailKeyName)) {
				List<string> emails = new List<string>();

				if (this.Settings.NotifyEditors && page != null) {
					emails.Add(page.CreateUser.Email);

					if (page.EditUser.UserId != page.CreateUser.UserId) {
						emails.Add(page.EditUser.Email);
					}

					if (page.CreditUserId.HasValue) {
						emails.Add(page.CreditUser.Email);
					}
				}

				if (!string.IsNullOrEmpty(this.Settings.DirectEmailKeyName)) {
					var email = CarrotHttpHelper.Configuration.GetValue<string>(this.Settings.DirectEmailKeyName).ToString();

					if (!string.IsNullOrEmpty(email)) {
						emails.Add(email);
					}
				}

				string hostName = CarrotHttpHelper.Request.Host.Value.ToLowerInvariant();
				var host = CarrotWebHelper.BuildHttpHost();

				string mailSubject = string.Format("Comment Form From {0}", hostName);

				string sBody = "Name:   " + pc.CommenterName
					+ "\r\nEmail:   " + pc.CommenterEmail
					+ "\r\nURL:   " + pc.CommenterURL
					+ "\r\n-----------------"
					+ "\r\nComment:\r\n" + HttpUtility.HtmlEncode(pc.PostCommentText)
					+ "\r\n=================\r\n"
					+ "\r\nIP:   " + pc.CommenterIP
					+ "\r\nSite URL:   " + string.Format("{0}{1}", host, page.FileName)
					+ "\r\nSite Time:   " + SiteData.CurrentSite.Now.ToString()
					+ "\r\nUTC Time:   " + DateTime.UtcNow.ToString();

				string sEmail = string.Join(";", emails);

				EmailHelper.SendMail(null, sEmail, mailSubject, sBody, false);
			}
		}
	}

	//==================================================

	public class LogoutInfo {

		public LogoutInfo() {
			ReconstructSettings();
		}

		public static string Key {
			get {
				return "CMS_logoutform";
			}
		}

		public void ReconstructSettings() {
			this.Settings = null;

			if (!string.IsNullOrEmpty(this.EncodedSettings)) {
				string xml = this.EncodedSettings.DecodeBase64();
				var xmlSerializer = new XmlSerializer(typeof(LogoutInfoSettings));
				using (var stringReader = new StringReader(xml)) {
					this.Settings = (LogoutInfoSettings)xmlSerializer.Deserialize(stringReader);
				}
			}

			this.IsLoggedIn = SecurityData.IsAuthenticated;
		}

		public string EncodedSettings { get; set; }
		public LogoutInfoSettings Settings { get; set; }
		public bool IsLoggedIn { get; set; }
		public string? RedirectUri { get; set; }

		public virtual ModelStateDictionary ClearOptionalItems(ModelStateDictionary modelState) {
			// these child objects are for display only, and their validation is not needed
			foreach (var ms in modelState.ToArray()) {
				if (ms.Key.ToLowerInvariant().Contains("settings")) {
					modelState.Remove(ms.Key);
				}
			}

			return modelState;
		}
	}

	//==================================================

	public class LoginInfo : FormModelBase {

		public LoginInfo()
			: base() {
			ReconstructSettings();
		}

		public static string Key {
			get {
				return "CMS_loginform";
			}
		}

		public void ReconstructSettings() {
			base.GetSettings(typeof(LoginInfoSettings));
			this.Settings = null;

			if (this.ValidateSettings != null && this.ValidateSettings is LoginInfoSettings) {
				this.Settings = this.ValidateSettings as LoginInfoSettings;
			}

			this.IsLoggedIn = SecurityData.IsAuthenticated;

			this.LogInStatus = this.IsLoggedIn ? SignInResult.Success : SignInResult.Failed;
		}

		[Required]
		[Display(Name = "Username")]
		[StringLength(128)]
		public string UserName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		public string? RedirectUri { get; set; }

		public LoginInfoSettings Settings { get; set; }
		public SignInResult LogInStatus { get; set; }
		public bool IsLoggedIn { get; set; }
	}

	//==================================================

	public class ForgotPasswordInfo : FormModelBase {

		public ForgotPasswordInfo()
			: base() {
			ReconstructSettings();
		}

		public static string Key {
			get {
				return "CMS_forgotform";
			}
		}

		public void ReconstructSettings() {
			base.GetSettings(typeof(ForgotPasswordInfoSettings));
			this.Settings = null;

			if (this.ValidateSettings != null && this.ValidateSettings is ForgotPasswordInfoSettings) {
				this.Settings = this.ValidateSettings as ForgotPasswordInfoSettings;
			}
		}

		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		[StringLength(128)]
		public string Email { get; set; }

		public ForgotPasswordInfoSettings Settings { get; set; }
	}

	//==================================================

	public class ResetPasswordInfo : FormModelBase {

		public ResetPasswordInfo()
			: base() {
			this.CreationResult = null;

			ReconstructSettings();
		}

		public static string Key {
			get {
				return "CMS_resetform";
			}
		}

		public void ReconstructSettings() {
			base.GetSettings(typeof(ResetPasswordInfoSettings));
			this.Settings = null;

			if (this.ValidateSettings != null && this.ValidateSettings is ResetPasswordInfoSettings) {
				this.Settings = this.ValidateSettings as ResetPasswordInfoSettings;
			}
		}

		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		[StringLength(128)]
		public string Email { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public ResetPasswordInfoSettings Settings { get; set; }

		public IdentityResult CreationResult { get; set; }
	}

	//==================================================

	public class ChangePasswordInfo : FormModelBase {

		public ChangePasswordInfo()
			: base() {
			ReconstructSettings();
		}

		public static string Key {
			get {
				return "CMS_chngpassform";
			}
		}

		public void ReconstructSettings() {
			base.GetSettings(typeof(ChangePasswordInfoSettings));
			this.Settings = null;

			if (this.ValidateSettings != null && this.ValidateSettings is ChangePasswordInfoSettings) {
				this.Settings = this.ValidateSettings as ChangePasswordInfoSettings;
			}
		}

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Current password")]
		public string OldPassword { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public ChangePasswordInfoSettings Settings { get; set; }
	}

	//==================================================

	public class ChangeProfileInfo : FormModelBase {

		public ChangeProfileInfo()
			: base() {
			ReconstructSettings();
		}

		public static string Key {
			get {
				return "CMS_chngproform";
			}
		}

		public void ReconstructSettings() {
			base.GetSettings(typeof(ChangeProfileInfoSettings));
			this.Settings = null;

			if (this.ValidateSettings != null && this.ValidateSettings is ChangeProfileInfoSettings) {
				this.Settings = this.ValidateSettings as ChangeProfileInfoSettings;
			}
		}

		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		[StringLength(128)]
		public string Email { get; set; }

		[Display(Name = "User nickname")]
		[StringLength(64)]
		public string UserNickName { get; set; }

		[Display(Name = "First name")]
		[StringLength(64)]
		public string FirstName { get; set; }

		[Display(Name = "Last name")]
		[StringLength(64)]
		public string LastName { get; set; }

		public ChangeProfileInfoSettings Settings { get; set; }
	}
}