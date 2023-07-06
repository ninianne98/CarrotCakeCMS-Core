using CarrotCake.CMS.Plugins.LoremIpsum.Code;
using Carrotware.CMS.Interface;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Routing;

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

var widget = new LoremRegistration();

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

var app = builder.Build();

app.UseResponseCaching();

// app.UseStatusCodePages();
app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

widget.RegisterWidgets(app);

app.MapFallback(context => {
	context.Response.Redirect(CmsTestHomeAttribute.DefaultPage.FixPathSlashes());
	return Task.CompletedTask;
});

app.MapControllerRoute(
	name: "StdRoutes",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.CarrotWebRouteSetup();

app.MapRazorPages();

app.Run();