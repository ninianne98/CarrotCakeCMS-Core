using CarrotCake.CMS.Plugins.FAQ2;
using Carrotware.CMS.Core;
using Carrotware.CMS.Interface;
using Carrotware.CMS.Interface.Controllers;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Routing;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2024, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: April 2024
*/

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var environment = builder.Environment;

var config = builder.CreateConfig();

services.AddHttpContextAccessor();
services.AddSingleton(environment);
services.AddSingleton(config);

var widget = new Faq2Registration();

services.AddControllersWithViews();
services.AddRazorPages();
services.AddMvc().AddControllersAsServices();

services.AddResponseCaching();

services.AddHttpContextAccessor();
services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();

services.Configure<RazorViewEngineOptions>(options => {
	options.ViewLocationExpanders.Add(new CmsViewExpander());
});

builder.ConfigureCarrotWeb(config);
builder.ConfigureCarrotHttpHelper(config);

widget.LoadWidgets(services);

services.AddTransient<ICarrotSite, SiteBasicInfo>();
services.AddTransient<IControllerActivator, CmsTestActivator>();

BaseWidgetController.WidgetStandaloneMode = true;

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

app.MapControllerRoute(
	name: "StdRoutes",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.CarrotWebRouteSetup();

app.MapRazorPages();

app.Run();