using Carrotware.CMS.Core;
using Carrotware.CMS.Interface;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Reflection;
using System.Security.Principal;
using System.Text;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.UI.Components {

	public static class CarrotCakeCmsHelper {

		public static CarrotCakeHtmlWeb CarrotCakeHtml(this HtmlHelper<dynamic> htmlHelper) {
			return new CarrotCakeHtmlWeb(htmlHelper);
		}

		public static CarrotCakeHtmlWeb CarrotCakeHtml(this IHtmlHelper htmlHelper) {
			return new CarrotCakeHtmlWeb(htmlHelper);
		}

		public static string SiteMapUri {
			get { return SiteFilename.SiteMapUri; }
		}

		public static string RssUri {
			get { return SiteFilename.RssFeedUri; }
		}

		public static string AdminScriptValues {
			get {
				return "/carrotcakeadmininfo.axd";
			}
		}

		internal static string RenderView(RenderWidgetData data, PartialViewResult partialResult) {
			return RenderView(data, partialResult, null);
		}

		internal static string RenderView(RenderWidgetData data, PartialViewResult partialResult, string viewName) {
			var engine = CarrotHttpHelper.HttpContext.RequestServices.GetRequiredService(typeof(IRazorViewEngine)) as IRazorViewEngine;

			var controller = data.Controller;
			var routeData = data.RouteData;
			var actualViewName = viewName;

			if (string.IsNullOrEmpty(viewName)) {
				actualViewName = partialResult.ViewName;
				if (string.IsNullOrEmpty(actualViewName)) {
					actualViewName = routeData.Values["action"].ToString();
				}
			}

			var context = data.GetActionContext();

			var viewEngineResult = engine.FindView(context, actualViewName, false);
			var view = viewEngineResult.View;

			var model = partialResult.Model;
			if (model != null) {
				controller.ViewData.Model = model;
			}

			string stringResult = null;

			using (var sw = new StringWriter()) {
				var ctx = data.GetViewContext(sw, view, model);
				var task = view.RenderAsync(ctx);
				stringResult = sw.ToString();
			}

			return stringResult;
		}
	}

	//=======================================

	public enum TextFieldZone {
		TextLeft,
		TextCenter,
		TextRight,
	}

	public enum CommonWidgetZone {
		phCenterTop,
		phCenterBottom,
		phRightTop,
		phRightBottom,
		phLeftTop,
		phLeftBottom,
		phWidgetZone01,
		phWidgetZone02,
		phWidgetZone03,
		phWidgetZone04,
		phWidgetZone05,
		phWidgetZone06,
		phWidgetZone07,
		phWidgetZone08,
		phWidgetZone09,
		phWidgetZone10,
	}

	//=======================================

	public class CarrotCakeHtmlWeb {
		internal IHtmlHelper _helper;
		private RouteValueDictionary _keyValuePairs = new RouteValueDictionary();

		public CarrotCakeHtmlWeb(IHtmlHelper htmlHelper) {
			_helper = htmlHelper;
			_keyValuePairs = new RouteValueDictionary();

			foreach (var route in _helper.ViewContext.RouteData.Values) {
				_keyValuePairs[route.Key] = route.Value;
			}
			// since this is from CmsContent, block area to not accidentally pick up a widget scope
			_keyValuePairs["area"] = string.Empty;
		}

		public string SiteMapUri {
			get { return SiteFilename.SiteMapUri; }
		}

		public string RssUri {
			get { return SiteFilename.RssFeedUri; }
		}

		public Controller GetFauxController() {
			Controller controller = null;

			if (controller == null) {
				var svc = CarrotHttpHelper.HttpContext.RequestServices.GetService<IContentController>();

				if (svc != null && svc is Controller) {
					controller = (Controller)svc;
					var data = new RenderWidgetData(controller, _helper);
					data.InitController();
				}
			}

			return controller;
		}

		public PagePayload CmsPage {
			get {
				var page = new PagePayload();

				if (_helper.ViewContext.HttpContext.Items[PagePayload.ItemKey] != null) {
					page = _helper.ViewContext.HttpContext.Items[PagePayload.ItemKey] as PagePayload;
				} else {
					if (SiteData.CurrentRoutePageID != null) {
						page = PayloadHelper.GetContent();
					} else {
						page = PayloadHelper.GetSamplerPayload();
					}
					_helper.ViewContext.HttpContext.Items[PagePayload.ItemKey] = page;
				}

				return page;
			}
		}

		public HtmlString MetaTags() {
			var sb = new StringBuilder();
			var page = this.CmsPage;

			if (page.TheSite.BlockIndex || page.ThePage.BlockIndex) {
				sb.AppendLine(_helper.CarrotWeb().MetaTag("robots", "noindex,nofollow,noarchive").ToString());
				sb.AppendLine(string.Empty);
			}

			if (!string.IsNullOrEmpty(page.ThePage.MetaKeyword)) {
				sb.AppendLine(_helper.CarrotWeb().MetaTag("keywords", page.ThePage.MetaKeyword).ToString());
				sb.AppendLine(string.Empty);
			}
			if (!string.IsNullOrEmpty(page.ThePage.MetaDescription)) {
				sb.AppendLine(_helper.CarrotWeb().MetaTag("description", page.ThePage.MetaDescription).ToString());
				sb.AppendLine(string.Empty);
			}

			sb.AppendLine(_helper.CarrotWeb().MetaTag("generator", SiteData.CarrotCakeCMSVersion).ToString());
			sb.AppendLine(string.Empty);

			return new HtmlString(sb.ToString());
		}

		public HtmlString RenderOpenGraph(OpenGraph.OpenGraphTypeDef type = OpenGraph.OpenGraphTypeDef.Default, bool showExpire = false) {
			var og = new OpenGraph(this.CmsPage);
			og.ShowExpirationDate = showExpire;
			og.OpenGraphType = type;

			return new HtmlString(og.ToHtmlString());
		}

		public string CurrentViewName {
			get {
				return _helper.ViewContext.View.Path;
			}
		}

		public IPrincipal UserPrincipal {
			get {
				return _helper.ViewContext.HttpContext.User;
			}
		}

		public bool IsAuthenticated {
			get {
				return this.UserPrincipal.Identity.IsAuthenticated;
			}
		}

		public IUrlHelper GetUrlHelper() {
			var urlHelperFactory = _helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
			return urlHelperFactory.GetUrlHelper(_helper.ViewContext);
		}

		public HtmlString Rss(SiteData.RSSFeedInclude mode) {
			return new HtmlString(string.Format("<!-- RSS Header Feed --> <link rel=\"alternate\" type=\"application/rss+xml\" title=\"RSS Feed\" href=\"{0}?type={1}\" /> ", CarrotCakeCmsHelper.RssUri, mode));
		}

		public HtmlString Rss() {
			return Rss(SiteData.RSSFeedInclude.BlogAndPages);
		}

		public HtmlString RssLink(string imagePath = "",
									string imageAltText = "RSS",
									object imageAttributes = null,
									object linkAttributes = null) {
			return RssLink(SiteData.RSSFeedInclude.BlogAndPages, imagePath, imageAltText, imageAttributes, linkAttributes);
		}

		public HtmlString RssLink(SiteData.RSSFeedInclude mode,
											string imagePath = "",
											string imageAltText = "RSS",
											object imageAttributes = null,
											object linkAttributes = null) {
			var url = GetUrlHelper();

			var anchorBuilder = new HtmlTag("a");
			anchorBuilder.Uri = string.Format("{0}?type={1}", CarrotCakeCmsHelper.RssUri, mode);
			anchorBuilder.MergeAttributes(linkAttributes);

			if (string.IsNullOrEmpty(imagePath)) {
				imagePath = ControlUtilities.GetWebResourceUrl("Carrotware.CMS.UI.Components.feed.png");
			}

			var imgBuilder = new HtmlTag("img");
			imgBuilder.Uri = url.Content(imagePath);
			imgBuilder.MergeAttribute("alt", imageAltText);
			imgBuilder.MergeAttribute("title", imageAltText);
			imgBuilder.MergeAttributes(imageAttributes);

			string imgHtml = imgBuilder.RenderSelfClosingTag();

			anchorBuilder.InnerHtml = imgHtml;

			return new HtmlString(anchorBuilder.ToString());
		}

		public HtmlString RssTextLink(string linkText = "RSS", object linkAttributes = null) {
			return RssTextLink(SiteData.RSSFeedInclude.BlogAndPages, linkText, linkAttributes);
		}

		public HtmlString RssTextLink(SiteData.RSSFeedInclude mode, string linkText = "RSS", object linkAttributes = null) {
			var anchorBuilder = new HtmlTag("a");
			anchorBuilder.Uri = string.Format("{0}?type={1}", CarrotCakeCmsHelper.RssUri, mode);
			anchorBuilder.MergeAttributes(linkAttributes);

			anchorBuilder.InnerHtml = linkText;

			return new HtmlString(anchorBuilder.ToString());
		}

		public HtmlString IncludeHead() {
			return IncludeHeader();
		}

		public HtmlString IncludeFoot() {
			return IncludeFooter();
		}

		public HtmlString IncludeHeader() {
			var sb = new StringBuilder();
			sb.AppendLine(string.Empty);

			if (SecurityData.IsAdmin || SecurityData.IsSiteEditor) {
				if (SecurityData.AdvancedEditMode) {
					sb.AppendLine(RenderPartialToString(SiteFilename.AdvancedEditHeadViewPath));
				}
			}

			sb.AppendLine(RenderPartialToString(SiteFilename.MainSiteSpecialViewHead));

			return new HtmlString(sb.ToString().Trim());
		}

		public HtmlString IncludeFooter() {
			var sb = new StringBuilder();
			sb.AppendLine(string.Empty);
			bool isPageTemplate = false;

			if (SecurityData.IsAdmin || SecurityData.IsSiteEditor) {
				if (SecurityData.AdvancedEditMode) {
					sb.AppendLine(RenderPartialToString(SiteFilename.AdvancedEditViewPath));
				} else {
					if (this.CmsPage.ThePage.Root_ContentID == SiteData.CurrentSiteID && SiteData.IsPageReal) {
						isPageTemplate = true;
					}

					if (!SiteData.IsLikelyFakeSearch()) {
						if (!SiteData.IsPageSampler && !isPageTemplate) {
							sb.AppendLine(RenderPartialToString(SiteFilename.EditNotifierViewPath));
						}
					}
				}
			}

			sb.AppendLine(RenderPartialToString(SiteFilename.MainSiteSpecialViewFoot));

			return new HtmlString(sb.ToString().Trim());
		}

		public ContentPageNext GetContentPageNext(ContentPageNext.NavDirection direction) {
			return new ContentPageNext {
				NavigationDirection = direction,
				ContentPage = this.CmsPage.ThePage
			};
		}

		public ContentPageNext GetContentPageNext(ContentPageNext.NavDirection direction, ContentPageNext.CaptionSource caption) {
			return new ContentPageNext {
				NavigationDirection = direction,
				CaptionDataField = caption,
				ContentPage = this.CmsPage.ThePage
			};
		}

		public ContentPageImageThumb GetContentPageImageThumb() {
			return new ContentPageImageThumb {
				ContentPage = this.CmsPage.ThePage
			};
		}

		public BreadCrumbNavigation GetBreadCrumbNavigation() {
			return new BreadCrumbNavigation {
				ContentPage = this.CmsPage.ThePage
			};
		}

		public BreadCrumbNavigation GetBreadCrumbNavigation(string selectedClass) {
			return new BreadCrumbNavigation {
				ContentPage = this.CmsPage.ThePage,
				CssSelected = selectedClass
			};
		}

		public SiteCanonicalURL GetSiteCanonicalURL() {
			return new SiteCanonicalURL {
				ContentPage = this.CmsPage.ThePage
			};
		}

		public SiteCanonicalURL GetSiteCanonicalURL(bool enable301) {
			return new SiteCanonicalURL {
				Enable301Redirect = enable301,
				ContentPage = this.CmsPage.ThePage
			};
		}

		public ChildNavigation GetChildNavigation() {
			return new ChildNavigation {
				CmsPage = this.CmsPage
			};
		}

		public SecondLevelNavigation GetSecondLevelNavigation() {
			return new SecondLevelNavigation {
				CmsPage = this.CmsPage
			};
		}

		public SearchForm BeginSearchForm(object formAttributes = null) {
			return new SearchForm(_helper, this.CmsPage, formAttributes);
		}

		public AjaxContactForm BeginContactForm(SimpleAjaxFormOptions ajaxOptions, object formAttributes = null) {
			ajaxOptions.FormId = "frmContact";
			return new AjaxContactForm(_helper, this.CmsPage, ajaxOptions, formAttributes);
		}

		public AjaxLoginForm BeginLoginForm(SimpleAjaxFormOptions ajaxOptions, object formAttributes = null) {
			ajaxOptions.FormId = "frmLogin";
			return new AjaxLoginForm(_helper, this.CmsPage, ajaxOptions, formAttributes);
		}

		public AjaxLogoutForm BeginLogoutForm(SimpleAjaxFormOptions ajaxOptions, object formAttributes = null) {
			ajaxOptions.FormId = "frmLogout";
			return new AjaxLogoutForm(_helper, this.CmsPage, ajaxOptions, formAttributes);
		}

		public AjaxForgotPasswordForm BeginForgotPasswordForm(SimpleAjaxFormOptions ajaxOptions, object formAttributes = null) {
			ajaxOptions.FormId = "frmPassword";
			return new AjaxForgotPasswordForm(_helper, this.CmsPage, ajaxOptions, formAttributes);
		}

		public AjaxResetPasswordForm BeginResetPasswordForm(SimpleAjaxFormOptions ajaxOptions, object formAttributes = null) {
			ajaxOptions.FormId = "frmResetPwd";
			return new AjaxResetPasswordForm(_helper, this.CmsPage, ajaxOptions, formAttributes);
		}

		public AjaxChangePasswordForm BeginChangePasswordForm(SimpleAjaxFormOptions ajaxOptions, object formAttributes = null) {
			ajaxOptions.FormId = "frmChangePwd";
			return new AjaxChangePasswordForm(_helper, this.CmsPage, ajaxOptions, formAttributes);
		}

		public AjaxChangeProfileForm BeginChangeProfileForm(SimpleAjaxFormOptions ajaxOptions, object formAttributes = null) {
			ajaxOptions.FormId = "frmProfile";
			return new AjaxChangeProfileForm(_helper, this.CmsPage, ajaxOptions, formAttributes);
		}

		private int _widgetCount = 0;

		public int WidgetCount {
			get {
				return _widgetCount++;
			}
		}

		internal RouteData AddUpdateRouting(RouteData routeData, string key, string value) {
			string keyLower = key.ToLowerInvariant();
			if (routeData.Values.ContainsKey(keyLower)) {
				routeData.Values[keyLower] = value;
			} else {
				routeData.Values.Add(keyLower, value);
			}

			return routeData;
		}

		internal string GetResultViewStringFromController(string actionName, Type type, object obj, object payload) {
			bool isFormPost = HttpMethods.IsPost(_helper.ViewContext.HttpContext.Request.Method);

			if (obj is Controller) {
				MethodInfo methodInfo = null;
				Controller controller = null;

				string areaName = type.Assembly.GetAssemblyName();

				if (obj != null && obj is Controller) {
					controller = (Controller)obj;
				} else {
					throw new Exception($"The type {type} was not a controller or did not exist.");
				}

				RouteData routeData = _helper.ViewContext.RouteData;
				AddUpdateRouting(routeData, "controller", type.Name.ToLowerInvariant().Replace("controller", string.Empty));
				AddUpdateRouting(routeData, "action", actionName);
				AddUpdateRouting(routeData, "area", areaName);

				var data = new RenderWidgetData(controller, _helper);
				data.RouteValues = routeData.Values;

				List<MethodInfo> mthds = type.GetMethods().Where(x => x.Name == actionName).ToList();
				if (mthds.Count <= 1) {
					methodInfo = mthds.FirstOrDefault();
				} else {
					if (!isFormPost) {
						methodInfo = mthds.Where(x => x.GetCustomAttributes(typeof(HttpGetAttribute), true).Any()).FirstOrDefault();
						if (methodInfo == null) {
							methodInfo = mthds.Where(x => !x.GetCustomAttributes(typeof(HttpPostAttribute), true).Any()).FirstOrDefault();
						}
					} else {
						methodInfo = mthds.Where(x => x.GetCustomAttributes(typeof(HttpPostAttribute), true).Any()).FirstOrDefault();
						if (methodInfo == null) {
							methodInfo = mthds.Where(x => !x.GetCustomAttributes(typeof(HttpGetAttribute), true).Any()).FirstOrDefault();
						}
					}
				}

				data.InitController();

				if (controller != null && payload != null && controller is IWidgetDataObject) {
					(controller as IWidgetDataObject).WidgetPayload = payload;
				}

				if (methodInfo != null) {
					object result = null;
					ParameterInfo[] parameters = methodInfo.GetParameters();

					if (parameters.Length == 0) {
						result = methodInfo.Invoke(controller, null);
					} else {
						List<object> parametersArray = new List<object>();

						if (!isFormPost || parameters.Length > 1) {
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
						} else {
							if (parameters.Length == 1) {
								var o = controller.ViewData.Model;
								parametersArray.Add(o);
							}
						}

						result = methodInfo.Invoke(controller, parametersArray.ToArray());
					}

					if (result is PartialViewResult) {
						var res = (PartialViewResult)result;

						_helper.ViewContext.ViewData[actionName] = res.ViewData;
						_helper.ViewContext.TempData[actionName] = res.TempData;

						if (string.IsNullOrEmpty(res.ViewName)) {
							res.ViewName = actionName;
						}

						string resultString = CarrotCakeCmsHelper.RenderView(data, res);
						controller.Dispose();

						return resultString;
					}
				}
			}

			return string.Empty;
		}

		public HtmlString RenderBody(TextFieldZone zone) {
			string bodyText = string.Empty;

			switch (zone) {
				case TextFieldZone.TextLeft:
					bodyText = this.CmsPage.ThePage.LeftPageText;
					break;

				case TextFieldZone.TextCenter:
					bodyText = this.CmsPage.ThePage.PageText;
					break;

				case TextFieldZone.TextRight:
					bodyText = this.CmsPage.ThePage.RightPageText;
					break;

				default:
					break;
			}

			bodyText = bodyText ?? string.Empty;

			bodyText = SiteData.CurrentSite.UpdateContent(bodyText);

			if (SecurityData.AdvancedEditMode) {
				AdvContentModel m = new AdvContentModel();
				m.Content = bodyText;
				m.AreaName = zone;
				switch (zone) {
					case TextFieldZone.TextLeft:
						m.Zone = "l";
						break;

					case TextFieldZone.TextCenter:
						m.Zone = "c";
						break;

					case TextFieldZone.TextRight:
						m.Zone = "r";
						break;
				}

				var sb = new StringBuilder();
				sb.Append(ControlUtilities.ReadEmbededScript("Carrotware.CMS.UI.Components._TextZone.cshtml"));

				sb.Replace("[[WIDGET_CONTENT]]", m.Content);
				sb.Replace("[[AREA_NAME]]", m.AreaName.ToString());
				sb.Replace("[[zone]]", m.Zone);
				sb.Replace("[[htmltext]]", SiteData.HtmlMode);
				sb.Replace("[[rawtext]]", SiteData.RawMode);

				bodyText = sb.ToString() ?? string.Empty;
			}

			return new HtmlString(bodyText);
		}

		internal void RestoreOriginalRoutes() {
			// restore original routes
			foreach (var route in _keyValuePairs) {
				_helper.ViewContext.RouteData.Values[route.Key] = route.Value;
			}

			// since this is from CmsContent, block area to not accidentally pick up a widget scope
			_helper.ViewContext.RouteData.Values["area"] = null;
			_helper.ViewContext.RouteData.Values["widgetid"] = null;

			_helper.ViewContext.RouteData.Values.Remove("area");
			_helper.ViewContext.RouteData.Values.Remove("widgetid");
		}

		internal string RenderPartialToString(string partialViewName) {
			return RenderPartialToString(partialViewName, null);
		}

		internal string RenderPartialToString(string partialViewName, object? model) {
			if (model != null) {
				return _helper.Partial(partialViewName, model).RenderToString();
			} else {
				return _helper.Partial(partialViewName).RenderToString();
			}
		}

		internal Task<string> RenderPartialToStringAsync(string partialViewName) {
			return RenderPartialToStringAsync(partialViewName, null);
		}

		internal async Task<string> RenderPartialToStringAsync(string partialViewName, object? model) {
			if (model != null) {
				return (await _helper.PartialAsync(partialViewName, model)).RenderToString();
			} else {
				return (await _helper.PartialAsync(partialViewName)).RenderToString();
			}
		}

		public HtmlString RenderWidget(CommonWidgetZone placeHolderName) {
			return RenderWidget(placeHolderName.ToString());
		}

		public HtmlString RenderWidget(string placeHolderName) {
			var sbWidgetbBody = new StringBuilder();
			var sbWidgetZone = new StringBuilder();
			var sbMasterWidgetWrapper = new StringBuilder();
			string widgetMenuTemplate = string.Empty;
			string sStatusTemplate = string.Empty;

			if (SecurityData.AdvancedEditMode) {
				widgetMenuTemplate = "<li id=\"liMenu\"><a href=\"javascript:[[JS_CALL]]\" id=\"cmsMenuEditLink\" class=\"cmsWidgetBarLink cmsWidgetBarIconPencil\" alt=\"[[CAP]]\" title=\"[[CAP]]\"> [[CAP]]</a></li>";

				sbWidgetZone.Append(ControlUtilities.ReadEmbededScript("Carrotware.CMS.UI.Components._WidgetZone.cshtml"));
				sbMasterWidgetWrapper.Append(ControlUtilities.ReadEmbededScript("Carrotware.CMS.UI.Components._WidgetWrapper.cshtml"));

				sbWidgetZone.Replace("[[PLACEHOLDER]]", placeHolderName);
				sbMasterWidgetWrapper.Replace("[[PLACEHOLDER]]", placeHolderName);
			}

			var widgetList = (from w in this.CmsPage.TheWidgets
							  where w.PlaceholderName.ToLowerInvariant() == placeHolderName.ToLowerInvariant()
							  orderby w.WidgetOrder, w.EditDate
							  select w).ToList();

			foreach (Widget widget in widgetList) {
				RestoreOriginalRoutes();
				_helper.ViewContext.RouteData.Values["widgetid"] = widget.Root_WidgetID;

				bool isWidgetClass = false;
				string widgetKey = string.Format("WidgetId_{0}_{1}", placeHolderName, this.WidgetCount);

				string widgetText = string.Empty;
				string widgetWrapper = string.Empty;
				Dictionary<string, string> lstMenus = new Dictionary<string, string>();

				if (widget.ControlPath.Contains(":")) {
					string[] path = widget.ControlPath.Split(':');
					string objectPrefix = path[0];
					string objectClass = path[1];
					string altView = path.Length >= 3 ? path[2] : string.Empty;

					object obj = null;
					object settings = null;

					try {
						Type objType = ReflectionUtilities.GetTypeFromString(objectClass);
						obj = _helper.ViewContext.HttpContext.RequestServices.GetService(objType);

						if (obj == null) {
							if (objType == typeof(Controller) || obj is IWidgetController) {
								obj = _helper.ViewContext.HttpContext.RequestServices.GetService(objType);
								if (obj == null) {
									obj = CarrotHttpHelper.HttpContext.RequestServices.GetService(objType);
								}
							} else {
								obj = Activator.CreateInstance(objType);
							}
						}

						if (objectPrefix.ToUpperInvariant() != "CLASS") {
							isWidgetClass = false;
							// assumed to be a controller action/method
							object attrib = ReflectionUtilities.GetAttribute<WidgetActionSettingModelAttribute>(objType, objectPrefix);

							if (attrib != null && attrib is WidgetActionSettingModelAttribute) {
								string attrClass = (attrib as WidgetActionSettingModelAttribute).ClassName;
								Type s = ReflectionUtilities.GetTypeFromString(attrClass);
								settings = Activator.CreateInstance(s);
							}
						} else {
							isWidgetClass = true;
							// a class widget is its own setting object
							settings = obj;
						}

						if (settings != null) {
							if (settings is IWidget) {
								IWidget w = settings as IWidget;
								w.SiteID = this.CmsPage.TheSite.SiteID;
								w.RootContentID = widget.Root_ContentID;
								w.PageWidgetID = widget.Root_WidgetID;
								w.IsDynamicInserted = true;
								w.IsBeingEdited = SecurityData.AdvancedEditMode;
								w.WidgetClientID = widgetKey;

								List<WidgetProps> lstProp = widget.ParseDefaultControlProperties();
								w.PublicParmValues = lstProp.ToDictionary(t => t.KeyName, t => t.KeyValue);

								lstMenus = w.JSEditFunctions;

								if (!lstMenus.Any() && w.EnableEdit) {
									lstMenus.Add("Edit", "cmsGenericEdit('" + widget.Root_ContentID.ToString() + "','" + widget.Root_WidgetID.ToString() + "')");
								}
							}

							if (settings is IWidgetView) {
								if (!string.IsNullOrEmpty(altView)) {
									altView = altView.ToLowerInvariant();
									// does not like full paths, use as though a relative view name
									if (altView.StartsWith("/view") && altView.EndsWith("html")) {
										altView = Path.GetFileName(altView);
										altView = Path.GetFileNameWithoutExtension(altView);
									}

									(settings as IWidgetView).AlternateViewFile = altView;
								}
							}

							if (settings is IWidgetRawData) {
								(settings as IWidgetRawData).RawWidgetData = widget.ControlProperties;
							}
						}

						if (obj != null && settings != null && obj is IWidgetDataObject) {
							(obj as IWidgetDataObject).WidgetPayload = settings;
						}

						if (isWidgetClass && obj is IHtmlContent) {
							widgetText = (obj as IHtmlContent).RenderToString();
						} else {
							widgetText = GetResultViewStringFromController(objectPrefix, objType, obj, settings);
						}
					} catch (Exception ex) {
						SiteData.WriteDebugException("renderwidget-class", ex);

						var msg = new LiteralMessage(ex, widgetKey, widget.ControlPath);
						obj = msg;
						widgetText = msg.ToHtmlString();
					}
				}

				widgetText = widgetText ?? string.Empty;

				if (!widget.ControlPath.Contains(":") && string.IsNullOrEmpty(widgetText)) {
					string[] path = widget.ControlPath.Split('|');
					string viewPath = path[0];
					string modelClass = string.Empty;
					if (path.Length > 1) {
						modelClass = path[1];
					}

					try {
						if (viewPath.EndsWith(".cshtml") || viewPath.EndsWith(".vbhtml")) {
							viewPath = viewPath.FixPathSlashes();
							if (string.IsNullOrEmpty(modelClass)) {
								widgetText = RenderPartialToString(viewPath);
							} else {
								Type objType = ReflectionUtilities.GetTypeFromString(modelClass);

								object model = Activator.CreateInstance(objType);

								if (model is IWidgetRawData) {
									(model as IWidgetRawData).RawWidgetData = widget.ControlProperties;
								}

								if (model is IWidget) {
									IWidget w = model as IWidget;
									w.SiteID = this.CmsPage.TheSite.SiteID;
									w.RootContentID = widget.Root_ContentID;
									w.PageWidgetID = widget.Root_WidgetID;
									w.IsDynamicInserted = true;
									w.IsBeingEdited = SecurityData.AdvancedEditMode;
									w.WidgetClientID = widgetKey;

									List<WidgetProps> lstProp = widget.ParseDefaultControlProperties();
									w.PublicParmValues = lstProp.ToDictionary(t => t.KeyName, t => t.KeyValue);

									lstMenus = w.JSEditFunctions;

									if (!lstMenus.Any() && w.EnableEdit) {
										lstMenus.Add("Edit", "cmsGenericEdit('" + widget.Root_ContentID.ToString() + "','" + widget.Root_WidgetID.ToString() + "')");
									}
								}

								widgetText = RenderPartialToString(viewPath, model);
							}
						}
					} catch (Exception ex) {
						SiteData.WriteDebugException("renderwidget-view", ex);

						var msg = new LiteralMessage(ex, widgetKey, widget.ControlPath);
						widgetText = msg.ToHtmlString();
					}
				}

				if (widgetText == null || widget.ControlPath.ToLowerInvariant().EndsWith(".ascx")) {
					var msg = new LiteralMessage("The widget is not supported.", widgetKey, widget.ControlPath);
					widgetText = msg.ToHtmlString();
				}

				widgetText = widgetText ?? string.Empty;

				if (SecurityData.AdvancedEditMode) {
					if (widget.IsWidgetActive) {
						sStatusTemplate = "<a href=\"javascript:cmsRemoveWidgetLink('[[ITEM_ID]]');\" id=\"cmsContentRemoveLink\" class=\"cmsWidgetBarLink cmsWidgetBarIconCross\" alt=\"Remove\" title=\"Remove\">  Disable</a>";
					} else {
						sStatusTemplate = "<a href=\"javascript:cmsActivateWidgetLink('[[ITEM_ID]]');\" id=\"cmsActivateWidgetLink\" class=\"cmsWidgetBarLink cmsWidgetBarIconActive\" alt=\"Activate\" title=\"Activate\">  Enable</a>";
					}

					var sbWidget = new StringBuilder();
					sbWidget.Append(sbMasterWidgetWrapper);

					sbWidget.Replace("[[STATUS_LINK]]", sStatusTemplate);
					sbWidget.Replace("[[WIDGET_PATH]]", widget.ControlPath);
					sbWidget.Replace("[[sequence]]", widget.WidgetOrder.ToString());
					sbWidget.Replace("[[ITEM_ID]]", widget.Root_WidgetID.ToString());

					var plug = (from p in this.CmsPage.Plugins
								where p.FilePath.ToLowerInvariant() == widget.ControlPath.ToLowerInvariant()
								select p).FirstOrDefault();

					string captionPrefix = string.Empty;

					if (!widget.IsWidgetActive) {
						captionPrefix = string.Format("{0} {1}", CMSConfigHelper.InactivePagePrefix, captionPrefix);
					}
					if (widget.IsRetired) {
						captionPrefix = string.Format("{0} {1}", CMSConfigHelper.RetiredPagePrefix, captionPrefix);
					}
					if (widget.IsUnReleased) {
						captionPrefix = string.Format("{0} {1}", CMSConfigHelper.UnreleasedPagePrefix, captionPrefix);
					}
					if (widget.IsWidgetPendingDelete) {
						captionPrefix = string.Format("{0} {1}", CMSConfigHelper.PendingDeletePrefix, captionPrefix);
					}

					if (plug != null) {
						string sysControl = (plug.SystemPlugin ? "[CMS]" : string.Empty);
						sbWidget.Replace("[[WIDGET_CAPTION]]", string.Format("{0}  {1}  {2}", captionPrefix, plug.Caption, sysControl).Trim());
					} else {
						sbWidget.Replace("[[WIDGET_CAPTION]]", string.Format("{0}  UNTITLED", captionPrefix).Trim());
					}

					var sbMenu = new StringBuilder();
					sbMenu.AppendLine();
					if (lstMenus != null) {
						foreach (var d in lstMenus) {
							sbMenu.AppendLine(widgetMenuTemplate.Replace("[[JS_CALL]]", d.Value).Replace("[[CAP]]", d.Key));
						}
					}

					sbWidget.Replace("[[WIDGET_MENU_ITEMS]]", sbMenu.ToString().Trim());
					sbWidget.Replace("[[WIDGET_CAPTION]]", widget.ControlPath + captionPrefix);

					sbWidget.Replace("[[WIDGET_CONTENT]]", widgetText);

					widgetWrapper = sbWidget.ToString();
				} else {
					widgetWrapper = widgetText;
				}

				if (!string.IsNullOrEmpty(widgetWrapper)) {
					sbWidgetbBody.AppendLine(widgetWrapper);
				}
			}

			string bodyText = string.Empty;

			if (SecurityData.AdvancedEditMode) {
				bodyText = sbWidgetZone.Replace("[[WIDGET_CONTENT]]", sbWidgetbBody.ToString()).ToString();
			} else {
				bodyText = sbWidgetbBody.ToString();
			}

			RestoreOriginalRoutes();

			return new HtmlString(bodyText);
		}
	}
}