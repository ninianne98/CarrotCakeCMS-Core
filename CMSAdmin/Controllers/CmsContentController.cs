using Carrotware.CMS.Core;
using Carrotware.CMS.Interface;
using Carrotware.CMS.Interface.Controllers;
using Carrotware.CMS.Security;
using Carrotware.CMS.UI.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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

namespace Carrotware.CMS.CoreMVC.UI.Admin.Controllers {

	public class CmsContentController : Controller, IContentController {
		protected ManageSecurity securityHelper = new ManageSecurity();
		protected CMSConfigHelper cmsHelper = new CMSConfigHelper();
		private PagePayload _page = null;
		private readonly ILogger _logger;

		public CmsContentController(ILogger<CmsContentController> logger)
			: base() {

			_logger = logger;
			this.TemplateFile = string.Empty;
			this.WidgetCount = 0;

			BaseWidgetController.WidgetStandaloneMode = false;
		}

		public override void OnActionExecuting(ActionExecutingContext context) {
			base.OnActionExecuting(context);

			if (this.RouteData != null) {
				var routes = this.RouteData.Values;
			}
		}

		public string TemplateFile { get; set; }

		private int _widgetCount = 0;

		public int WidgetCount {
			get {
				return _widgetCount++;
			}
			set {
				_widgetCount = value;
			}
		}

		[HttpGet]
		public PartialViewResult ViewOnly() {
			return PartialView();
		}

