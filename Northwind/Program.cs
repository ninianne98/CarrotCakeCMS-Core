using Carrotware.CMS.Interface;
using Carrotware.CMS.Interface.Controllers;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Routing;
using Northwind;
using Northwind.Data;

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
//var config = builder.Configuration;

var buildCfg = new ConfigurationBuilder()
			.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

var config = buildCfg.Build();

var widget = new NorthRegistration();

services.AddControllersWithViews();
services.AddRazorPages();
services.AddMvc().AddControllersAsServices();

services.AddResponseCaching();

var accessor = new HttpContextAccessor();

services.AddSingleton<IHttpContextAccessor>(accessor);
services.AddHttpContextAccessor();
services.AddSingleton(environment);
services.AddSingleton(config);

services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();

services.Configure<RazorViewEngineOptions>(options => {
	options.ViewLocationExpanders.Add(new CmsViewExpander());
});

CarrotWebHelper.Configure(config, environment, services);
CarrotHttpHelper.Configure(config, environment, services);

widget.LoadWidgets(services);

services.AddTransient<ICarrotSite, SiteTestInfo>();
services.AddTransient<IControllerActivator, CmsTestActivator>();

BaseWidgetController.WidgetStandaloneMode = true;

var app = builder.Build();

app.UseResponseCaching();

app.MigrateDatabase();

// app.UseStatusCodePages();
app.UseDeveloperExceptionPage();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
	app.UseMigrationsEndPoint();
} else {
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

widget.RegisterWidgets(app);

app.MapControllerRoute(
	name: "StdRoutes",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.CarrotWebRouteSetup();

app.MapRazorPages();

app.Run();