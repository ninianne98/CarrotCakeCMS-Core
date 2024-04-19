using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.FileProviders;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Loader;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, 2024 Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023, April 2024
*/

namespace Carrotware.CMS.Interface {

	public static class WidgetLoad {
		private static ConcurrentBag<Assembly> _assemblies = new ConcurrentBag<Assembly>();
		private static ConcurrentBag<Type> _types = new ConcurrentBag<Type>();
		private static WebApplication _webApplication;
		private static IServiceCollection _servicesn;

		private static string[] GetFiles() {
			string fldr = AppDomain.CurrentDomain.BaseDirectory ?? AppDomain.CurrentDomain.RelativeSearchPath ?? string.Empty;

			var files = Directory.GetFiles(fldr, "*.dll", SearchOption.AllDirectories).ToList().Select(x => new FileInfo(x)).ToList();

			files.RemoveAll(x => x.Name.StartsWith("Microsoft."));
			files.RemoveAll(x => x.Name.StartsWith("System."));

			return files.Select(x => x.FullName).ToArray();
		}

		internal static void DiscoverWidgets() {
			if (_assemblies.Any()) {
				// in case it is called again, start over fresh
				_assemblies = new ConcurrentBag<Assembly>();
				_types = new ConcurrentBag<Type>();
			}

			string[] files = GetFiles();
			string nsp = typeof(WidgetLoad).Namespace.ToLowerInvariant();

			foreach (string file in files) {
				try {
					var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
					var types = assembly.GetTypes();

					// find any common widget types & this must be a widget
					var widget = types.Where(t => t.GetInterface(nameof(IWidgetController)) != null
										|| t.GetInterface(nameof(IWidget)) != null
										|| t.GetInterface(nameof(IWidgetSettings)) != null
										|| t.GetInterface(nameof(IWidgetLoader)) != null
										|| t.GetInterface(nameof(IWidgetDataObject)) != null
										|| t.GetInterface(nameof(IWidgetRawData)) != null
										|| t.GetInterface(nameof(IAdminModule)) != null).FirstOrDefault();

					if (widget != null && !widget.FullName.ToLowerInvariant().Contains(nsp)) {
						_assemblies.Add(assembly);
					}
				} catch (Exception ex) { }
			}

			foreach (var assembly in _assemblies) {
				try {
					var types = assembly.GetTypes();
					var widgets = types.Where(t => t.GetInterface(nameof(IWidgetController)) != null
										|| t.GetInterface(nameof(IWidget)) != null
										|| t.GetInterface(nameof(IWidgetSettings)) != null
										|| t.GetInterface(nameof(IWidgetLoader)) != null
										|| t.GetInterface(nameof(IWidgetDataObject)) != null
										|| t.GetInterface(nameof(IWidgetRawData)) != null
										|| t.GetInterface(nameof(IAdminModule)) != null).ToList();

					if (widgets.Any()) {
						foreach (var t in widgets) {
							if (!t.Namespace.ToLowerInvariant().StartsWith(nsp)
										&& t.IsClass
										&& !t.IsAbstract
										&& !t.Name.ToLowerInvariant().Contains("anonymoustype")) {
								_types.Add(t);
							}
						}
					}
				} catch (Exception ex) { }
			}
		}

		public static void ReregisterWidgets() {
			DiscoverWidgets();
			_webApplication.RegisterWidgets();
		}

		public static void ReloadWidgets() {
			_servicesn.LoadWidgets();
		}

		public static void RegisterWidgets(this WebApplication app) {
			_webApplication = app;

			string nsp = typeof(WidgetLoad).Namespace.ToLowerInvariant();

			foreach (var assembly in _assemblies) {
				try {
					var types = assembly.GetTypes();
					var widget = types.Where(t => t.GetInterface(nameof(IWidgetLoader)) != null).FirstOrDefault();

					if (widget != null && !widget.FullName.ToLowerInvariant().Contains(nsp)) {
						IWidgetLoader instance = (IWidgetLoader)Activator.CreateInstance(widget);
						instance.RegisterWidgets(app);
					}
				} catch (Exception ex) { }
			}
		}

		public static void LoadWidgets(this IServiceCollection services) {
			_servicesn = services;

			string nsp = typeof(WidgetLoad).Namespace.ToLowerInvariant();
			DiscoverWidgets();

			services.Configure<RazorViewEngineOptions>(options => {
				options.ViewLocationExpanders.Add(new CmsViewExpander());
			});

			foreach (var t in _types) {
				services.AddTransient(t);
			}

			foreach (var assembly in _assemblies) {
				try {
					var types = assembly.GetTypes();
					var part = new AssemblyPart(assembly);

					services.Configure<MvcRazorRuntimeCompilationOptions>(options => {
						options.FileProviders.Add(new EmbeddedFileProvider(assembly));
					});

					services.AddControllers().PartManager.ApplicationParts.Add(part);

					var widget = types.Where(t => t.GetInterface(nameof(IWidgetLoader)) != null).FirstOrDefault();

					if (widget != null && !widget.FullName.ToLowerInvariant().Contains(nsp)) {
						IWidgetLoader instance = (IWidgetLoader)Activator.CreateInstance(widget);
						instance.LoadWidgets(services);
					}
				} catch (Exception ex) { }
			}
		}
	}
}