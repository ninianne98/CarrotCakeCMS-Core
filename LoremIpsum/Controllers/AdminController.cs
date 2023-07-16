using CarrotCake.CMS.Plugins.LoremIpsum.Models;
using Carrotware.CMS.Core;
using Carrotware.CMS.Interface;
using Carrotware.CMS.Interface.Controllers;
using Carrotware.CMS.Security;
using Carrotware.CMS.Security.Models;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace CarrotCake.CMS.Plugins.LoremIpsum.Controllers {

	[WidgetController(typeof(AdminController))]
	public class AdminController : BaseAdminWidgetController {
		private ManageSecurity securityHelper = new ManageSecurity();
		protected readonly ICarrotSite _site;
		private readonly ILogger<AdminController> _logger;

		public AdminController(ILogger<AdminController> logger, ICarrotSite site) {
			_logger = logger;
			_site = site;
		}

		public IActionResult Index() {
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult LogOut() {
			_logger.LogInformation($"Admin : LogOut: {DateTime.UtcNow}");

			securityHelper.SignInManager.SignOutAsync();
			this.HttpContext.Session.Clear();

			return RedirectToAction(this.GetActionName(x => x.Index()));
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login() {
			_logger.LogInformation($"Admin : Login: {DateTime.UtcNow}");

			var model = new UserLogin();
			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(UserLogin model) {
			if (ModelState.IsValid) {
				var user = await securityHelper.UserManager.FindByNameAsync(model.Username);
				var result = await securityHelper.SignInManager.PasswordSignInAsync(model.Username, model.Password, true, true);

				if (result.Succeeded) {
					return RedirectToAction(this.GetActionName(x => x.Index()));
				}
			}

			_logger.LogWarning($"Admin : Login Fail: {model.Username} - {DateTime.UtcNow}");
			ModelState.AddModelError("message", "Invalid login attempt");
			return View(model);
		}

		[HttpGet]
		public IActionResult Pages() {
			ViewBag.Title = "Pages";
			var model = new ContentCreator(ContentPageType.PageType.ContentEntry);

			return View("View", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Pages(ContentCreator model) {
			ViewBag.Title = "Pages";
			model.ContentType = ContentPageType.PageType.ContentEntry;

			if (ModelState.IsValid) {
				model.BuildPages();
			}

			return View("View", model);
		}

		[HttpGet]
		public IActionResult Posts() {
			ViewBag.Title = "Posts";
			var model = new ContentCreator(ContentPageType.PageType.BlogEntry);

			return View("View", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Posts(ContentCreator model) {
			ViewBag.Title = "Posts";
			model.ContentType = ContentPageType.PageType.BlogEntry;

			if (ModelState.IsValid) {
				model.BuildPosts();
			}

			return View("View", model);
		}
	}
}