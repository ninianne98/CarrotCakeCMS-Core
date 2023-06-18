using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

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

	public static class WidgetLoad {
		private static ConcurrentBag<Assembly> _assemblies = new ConcurrentBag<Assembly>();
		private static ConcurrentBag<Type> _types = new ConcurrentBag<Type>();

		private static string[] GetFiles() {
			var fldr = AppDomain.CurrentDomain.BaseDirectory ?? AppDomain.CurrentDomain.RelativeSearchPath;

			var files = Directory.GetFiles(fldr, "*.dll", SearchOption.AllDirectories).ToList();

			files.RemoveAll(x => x.Contains("Microsoft.AspNetCore"));
			files.RemoveAll(x => x.Contains("Microsoft.CodeAnalysis"));
			files.RemoveAll(x => x.Contains("Microsoft.EntityFrameworkCore"));
			files.RemoveAll(x => x.Contains("Microsoft.VisualStudio."));

			return files.ToArray();
		}

		internal static void DiscoverWidgets() {
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
										|| t.GetInterface(nameof(IAdminModule)) != null).ToList();

					if (widgets.Any()) {
						foreach (var t in widgets) {
							if (!t.Namespace.ToLowerInvariant().StartsWith(nsp)
										&& !t.GetTypeInfo().IsAbstract
										&& !t.GetTypeInfo().IsInterface
										&& !t.Name.ToLowerInvariant().Contains("anonymoustype")) {
								_types.Add(t);
							}
						}
					}
				} catch (Exception ex) { }
			}
		}

		public static void RegisterWidgets(this WebApplication app) {
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
						options.FileProviders.Add(new EmbeddedFileProvider(part.Assembly));
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