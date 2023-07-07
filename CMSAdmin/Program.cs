using Carrotware.CMS.Core;
using Carrotware.CMS.CoreMVC.UI.Admin.Controllers;
using Carrotware.CMS.Data;
using Carrotware.CMS.Data.Models;
using Carrotware.CMS.Interface;
using Carrotware.CMS.Interface.Controllers;
using Carrotware.CMS.Security;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
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

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var environment = builder.Environment;

var buildCfg = new ConfigurationBuilder()
				.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables();

var config = buildCfg.Build();

builder.Logging.ClearProviders();

var logConfig = config.GetSection("Logging");
var loggerFactory = LoggerFactory.Create(builder => {
	builder.AddConfiguration(logConfig);
	builder.AddDebug();
	builder.AddSimpleConsole();
});

services.AddDbContext<CarrotCakeContext>(opt => opt.UseSqlServer(config.GetConnectionString("CarrotwareCMS")));

// auth  stuff
services.ConfigureCmsAuth(config);

services.AddControllersWithViews();
services.AddMvc().AddControllersAsServices();
services.AddRazorPages().AddRazorRuntimeCompilation();
services.AddDatabaseDeveloperPageExceptionFilter();
services.AddResponseCaching();

services.AddHttpContextAccessor();
services.AddSingleton(environment);
services.AddSingleton(config);
services.AddSingleton(loggerFactory);

CarrotWebHelper.Configure(config, environment, services);
CarrotHttpHelper.Configure(config, environment, services);

services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();

services.AddTransient<ICarrotSite, SiteBasicInfo>();

services.LoadWidgets();

services.AddScoped(typeof(PagePayload));
services.AddScoped(typeof(CmsRouting));
services.AddTransient<IControllerActivator, CmsActivator>();

BaseWidgetController.WidgetStandaloneMode = false;

var app = builder.Build();

app.UseResponseCaching();

app.MigrateDatabase();

// app.UseStatusCodePages();

app.UseDeveloperExceptionPage();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment()) {
//	app.UseMigrationsEndPoint();
//} else {
//	app.UseExceptionHandler("/Home/Error");
//	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//	app.UseHsts();
//}

string cmsControler = nameof(CmsContentController).Replace("Controller", "");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.Use(async (context, next) => {
	await next();
	//Console.WriteLine($"Found: {context.GetEndpoint()?.DisplayName}");
	if (context.Response.StatusCode == (int)System.Net.HttpStatusCode.NotFound) {
		context.Request.Path = string.Format("/{0}/Get404", cmsControler);
		await next();
	}
});

app.CarrotWebRouteSetup();

app.UseAuthentication();
app.UseAuthorization();

//app.MapControllerRoute(
//	name: "CmsRoutes",
//	pattern: "{*" + CmsConstraint.RouteKey + "}",
//	defaults: new { controller = CmsRouteConstants.CmsController.Content, action = CmsRouteConstants.DefaultAction },
//	constraints: new { key = new CmsConstraint(config) }
//);

app.MapDynamicControllerRoute<CmsRouting>("{*" + CmsRouting.RouteKey + "}");

var cccConfig = CarrotCakeConfig.GetConfig();
var adminFolder = cccConfig.MainConfig.AdminFolderPath.TrimPathSlashes();

app.MapControllerRoute(name: "C3Admin_Route",
	pattern: adminFolder + "/{action=Index}/{id?}",
	defaults: new {
		controller = CmsRouteConstants.CmsController.Admin
	});

app.MapControllerRoute(name: "C3AdminApi_Route",
	pattern: "api/" + adminFolder + "/{action=Index}/{id?}",
	defaults: new {
		controller = CmsRouteConstants.CmsController.AdminApi
	});

//app.MapControllerRoute(name: "C3StdAreas", pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
//app.MapControllerRoute(name: "C3StdAreas", pattern: "{area}/{controller=Home}/{action=Index}/{id?}");
//app.MapControllerRoute(name: "C3StdRoutes", pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.RegisterWidgets();

app.Run();