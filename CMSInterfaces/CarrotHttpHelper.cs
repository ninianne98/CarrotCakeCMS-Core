using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text.Encodings.Web;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.Interface {

	public static class CarrotHttpHelper {
		private static IHttpContextAccessor _httpContextAccessor;
		private static IWebHostEnvironment _webHostEnvironment;
		private static IConfigurationRoot _configuration;
		private static IServiceCollection _services;
		private static IServiceProvider _serviceProvider;
		private static SignInManager<IdentityUser> _signinmanager;
		private static UserManager<IdentityUser> _usermanager;

		public static void Configure(IConfigurationRoot configuration, IWebHostEnvironment environment, IServiceCollection services) {
			_configuration = configuration;
			_webHostEnvironment = environment;
			_services = services;

			_serviceProvider = services.BuildServiceProvider();

			_httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();

			try {
				_signinmanager = _serviceProvider.GetRequiredService<SignInManager<IdentityUser>>();
				_usermanager = _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
			} catch (Exception ex) { }
		}

		public static IWebHostEnvironment WebHostEnvironment { get { return _webHostEnvironment; } }

		public static IConfigurationRoot Configuration { get { return _configuration; } }

		public static IServiceCollection Services { get { return _services; } }

		public static IServiceProvider ServiceProvider { get { return _serviceProvider; } }

		public static HttpContext Current { get { return HttpContext; } }

		public static HttpRequest Request { get { return HttpContext.Request; } }

		public static HttpResponse Response { get { return HttpContext.Response; } }

		public static HttpContext HttpContext { get { return _httpContextAccessor.HttpContext; } }

		public static IEnumerable<T> GetAll<T>(this IServiceProvider provider) {
			var site = typeof(ServiceProvider).GetProperty("CallSiteFactory", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(provider);
			var desc = site.GetType().GetField("_descriptors", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(site) as ServiceDescriptor[];
			return desc.Select(s => provider.GetRequiredService(s.ServiceType)).OfType<T>();
		}

		public static IEnumerable<object> GetAllServices(this IServiceProvider provider) {
			var site = typeof(ServiceProvider).GetProperty("CallSiteFactory", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(provider);
			var desc = site.GetType().GetField("_descriptors", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(site) as ServiceDescriptor[];
			return desc.Select(s => provider.GetRequiredService(s.ServiceType));
		}

		public static string QueryString(string name) {
			var query = Request.QueryString;

			if (query.HasValue) {
				var dict = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(query.Value);

				if (dict != null) {
					if (dict.ContainsKey(name)) {
						return dict[name];
					}
				}
			}

			return null;
		}

		public static string Session(string name) {
			var query = Current.Session;

			if (query.Keys.Any()) {
				return query.GetString(name);
			}

			return null;
		}


		public static string MapPath(string path) {
			return Path.Combine(_webHostEnvironment.ContentRootPath, path.Replace("~", ""));
		}

		public static void CacheInsert(string keyAdminMenuModules, List<object> modules, object value, DateTime dateTime, object noSlidingExpiration) {
			// TODO: make sliding cache wrapper
		}

		public static void CacheRemove(string keyAdminToolboxModules) {
			// TODO: make sliding cache wrapper
		}

		public static Dictionary<string, object> Cache {
			// TODO: make sliding cache wrapper
			get {
				return new Dictionary<string, object>();
			}
		}

		public static HtmlEncoder HtmlEncoder { get; set; } = HtmlEncoder.Default;
		public static UrlEncoder UrlEncoder { get; set; } = UrlEncoder.Default;
	}
}