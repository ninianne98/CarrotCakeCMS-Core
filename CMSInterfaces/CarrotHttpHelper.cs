using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
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
		private static IMemoryCache _memoryCache;

		public static void Configure(IConfigurationRoot configuration, IWebHostEnvironment environment, IServiceCollection services) {
			_configuration = configuration;
			_webHostEnvironment = environment;
			_services = services;

			services.AddMemoryCache();
			services.AddHttpContextAccessor();
			_serviceProvider = services.BuildServiceProvider();

			_httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
			_memoryCache = _serviceProvider.GetRequiredService<IMemoryCache>();

			try {
				_signinmanager = _serviceProvider.GetRequiredService<SignInManager<IdentityUser>>();
				_usermanager = _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
			} catch (Exception ex) { }
		}

		public static IWebHostEnvironment WebHostEnvironment { get { return _webHostEnvironment; } }

		public static IConfigurationRoot Configuration { get { return _configuration; } }

		public static IServiceCollection Services { get { return _services; } }

		public static IServiceProvider ServiceProvider { get { return _serviceProvider; } }

		public static IMemoryCache MemoryCache { get { return _memoryCache; } }

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
			var query = HttpContext.Session;

			if (query.Keys.Any()) {
				return query.GetString(name);
			}

			return null;
		}

		public static string MapPath(string path) {
			var p = path.NormalizeFilename();
			string root = _webHostEnvironment.ContentRootPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

			//var rootpaths = root.Split(Path.AltDirectorySeparatorChar);
			//var newPath = path.Split(Path.AltDirectorySeparatorChar);
			//return string.Join('/', rootpaths.Union(newPath).Where(x => x.Length > 0));
			return Path.Join(root, p);
		}

		public static string HttpReferer {
			get {
				//string referer = Request.Headers["Referer"].ToString();
				var header = Request.GetTypedHeaders();
				return header.Referer != null ? header.Referer.ToString() : string.Empty;
			}
		}

		public static void CacheInsert(string cacheKey, object value, double minutes) {
			var opt = new MemoryCacheEntryOptions();

			if (minutes < 0.5) {
				minutes = 0.5;
			}
			if (minutes > 90) {
				minutes = 90;
			}

			opt.SetPriority(CacheItemPriority.Normal)
				.SetSlidingExpiration(TimeSpan.FromMinutes(minutes))
				.SetAbsoluteExpiration(TimeSpan.FromMinutes(minutes * 10))
				.SetSize(4096);

			_memoryCache.Set(cacheKey, value, opt);
		}

		public static void CacheRemove(string cacheKey) {
			_memoryCache.Remove(cacheKey);
		}

		public static object CacheGet(string cacheKey) {
			object value;
			_memoryCache.TryGetValue(cacheKey, out value);
			return value;
		}

		public static HtmlEncoder HtmlEncoder { get; set; } = HtmlEncoder.Default;
		public static UrlEncoder UrlEncoder { get; set; } = UrlEncoder.Default;
	}
}