		[HttpGet]
		public ActionResult Default() {

			try {
				return DefaultView();
			} catch (Exception ex) {
				//assumption is database is probably empty / needs updating, so trigger the under construction view
				SiteData.WriteDebugException("cmscontentcontroller_defaultview", ex);
				_logger.LogWarning(ex, "cmscontentcontroller_defaultview");

				return View("_EmptyHome");
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Default(FormCollection model) {
			_page = PayloadHelper.GetContentFromContext();

			object frm = null;

			if (this.HttpContext.Request.Form["form_type"].ToString().Length > 0) {
				string formMode = Request.Form["form_type"].ToString().ToLowerInvariant();

				if (formMode == "searchform") {
					frm = new SiteSearch();
					frm = FormHelper.ParseRequest(frm, Request);
					this.ViewData["CMS_searchform"] = frm;
					if (frm != null) {
						this.TryValidateModel(frm);
					}
				}

				if (formMode == "contactform") {
					frm = new ContactInfo();
					frm = FormHelper.ParseRequest(frm, Request);
					var cmt = (ContactInfo)frm;
					cmt.Root_ContentID = _page.ThePage.Root_ContentID;
					cmt.CreateDate = SiteData.CurrentSite.Now;
					cmt.CommenterIP = this.HttpContext.Connection.RemoteIpAddress.ToString();
					this.ViewData[ContactInfo.Key] = frm;
					if (cmt != null) {
						this.TryValidateModel(cmt);
					}
				}
			}

			return DefaultView();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public PartialViewResult Default2(ContactInfo model, bool contact) {
			return PartialView();
		}

		public ActionResult DefaultView() {
			LoadPage();

			if (_page != null && _page.ThePage.Root_ContentID != Guid.Empty) {
				DateTime dtModified = _page.TheSite.ConvertSiteTimeToLocalServer(_page.ThePage.EditDate);
				string strModifed = dtModified.ToString("r");

				Response.Headers.LastModified = strModifed;

				DateTime dtExpire = DateTime.Now.AddSeconds(30);

				if (User.Identity.IsAuthenticated) {
					dtExpire = DateTime.Now.AddMinutes(-30);
					Response.Headers.Expires = dtExpire.ToString("r");
				} else {
					Response.Headers.Expires = dtExpire.ToString("r");
				}

				SiteData.WriteDebugException("cmscontentcontroller_defaultview _page != null", new Exception(string.Format("Loading: {0} {1} {2}", _page.ThePage.FileName, _page.ThePage.TemplateFile, this.DisplayTemplateFile)));
				_logger.LogInformation("cmscontentcontroller_defaultview _page != null");

				return View(this.DisplayTemplateFile);
			} else {
				string sFileRequested = Request.Path;

				SiteData.WriteDebugException("cmscontentcontroller_defaultview _page == null", new Exception(string.Format("Requesting: {0} {1}", sFileRequested, this.DisplayTemplateFile)));
				_logger.LogInformation("cmscontentcontroller_defaultview _page == null");

				DateTime dtModified = DateTime.Now.Date;
				string strModifed = dtModified.ToString("r");
				Response.Headers.LastModified = strModifed;
				Response.Headers.Expires = dtModified.AddSeconds(30).ToString("r");

				if (SiteData.IsLikelyHomePage(sFileRequested)) {
					SiteData.WriteDebugException("cmscontentcontroller_defaultview", new Exception("Empty _page"));
					_logger.LogInformation("cmscontentcontroller_defaultview");

					return View("_EmptyHome");
				} else {
					Response.StatusCode = (int)System.Net.HttpStatusCode.NotFound;
					SiteData.WriteDebugException("cmscontentcontroller_httpnotfound", new Exception("HttpNotFound"));
					_logger.LogInformation("cmscontentcontroller_httpnotfound");

					return NotFound();
				}
			}
		}

		public ActionResult PageNotFound() {
			//SiteData.Perform404Redirect(Request.Path);

			Response.StatusCode = (int)System.Net.HttpStatusCode.NotFound;
			SiteData.WriteDebugException("cmscontentcontroller_pagenotfound", new Exception(string.Format("HttpNotFound: {0}", Request.Path)));
			_logger.LogInformation("cmscontentcontroller_pagenotfound");

			return NotFound();
		}

		protected void SearchData() {
			if (_page == null || _page.ThePage.ContentID == Guid.Empty) {
				// use a fake search page when needed, but don't allow editing
				if (!SecurityData.AdvancedEditMode && SiteData.IsLikelyFakeSearch()) {
					var search = ContentPageHelper.GetEmptySearch();
					_page = PayloadHelper.GetContent(search);
				}
				//get the real search
				if (SiteData.IsLikelySearch()) {
					_page = PayloadHelper.GetContent(SiteData.CurrentSite.Blog_Root_ContentID.Value);
				}
			}
		}

		protected void LoadPage() {
			_page = PayloadHelper.GetContentFromContext();

			SearchData();

			this.TemplateFile = this.DisplayTemplateFile;
		}

		protected void LoadPage(string uri) {
			_page = PayloadHelper.GetContent(uri);

			SearchData();

			this.TemplateFile = this.DisplayTemplateFile;
		}

		protected string DisplayTemplateFile {
			get {
				var root = CarrotHttpHelper.MapPath("/");
				var template = (_page.ThePage.TemplateFile ?? "").Replace("~/", "/");
				var templatePath = Path.Join(root, template);

				if (_page != null && _page.ThePage != null && !string.IsNullOrEmpty(_page.ThePage.TemplateFile)
						&& System.IO.File.Exists(templatePath)) {
					return Path.Combine(root, template);
				} else {
					return SiteData.DefaultTemplateFilename;
				}
			}
		}

		[HttpGet]
		public ActionResult RSSFeed(string type) {
			return new ContentResult {
				ContentType = SiteData.RssDocType,
				Content = SiteData.CurrentSite.GetRSSFeed(type)
			};
		}

		[HttpGet]
		public ActionResult SiteMap() {
			return new ContentResult {
				ContentType = SiteData.RssDocType,
				Content = SiteMapHelper.GetSiteMap()
			};
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public PartialViewResult Contact(ContactInfo model) {
			model.ReconstructSettings();
			this.ViewData[ContactInfo.Key] = model;
			model.IsSaved = false;

			LoadPage(model.Settings.Uri);

			var settings = model.Settings;

			if (settings.UseValidateHuman) {
				bool IsValidated = model.ValidateHuman.ValidateValue(model.ValidationValue);
				if (!IsValidated) {
					ModelState.AddModelError("ValidationValue", model.ValidateHuman.AltValidationFailText);
					model.ValidationValue = string.Empty;
				}
			}

			//TODO: log the comment and B64 encode some of the settings (TBD)
			if (ModelState.IsValid) {
				string sIP = this.HttpContext.Connection.RemoteIpAddress.ToString();

				PostComment pc = new PostComment();
				pc.ContentCommentID = Guid.NewGuid();
				pc.Root_ContentID = _page.ThePage.Root_ContentID;
				pc.CreateDate = SiteData.CurrentSite.Now;
				pc.IsApproved = false;
				pc.IsSpam = false;
				pc.CommenterIP = sIP;
				pc.CommenterName = HttpUtility.HtmlEncode(model.CommenterName);
				pc.CommenterEmail = HttpUtility.HtmlEncode(model.CommenterEmail ?? string.Empty);
				pc.PostCommentText = HttpUtility.HtmlEncode(model.PostCommentText); //.Replace("<", "&lt;").Replace(">", "&gt;");
				pc.CommenterURL = HttpUtility.HtmlEncode(model.CommenterURL ?? string.Empty);

				pc.Save();

				model.IsSaved = true;

				model.CommenterName = string.Empty;
				model.CommenterEmail = string.Empty;
				model.PostCommentText = string.Empty;
				model.CommenterURL = string.Empty;
				model.ValidationValue = string.Empty;

				this.ViewData[ContactInfo.Key] = model;
				model.SendMail(pc, _page.ThePage);

				ModelState.Clear();
			}

			return PartialView(settings.PostPartialName);
		}

		//====================================
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);

			if (cmsHelper != null) {
				cmsHelper.Dispose();
			}
		}

		protected void AddErrors(IdentityResult result) {
			Helper.AddErrors(ModelState, result);
		}

		//====================================

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordInfo model) {
			model.ReconstructSettings();
			this.ViewData[ForgotPasswordInfo.Key] = model;

			LoadPage(model.Settings.Uri);

			var settings = model.Settings;

			if (settings.UseValidateHuman) {
				bool IsValidated = model.ValidateHuman.ValidateValue(model.ValidationValue);
				if (!IsValidated) {
					ModelState.AddModelError("ValidationValue", model.ValidateHuman.AltValidationFailText);
					model.ValidationValue = string.Empty;
				}
			}

			string confirmView = settings.PostPartialName;
			if (!string.IsNullOrEmpty(settings.PostPartialName)) {
				confirmView = settings.PostPartialConfirmation;
			}

			string confirmUri = settings.Uri;
			if (!string.IsNullOrEmpty(settings.ConfirmUri)) {
				confirmUri = settings.ConfirmUri;
			}

			if (ModelState.IsValid) {
				var user = await securityHelper.UserManager.FindByEmailAsync(model.Email);
				if (user != null) {
					SecurityData sd = new SecurityData();
					sd.ResetPassword(confirmUri, model.Email);
				}

				return PartialView(confirmView, model);
			}

			Helper.HandleErrorDict(ModelState);

			return PartialView(settings.PostPartialName, model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(ResetPasswordInfo model) {
			model.ReconstructSettings();
			this.ViewData[ResetPasswordInfo.Key] = model;

			LoadPage(model.Settings.Uri);

			var settings = model.Settings;

			if (settings.UseValidateHuman) {
				bool IsValidated = model.ValidateHuman.ValidateValue(model.ValidationValue);
				if (!IsValidated) {
					ModelState.AddModelError("ValidationValue", model.ValidateHuman.AltValidationFailText);
					model.ValidationValue = string.Empty;
				}
			}

			if (string.IsNullOrEmpty(settings.UserCode)) {
				ModelState.AddModelError(string.Empty, "Reset code not provided.");
			}

			if (ModelState.IsValid) {
				string confirmView = settings.PostPartialName;
				if (!string.IsNullOrEmpty(settings.PostPartialName)) {
					confirmView = settings.PostPartialConfirmation;
				}

				var user = await securityHelper.UserManager.FindByEmailAsync(model.Email);
				if (user == null) {
					return PartialView(confirmView, model);
				} else {
					SecurityData sd = new SecurityData();
					var result = await sd.ResetPassword(user, settings.UserCode, model.Password);
					model.CreationResult = result;

					if (result.Succeeded) {
						return PartialView(confirmView, model);
					}

					AddErrors(result);
				}
			}

			Helper.HandleErrorDict(ModelState);

			return PartialView(settings.PostPartialName, model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangePassword(ChangePasswordInfo model) {
			model.ReconstructSettings();
			this.ViewData[ChangePasswordInfo.Key] = model;

			LoadPage(model.Settings.Uri);

			var settings = model.Settings;
			if (!SecurityData.IsAuthenticated) {
				ModelState.AddModelError("", "User is not authenticated");
			}

			if (settings.UseValidateHuman) {
				bool IsValidated = model.ValidateHuman.ValidateValue(model.ValidationValue);
				if (!IsValidated) {
					ModelState.AddModelError("ValidationValue", model.ValidateHuman.AltValidationFailText);
					model.ValidationValue = string.Empty;
				}
			}

			if (ModelState.IsValid && SecurityData.IsAuthenticated) {
				string successView = settings.PostPartialName;
				if (!string.IsNullOrEmpty(settings.PostPartialName)) {
					successView = settings.PostPartialSuccess;
				}

				var user = SecurityData.CurrentIdentityUser;

				if (user != null) {
					var result = await securityHelper.UserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

					if (result.Succeeded) {
						await securityHelper.SignInManager.SignInAsync(user, false);
						return PartialView(successView, model);
					}
					AddErrors(result);
				} else {
					// unusual as this is the signed in user, but whatever
					ModelState.AddModelError("ValidationValue", "User does not exist.");
				}
			}

			Helper.HandleErrorDict(ModelState);

			return PartialView(settings.PostPartialName, model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangeProfile(ChangeProfileInfo model) {
			model.ReconstructSettings();
			this.ViewData[ChangeProfileInfo.Key] = model;

			LoadPage(model.Settings.Uri);

			var settings = model.Settings;

			if (settings.UseValidateHuman) {
				bool IsValidated = model.ValidateHuman.ValidateValue(model.ValidationValue);
				if (!IsValidated) {
					ModelState.AddModelError("ValidationValue", model.ValidateHuman.AltValidationFailText);
					model.ValidationValue = string.Empty;
				}
			}

			if (ModelState.IsValid && SecurityData.IsAuthenticated) {
				string successView = settings.PostPartialName;
				if (!string.IsNullOrEmpty(settings.PostPartialName)) {
					successView = settings.PostPartialSuccess;
				}

				ExtendedUserData exUsr = SecurityData.CurrentExUser;
				var user = SecurityData.CurrentIdentityUser;

				IdentityResult result = await securityHelper.UserManager.SetEmailAsync(user, model.Email);

				exUsr.UserNickName = model.UserNickName;
				exUsr.FirstName = model.FirstName;
				exUsr.LastName = model.LastName;

				exUsr.Save();

				if (result.Succeeded) {
					return PartialView(successView, model);
				}
			}

			Helper.HandleErrorDict(ModelState);

			return PartialView(settings.PostPartialName, model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Logout(LogoutInfo model) {
			model.ReconstructSettings();
			this.ViewData[LogoutInfo.Key] = model;
			LoadPage(model.Settings.Uri);

			if (ModelState.IsValid) {
				ModelState.Clear();
			}

			await securityHelper.SignInManager.SignOutAsync();

			return PartialView(model.Settings.PostPartialName);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginInfo model) {
			bool rememberme = false;

			model.ReconstructSettings();
			this.ViewData[LoginInfo.Key] = model;

			LoadPage(model.Settings.Uri);

			var settings = model.Settings;

			string partialName = settings.PostPartialName;

			if (settings.UseValidateHuman) {
				bool IsValidated = model.ValidateHuman.ValidateValue(model.ValidationValue);
				if (!IsValidated) {
					ModelState.AddModelError("ValidationValue", model.ValidateHuman.AltValidationFailText);
					model.ValidationValue = string.Empty;
				}
			}

			if (ModelState.IsValid) {
				ModelState.Clear();

				var user = await securityHelper.UserManager.FindByNameAsync(model.UserName);
				var result = await securityHelper.SignInManager.PasswordSignInAsync(model.UserName, model.Password, rememberme, true);

				model.LogInStatus = result;

				if (result.Succeeded) {
					await securityHelper.UserManager.ResetAccessFailedCountAsync(user);
				} else {
					if (result.IsLockedOut) {
						ModelState.AddModelError(string.Empty, "User locked out.");

						if (!string.IsNullOrEmpty(settings.PostPartialNameLockout)) {
							partialName = settings.PostPartialNameLockout;
						}
					} else {
						ModelState.AddModelError(string.Empty, "Invalid login attempt.");

						if (!string.IsNullOrEmpty(settings.PostPartialNameFailure)) {
							partialName = settings.PostPartialNameFailure;
						}

						if (user.LockoutEnd.HasValue && user.LockoutEnd.Value < DateTime.UtcNow) {
							user.LockoutEnd = null;
							user.AccessFailedCount = 1;
							await securityHelper.UserManager.UpdateAsync(user);
						}
					}
				}
			}

			return PartialView(partialName, model);
		}
	}
}