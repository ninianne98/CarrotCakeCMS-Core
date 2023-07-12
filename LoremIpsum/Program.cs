using CarrotCake.CMS.Plugins.LoremIpsum.Code;
using Carrotware.CMS.Data.Models;
using Carrotware.CMS.Interface;
using Carrotware.CMS.Interface.Controllers;
using Carrotware.CMS.Security;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
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

services.AddDbContext<CarrotCakeContext>(opt => opt.UseSqlServer(config.GetConnectionString(CarrotCakeContext.DBKey)));

// auth  stuff
services.ConfigureCmsAuth(config);

var widget = new LoremRegistration();

services.AddControllersWithViews();
services.AddRazorPages();
services.AddMvc().AddControllersAsServices();

services.AddResponseCaching();

services.AddHttpContextAccessor();
services.AddSingleton(environment);
services.AddSingleton(config);

services.AddHttpContextAccessor();
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
	pattern: "{controller=Admin}/{action=Index}/{id?}");

app.CarrotWebRouteSetup();

app.MapRazorPages();

app.Run();