using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Concurrent;
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

	public class RenderWidgetData {

		// this is needed because the version of the controller in services has some key properties as null
		public RenderWidgetData() {
			this.Controller = null;
			_helper = null;

			this.RouteValues = new RouteValueDictionary();
			_routeData = new RouteData(this.RouteValues);
			_context = CarrotHttpHelper.HttpContext;
		}

		public RenderWidgetData(Controller controller) : this() {
			this.Controller = controller;
			_context = controller.HttpContext;
		}

		public RenderWidgetData(Controller controller, IHtmlHelper helper) : this() {
			this.Controller = controller;
			_helper = helper;
			_context = _helper.ViewContext.HttpContext;

			foreach (var r in helper.ViewContext.RouteData.Values) {
				this.RouteValues.Add(r.Key, r.Value);
			}
		}

		private HttpContext _context;
		private IHtmlHelper _helper;
		private RouteData _routeData;
		private PartialViewResult _partialViewResult;

		public Controller Controller { get; set; }
		public RouteData RouteData { get { return _routeData; } }
		public RouteValueDictionary RouteValues { get; set; }

		public PartialViewResult PartialView { get { return _partialViewResult; } }
		public HttpContext HttpContext { get { return _context; } }

		public void InitController() {
			var act = this.GetActionContext();

			var context = new ActionExecutingContext(act, new List<IFilterMetadata>(),
						new Dictionary<string, object>(), this.Controller);

			this.Controller.OnActionExecuting(context);
		}

		public void CapturePartialResult(PartialViewResult partial) {
			_partialViewResult = partial;
		}

		public ActionContext GetActionContext() {
			if (_helper == null) {
				_routeData = new RouteData(this.RouteValues);
				return new ActionContext(this.HttpContext, this.RouteData, new ActionDescriptor());
			} else {
				return new ActionContext(_helper.ViewContext.HttpContext, _helper.ViewContext.RouteData, new ActionDescriptor());
			}
		}

		public ViewContext GetViewContext(TextWriter stream, IView view, object? model) {
			var actionContext = this.GetActionContext();
			var dataProvider = this.HttpContext.RequestServices.GetService(typeof(ITempDataProvider)) as ITempDataProvider;

			var tempData = new TempDataDictionary(this.HttpContext, dataProvider);
			var viewData = new ViewDataDictionary<object>(new EmptyModelMetadataProvider(), new ModelStateDictionary());

			if (this.PartialView != null) {
				// get the TempData from the widget controller
				if (this.PartialView.TempData != null) {
					tempData.Clear();
					foreach (var v in this.PartialView.TempData) {
						tempData.Add(v.Key, v.Value);
					}
				}
				// get the ViewData/ViewBag from the widget controller
				if (this.PartialView.ViewData != null) {
					viewData.Clear();
					foreach (var v in this.PartialView.ViewData) {
						viewData.Add(v.Key, v.Value);
					}
				}
			}

			//// get the ViewData/ViewBag from the main page, ex template stuff
			//if (_helper != null && _helper.ViewData != null) {
			//	foreach (var v in _helper.ViewData) {
			//		// don't overwrite any values found from the widget
			//		if (!viewData.ContainsKey(v.Key)) {
			//			viewData.Add(v.Key, v.Value);
			//		}
			//	}
			//}

			if (model != null) {
				viewData.Model = model;
			}

			return new ViewContext(actionContext, view, viewData, tempData, stream, new HtmlHelperOptions());
		}
	}

	//===================================

	public static class RenderWidgetHelper {
		private static ConcurrentDictionary<string, Type> _discoveredTypes = new ConcurrentDictionary<string, Type>();

		public static RenderWidgetData CreateController(this Controller source, Type type, string actionName, string areaName, object widgetPayload) {
			return CreateController(type, source, actionName, areaName, widgetPayload);
		}

		public static RenderWidgetData CreateController(this Controller source, string typeName, string actionName, string areaName, object widgetPayload) {
			return CreateController(typeName, source, actionName, areaName, widgetPayload);
		}

		public static HtmlString RenderActionToString(this Controller source, string typeName, string actionName, string areaName, object widgetPayload) {
			return RenderActionToString(typeName, source, actionName, areaName, widgetPayload);
		}

		public static HtmlString RenderActionToString(this Controller source, Type type, string actionName, string areaName, object widgetPayload) {
			return RenderActionToString(type, source, actionName, areaName, widgetPayload);
		}

		public static string ResultToString(this PartialViewResult partialResult, RenderWidgetData data, string viewName = null) {
			return ResultToString(data, partialResult, viewName);
		}

		public static RenderWidgetData CreateController(string typeName, Controller source, string actionName, string areaName, object widgetPayload) {
			Type type = Type.GetType(typeName);

			if (_discoveredTypes.ContainsKey(typeName) & type == null) {
				type = _discoveredTypes[typeName];
			}

			if (type == null) {
				var parts = typeName.Split(',');
				if (parts.Length == 2) {
					Assembly asmb = GetAssembly(typeName);
					type = GetType(typeName, asmb);
					string assemblyName = asmb.GetAssemblyName();

					if (type.GetInterfaces().Contains(typeof(IWidgetController)) && string.IsNullOrEmpty(areaName)) {
						areaName = assemblyName;
					}
				}
			}

			if (!_discoveredTypes.ContainsKey(typeName) && type != null) {
				_discoveredTypes.TryAdd(typeName, type);
			}

			return CreateController(type, source, actionName, areaName, widgetPayload);
		}

		public static RenderWidgetData CreateController(Type type, Controller source, string actionName, string areaName, object widgetPayload) {
			var data = new RenderWidgetData();
			Controller controller = null;

			var svc = source.HttpContext.RequestServices.GetService(type);

			if (svc == null) {
				svc = CarrotHttpHelper.ServiceProvider.GetService(type);
			}

			//fallback in case it isn't in the services
			//if (svc == null) {
			//	var constructor = type.GetConstructors().Single();
			//	svc = constructor.Invoke(
			//			constructor.GetParameters()
			//				.Select(parameter => CarrotHttpHelper.HttpContext
			//								.RequestServices.GetService(parameter.ParameterType))
			//				.ToArray()
			//			);
			//}

			if (svc != null && svc is Controller) {
				controller = (Controller)svc;
			} else {
				throw new Exception($"The type {type} was not a controller or did not exist.");
			}

			controller.ControllerContext = source.ControllerContext;

			data = new RenderWidgetData(controller);

			string controlerName = controller.GetType().Name.ToLowerInvariant().Replace("controller", string.Empty);

			var routeData = data.RouteData.Values;

			if (controller is IWidgetController && string.IsNullOrEmpty(areaName)) {
				areaName = ((IWidgetController)controller).AreaName;
			}

			routeData["area"] = string.Empty;
			if (!string.IsNullOrWhiteSpace(areaName)) {
				routeData["area"] = areaName;
			}

			routeData["action"] = actionName;
			routeData["controller"] = controlerName;

			foreach (var r in source.RouteData.Values.Where(x => x.Key.ToLowerInvariant() != "controller"
					&& x.Key.ToLowerInvariant() != "action" && x.Key.ToLowerInvariant() != "area")) {
				routeData[r.Key] = r.Value;
			}

			if (controller is IWidgetDataObject) {
				((IWidgetDataObject)controller).WidgetPayload = widgetPayload;
			}

			data.InitController();

			return data;
		}

		public static PartialViewResult ExecuteAction(RenderWidgetData data) {
			Controller controller = data.Controller;
			var type = controller.GetType();
			var routeData = data.RouteData;
			var actionName = routeData.Values["action"].ToString();

			MethodInfo? methodInfo = null;
			List<MethodInfo> mthds = type.GetMethods().Where(x => x.Name == actionName).ToList();

			// because there might be an overload, get the GET version if there is more than one
			if (mthds.Count <= 1) {
				methodInfo = mthds.FirstOrDefault();
			} else {
				methodInfo = mthds.Where(x => x.GetCustomAttributes(typeof(HttpGetAttribute), true).Any()).FirstOrDefault();
			}

			PartialViewResult partial = null;

			if (methodInfo != null) {
				object result = null;
				ParameterInfo[] parameters = methodInfo.GetParameters();

				if (parameters.Length == 0) {
					result = methodInfo.Invoke(controller, null);
				} else {
					List<object> parametersArray = new List<object>();

					foreach (ParameterInfo parm in parameters) {
						object val = null;

						if (routeData.Values[parm.Name] != null) {
							val = routeData.Values[parm.Name];
						}
						if (val == null && CarrotHttpHelper.QueryString(parm.Name) != null) {
							val = CarrotHttpHelper.QueryString(parm.Name);
						}

						if (val != null) {
							object o = null;
							Type tp = parm.ParameterType;
							tp = Nullable.GetUnderlyingType(tp) ?? tp;

							if (tp == typeof(Guid)) {
								o = new Guid(val.ToString());
							} else {
								o = Convert.ChangeType(val, tp);
							}

							parametersArray.Add(o);
						} else {
							parametersArray.Add(null);
						}
					}

					result = methodInfo.Invoke(controller, parametersArray.ToArray());
				}

				partial = (PartialViewResult)result;
				data.CapturePartialResult(partial);
			}

			return partial;
		}

		public static string ResultToString(RenderWidgetData data, PartialViewResult partialResult, string viewName = null) {
			Controller controller = data.Controller;
			string stringResult = null;

			var engine = data.HttpContext.RequestServices.GetRequiredService(typeof(IRazorViewEngine)) as IRazorViewEngine;

			if (string.IsNullOrEmpty(viewName)) {
				viewName = data.RouteData.Values["action"].ToString();
			}

			if (partialResult != null) {
				data.CapturePartialResult(partialResult);

				var model = partialResult.Model;

				if (model != null) {
					controller.ViewData.Model = model;
				}

				var context = data.GetActionContext();

				if (engine != null) {
					var actualViewName = (string.IsNullOrWhiteSpace(partialResult.ViewName) ? viewName : partialResult.ViewName) ?? string.Empty;
					var viewEngineResult = engine.FindView(context, actualViewName, false);
					var view = viewEngineResult.View;

					if (view != null) {
						using (var sw = new StringWriter()) {
							var ctx = data.GetViewContext(sw, view, model);
							var task = view.RenderAsync(ctx);
							stringResult = sw.ToString();
						}
					} else {
						throw new Exception($"View '{actualViewName}' is null");
					}
				} else {
					throw new Exception("IRazorViewEngine is null");
				}
			}

			return stringResult;
		}

		internal static Assembly GetAssembly(string typeName) {
			var parts = typeName.Split(',');
			var currentAssembly = Assembly.GetExecutingAssembly();
			var fldr = AppDomain.CurrentDomain.BaseDirectory ?? AppDomain.CurrentDomain.RelativeSearchPath;

			if (parts.Length == 2) {
				var files = Directory.GetFiles(fldr, $"{parts[1].Trim()}.dll", SearchOption.AllDirectories);
				var assemblyToLoad = files.First();

				var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyToLoad);

				return assembly;
			}

			return currentAssembly;
		}

		internal static Type GetType(string typeName, Assembly assembly) {
			var parts = typeName.Split(',');
			if (parts.Length == 2) {
				return assembly.GetType(parts[0].Trim());
			}

			return typeof(RenderWidgetHelper);
		}

		public static HtmlString RenderActionToString(string typeName, Controller source, string actionName, string areaName, object widgetPayload) {
			Type type = Type.GetType(typeName);

			if (_discoveredTypes.ContainsKey(typeName) & type == null) {
				type = _discoveredTypes[typeName];
			}

			if (type == null) {
				var parts = typeName.Split(',');
				if (parts.Length == 2) {
					Assembly asmb = GetAssembly(typeName);
					type = GetType(typeName, asmb);
					string assemblyName = asmb.GetAssemblyName();

					if (type.GetInterfaces().Contains(typeof(IWidgetController)) && string.IsNullOrEmpty(areaName)) {
						areaName = assemblyName;
					}
				}
			}

			if (!_discoveredTypes.ContainsKey(typeName) && type != null) {
				_discoveredTypes.TryAdd(typeName, type);
			}

			return RenderActionToString(type, source, actionName, areaName, widgetPayload);
		}

		public static HtmlString RenderActionToString(Type type, Controller source, string actionName, string areaName, object widgetPayload) {
			var data = CreateController(type, source, actionName, areaName, widgetPayload);

			var resultExec = ExecuteAction(data);

			var resultString = ResultToString(data, resultExec);

			return new HtmlString(resultString);
		}
	}
}