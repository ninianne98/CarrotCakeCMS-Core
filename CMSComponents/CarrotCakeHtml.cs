using Carrotware.CMS.Core;
using Carrotware.CMS.Interface;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Globalization;
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
				return "/carrotcakeadmininfo.ashx";
			}
		}

		internal static string RenderView(RenderWidgetData data, PartialViewResult partialResult) {
			return RenderView(data, partialResult, null);
		}

		internal static string RenderView(RenderWidgetData data, PartialViewResult partialResult, string viewName) {
			//var controller = data.Controller;
			//var routeData = data.RouteData;
			//var actualViewName = viewName;

			string stringResult = partialResult != null ? partialResult.ResultToString(data, viewName) : string.Empty;

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
			_keyValuePairs[RouteInfo.Keys.Area] = string.Empty;
		}

		public string SiteMapUri {
			get { return SiteFilename.SiteMapUri; }
		}

		public string RssUri {
			get { return SiteFilename.RssFeedUri; }
		}

		public Controller? GetFauxController() {
			Controller? controller = null;

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

		public HtmlString RenderOpenGraph(OpenGraph.OpenGraphTypeDef type = OpenGraph.OpenGraphTypeDef.Default,
					bool showExpire = false) {
			var og = new OpenGraph(this.CmsPage);
			og.ShowExpirationDate = showExpire;
			og.OpenGraphType = type;

			return new HtmlString(og.ToHtmlString());
		}

		public HtmlString SocialMetaTags() {
			return SocialMetaTags(string.Empty);
		}

		public HtmlString SocialMetaTags(string twitterSite) {
			var sb = new StringBuilder();
			sb.AppendLine(string.Empty);

			// Extract the active page payload context directly from the instance
			var page = this.CmsPage;
			if (page == null || page.ThePage == null) {
				return new HtmlString(sb.ToString());
			}

			var cp = page.ThePage;
			var site = page.TheSite;
			string culture = CultureInfo.CurrentUICulture.Name.Replace("-", "_");

			// Resolve Page Mapping
			bool isHome = cp.NavOrder == 0;
			string siteName = site != null ? site.SiteName : string.Empty;
			string pageTitle = !string.IsNullOrEmpty(cp.TitleBar) ? cp.TitleBar : (cp.PageHead ?? cp.NavMenuText ?? string.Empty);
			string pageDesc = cp.MetaDescription ?? string.Empty;
			if (string.IsNullOrEmpty(pageDesc)) {
				pageDesc = cp.PageTextPlainSummary.ToString() ?? string.Empty;
			}
			if (string.IsNullOrEmpty(pageDesc)) {
				pageDesc = cp.NavMenuText ?? string.Empty;
			}

			string absoluteUrl = cp.GetDefaultUri();
			string absoluteImageUrl = string.IsNullOrWhiteSpace(cp.Thumbnail) == false ?
								(_helper.ViewContext.HttpContext.Request.PathBase + cp.Thumbnail) : string.Empty;

			string twitterHandle = twitterSite;
			if (string.IsNullOrEmpty(twitterHandle)) {
				var ta = CarrotHttpHelper.Configuration.GetValue<string>("CarrotTwitterAccount") ?? string.Empty;
				if (!string.IsNullOrWhiteSpace(ta)) {
					twitterHandle = ta;
				}
			}

			if (!string.IsNullOrWhiteSpace(twitterHandle) && !twitterHandle.StartsWith("@")) {
				twitterHandle = "@" + twitterHandle;
			}

			// Common Open Graph Tags
			sb.AppendLine("<!-- Open Graph Meta Tags -->");
			sb.AppendLine(_helper.CarrotWeb().MetaTag("og:site_name", siteName).ToString());
			sb.AppendLine(_helper.CarrotWeb().MetaTag("og:locale", culture).ToString());
			sb.AppendLine(_helper.CarrotWeb().MetaTag("og:title", pageTitle).ToString());
			sb.AppendLine(_helper.CarrotWeb().MetaTag("og:description", pageDesc).ToString());
			sb.AppendLine(_helper.CarrotWeb().MetaTag("og:url", absoluteUrl).ToString());

			var pageType = isHome ? "frontpage" : (page.IsBlogPost ? "article" : "website");
			sb.AppendLine(_helper.CarrotWeb().MetaTag("og:type", pageType).ToString());

			if (page.IsBlogPost) {
				sb.AppendLine(_helper.CarrotWeb().MetaTag("article:published_time", site.ConvertSiteTimeToISO8601(cp.GoLiveDate)).ToString());
				sb.AppendLine(_helper.CarrotWeb().MetaTag("article:modified_time", site.ConvertSiteTimeToISO8601(cp.EditDate)).ToString());

				if (cp.BylineUser != null && !string.IsNullOrEmpty(cp.BylineUser.FullName_FirstLast)
							&& cp.BylineUser.FullName_FirstLast != cp.BylineUser.UserName) {
					sb.AppendLine(_helper.CarrotWeb().MetaTag("article:author", cp.BylineUser.FullName_FirstLast).ToString());
				}
				foreach (var pc in page.GetPageCategories(10)) {
					sb.AppendLine(_helper.CarrotWeb().MetaTag("article:section", pc.CategoryText).ToString());
				}
				foreach (var pt in page.GetPageTags(10)) {
					sb.AppendLine(_helper.CarrotWeb().MetaTag("article:tag", pt.TagText).ToString());
				}
			}

			// Convert and resolve virtual paths to absolute application URLs
			if (!string.IsNullOrEmpty(absoluteImageUrl)) {
				sb.AppendLine(_helper.CarrotWeb().MetaTag("og:image", absoluteImageUrl).ToString());
			}

			// Output Twitter Card Tags
			sb.AppendLine(Environment.NewLine + "<!-- Twitter Card Meta Tags -->");
			if (!string.IsNullOrEmpty(twitterHandle)) {
				sb.AppendLine(_helper.CarrotWeb().MetaTag("twitter:site", twitterHandle).ToString());
			}
			sb.AppendLine(_helper.CarrotWeb().MetaTag("twitter:card", "summary").ToString()); // or summary_large_image
			sb.AppendLine(_helper.CarrotWeb().MetaTag("twitter:title", pageTitle).ToString());
			sb.AppendLine(_helper.CarrotWeb().MetaTag("twitter:description", pageDesc).ToString());

			if (!string.IsNullOrEmpty(absoluteImageUrl)) {
				sb.AppendLine(_helper.CarrotWeb().MetaTag("twitter:image", absoluteImageUrl).ToString());
			}

			sb.Replace(Environment.NewLine, Environment.NewLine + "\t").Replace("\t\t", "\t");
			return new HtmlString(sb.ToString());
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
			return new SiteCanonicalURL(this.CmsPage.ThePage);
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

				routeData.SetRouteValues(areaName, type.GetControllerName(), actionName, null);

				var data = new RenderWidgetData(controller, _helper);
				data.RouteValues = routeData.Values;

				List<MethodInfo> mthds = type.GetMethods().Where(x => x.Name.ToLowerInvariant() == actionName.ToLowerInvariant()).ToList();
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
						var partial = (PartialViewResult)result;

						if (string.IsNullOrEmpty(partial.ViewName)) {
							partial.ViewName = actionName;
						}

						string resultString = CarrotCakeCmsHelper.RenderView(data, partial);
						controller.Dispose();

						return resultString;
					}
				}
			}

			return string.Empty;
		}

		internal async Task<string> GetResultViewStringFromControllerAsync(string actionName, Type type, object obj, object payload) {
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

				routeData.SetRouteValues(areaName, type.GetControllerName(), actionName, null);

				var data = new RenderWidgetData(controller, _helper);
				data.RouteValues = routeData.Values;

				List<MethodInfo> mthds = type.GetMethods().Where(x => x.Name.ToLowerInvariant() == actionName.ToLowerInvariant()).ToList();
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

					if (result is Task task) {
						await task;

						if (task.GetType().IsGenericType) {
							result = task.GetType().GetProperty("Result")?.GetValue(task);
						}
					}

					if (result is PartialViewResult partial) {
						if (string.IsNullOrEmpty(partial.ViewName)) {
							partial.ViewName = actionName;
						}

						string resultString = CarrotCakeCmsHelper.RenderView(data, partial);
						controller.Dispose();

						return resultString;
					}
				}
			}

			return string.Empty;
		}

		public HtmlString RenderBody() {
			return RenderBody(TextFieldZone.TextCenter);
		}

		public Task<HtmlString> RenderBodyAsync() {
			return RenderBodyAsync(TextFieldZone.TextCenter);
		}

		public HtmlString RenderBody(TextFieldZone zone) {
			string bodyText = string.Empty;

			switch (zone) {
				case TextFieldZone.TextLeft:
					bodyText = this.CmsPage.ThePage.LeftPageText ?? string.Empty;
					break;

				case TextFieldZone.TextCenter:
					bodyText = this.CmsPage.ThePage.PageText ?? string.Empty;
					break;

				case TextFieldZone.TextRight:
					bodyText = this.CmsPage.ThePage.RightPageText ?? string.Empty;
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

				sb.Replace("[[cms_zone]]", m.Zone);
				sb.Replace("[[htmltext]]", SiteData.HtmlMode);
				sb.Replace("[[rawtext]]", SiteData.RawMode);
				sb.Replace("[[CMS_AREA_NAME]]", m.AreaName.ToString());
				sb.Replace("[[Þ¤CMS_TEXT_CONTENT¤Þ]]", m.Content);

				bodyText = sb.ToString() ?? string.Empty;
			}

			return new HtmlString(bodyText);
		}

		public Task<HtmlString> RenderBodyAsync(TextFieldZone zone) {
			return Task.FromResult(RenderBody(zone));
		}

		internal void RestoreOriginalRoutes() {
			// restore original routes
			foreach (var route in _keyValuePairs) {
				_helper.ViewContext.RouteData.Values[route.Key] = route.Value;
			}

			// since this is from CmsContent, block area to not accidentally pick up a widget scope
			_helper.ViewContext.RouteData.Values[RouteInfo.Keys.Area] = null;
			_helper.ViewContext.RouteData.Values[CmsRouting.Keys.WidgetId] = null;

			_helper.ViewContext.RouteData.Values.Remove(RouteInfo.Keys.Area);
			_helper.ViewContext.RouteData.Values.Remove(CmsRouting.Keys.WidgetId);
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

		public Task<HtmlString> RenderWidgetAsync(CommonWidgetZone placeHolderName) {
			return RenderWidgetAsync(placeHolderName.ToString());
		}

		public HtmlString RenderWidget(string placeHolderName) {
			var (menuTemplate, zoneTemplate, wrapperTemplate) = GetAdminTemplates(placeHolderName);
			var sbWidgetbBody = new StringBuilder();

			var widgetList = GetWidgetList(placeHolderName, out PagePayload page);
			var siteId = page.TheSite.SiteID;

			foreach (Widget widget in widgetList) {
				var lstMenus = new Dictionary<string, string>();
				string widgetKey = PrepareWidgetLoop(widget, placeHolderName);

				string widgetText = ResolveWidgetTextInternal(widget, widgetKey, siteId,
					(action, type, obj, payload) => Task.FromResult(GetResultViewStringFromController(action, type, obj, payload)),
					(view, model) => Task.FromResult(RenderPartialToString(view, model)),
					lstMenus, "renderwidget").GetAwaiter().GetResult();

				ProcessWidgetFinal(sbWidgetbBody, widget, page, widgetKey, widgetText, lstMenus, menuTemplate, wrapperTemplate);
			}

			return FinalizeWidgetZone(sbWidgetbBody, zoneTemplate);
		}

		public async Task<HtmlString> RenderWidgetAsync(string placeHolderName) {
			var (menuTemplate, zoneTemplate, wrapperTemplate) = GetAdminTemplates(placeHolderName);
			var sbWidgetbBody = new StringBuilder();

			var widgetList = GetWidgetList(placeHolderName, out PagePayload page);
			var siteId = page.TheSite.SiteID;

			foreach (Widget widget in widgetList) {
				var lstMenus = new Dictionary<string, string>();
				string widgetKey = PrepareWidgetLoop(widget, placeHolderName);

				string widgetText = await ResolveWidgetTextInternal(widget, widgetKey, siteId,
					(action, type, obj, payload) => GetResultViewStringFromControllerAsync(action, type, obj, payload),
					(view, model) => RenderPartialToStringAsync(view, model),
					lstMenus, "renderwidget-async");

				ProcessWidgetFinal(sbWidgetbBody, widget, page, widgetKey, widgetText, lstMenus, menuTemplate, wrapperTemplate);
			}

			return FinalizeWidgetZone(sbWidgetbBody, zoneTemplate);
		}

		private List<Widget> GetWidgetList(string placeHolderName, out PagePayload page) {
			page = this.CmsPage ?? new PagePayload();

			return (from w in page.TheWidgets
					where w.PlaceholderName.ToLowerInvariant() == placeHolderName.ToLowerInvariant()
					orderby w.WidgetOrder, w.EditDate
					select w).ToList();
		}

		private string PrepareWidgetLoop(Widget widget, string placeHolderName) {
			RestoreOriginalRoutes();
			_helper.ViewContext.RouteData.Values[CmsRouting.Keys.WidgetId] = widget.Root_WidgetID;
			return $"WidgetId_{placeHolderName}_{this.WidgetCount}";
		}

		private async Task<string> ResolveWidgetTextInternal(Widget widget, string widgetKey, Guid siteId,
			Func<string, Type, object?, object?, Task<string>> controllerRunner,
			Func<string, object?, Task<string>> partialRunner,
			Dictionary<string, string> lstMenus, string logKey) {
			string widgetText = string.Empty;

			if (widget.ControlPath.Contains(':')) {
				string[] path = widget.ControlPath.Split(':');
				string objectPrefix = path[0];
				string objectClass = path[1];
				string altView = path.Length >= 3 ? path[2] : string.Empty;
				bool isWidgetClass = objectPrefix.ToUpperInvariant() == "CLASS";

				try {
					Type objType = ReflectionUtilities.GetTypeFromString(objectClass);
					object? obj = ResolveWidgetObject(objType);
					object? settings = isWidgetClass ? obj : null;

					if (!isWidgetClass) {
						object attrib = ReflectionUtilities.GetAttribute<WidgetActionSettingModelAttribute>(objType, objectPrefix);
						if (attrib is WidgetActionSettingModelAttribute settingAttr) {
							settings = Activator.CreateInstance(ReflectionUtilities.GetTypeFromString(settingAttr.ClassName));
						}
					}

					ConfigureWidget(widget, widgetKey, siteId, obj, settings, altView, out var menus);
					foreach (var menu in menus) lstMenus[menu.Key] = menu.Value;

					if (isWidgetClass && obj is IHtmlContent htmlContent) {
						widgetText = htmlContent.RenderToString();
					} else {
						widgetText = await controllerRunner(objectPrefix, objType, obj, settings);
					}
				} catch (Exception ex) { widgetText = HandleWidgetException(ex, widgetKey, widget.ControlPath, $"{logKey}-class"); }
			}

			if (!widget.ControlPath.Contains(':') && string.IsNullOrEmpty(widgetText)) {
				string[] path = widget.ControlPath.Split('|');
				string viewPath = path[0].FixPathSlashes();
				string modelClass = path.Length > 1 ? path[1] : string.Empty;

				try {
					if (viewPath.EndsWith(".cshtml") || viewPath.EndsWith(".vbhtml")) {
						object? model = null;
						if (!string.IsNullOrEmpty(modelClass)) {
							model = Activator.CreateInstance(ReflectionUtilities.GetTypeFromString(modelClass));
							ConfigureWidget(widget, widgetKey, siteId, null, model, null, out var menus);
							foreach (var menu in menus) lstMenus[menu.Key] = menu.Value;
						}
						widgetText = await partialRunner(viewPath, model);
					}
				} catch (Exception ex) { widgetText = HandleWidgetException(ex, widgetKey, widget.ControlPath, $"{logKey}-view"); }
			}

			return widgetText;
		}

		private void ProcessWidgetFinal(StringBuilder sb, Widget widget, PagePayload page, string widgetKey, string widgetText, Dictionary<string, string> lstMenus, string menuTemplate, string wrapperTemplate) {
			widgetText = GetUnsupportedWidgetText(widget, widgetKey, widgetText);

			if (SecurityData.AdvancedEditMode) {
				widgetText = WrapWidgetAdmin(widget, page, widgetText, lstMenus, menuTemplate, wrapperTemplate);
			}

			if (!string.IsNullOrEmpty(widgetText)) {
				sb.AppendLine(widgetText);
			}
		}

		private HtmlString FinalizeWidgetZone(StringBuilder sb, string zoneTemplate) {
			string bodyText = SecurityData.AdvancedEditMode
				? zoneTemplate.Replace("[[Þ¤CMS_WIDGET_CONTENT¤Þ]]", sb.ToString())
				: sb.ToString();

			RestoreOriginalRoutes();
			return new HtmlString(bodyText);
		}

		private (string menu, string zone, string wrapper) GetAdminTemplates(string placeHolderName) {
			if (!SecurityData.AdvancedEditMode) return (string.Empty, string.Empty, string.Empty);

			string menuTemplate = "<li id=\"liMenu\"><a href=\"javascript:[[JS_CALL]]\" id=\"cmsMenuEditLink\" class=\"cmsWidgetBarLink cmsWidgetBarIconPencil\" alt=\"[[CAP]]\" title=\"[[CAP]]\"> [[CAP]]</a></li>";
			var zone = new StringBuilder(ControlUtilities.ReadEmbededScript("Carrotware.CMS.UI.Components._WidgetZone.cshtml"));
			var wrapper = new StringBuilder(ControlUtilities.ReadEmbededScript("Carrotware.CMS.UI.Components._WidgetWrapper.cshtml"));

			zone.Replace("[[CMS_WIDGET_PLACEHOLDER]]", placeHolderName);
			wrapper.Replace("[[CMS_WIDGET_PLACEHOLDER]]", placeHolderName);

			return (menuTemplate, zone.ToString(), wrapper.ToString());
		}

		private void ConfigureWidget(Widget widget, string widgetKey, Guid siteId, object? obj, object? settings, string? altView, out Dictionary<string, string> lstMenus) {
			lstMenus = new Dictionary<string, string>();

			if (settings is IWidget w) {
				w.SiteID = siteId;
				w.RootContentID = widget.Root_ContentID;
				w.PageWidgetID = widget.Root_WidgetID;
				w.IsDynamicInserted = true;
				w.IsBeingEdited = SecurityData.AdvancedEditMode;
				w.WidgetClientID = widgetKey;
				w.PublicParmValues = widget.ParseDefaultControlProperties().ToDictionary(t => t.KeyName, t => t.KeyValue);

				lstMenus = w.JSEditFunctions;

				if (!lstMenus.Any() && w.EnableEdit) {
					lstMenus.Add("Edit", $"cmsGenericEdit('{widget.Root_ContentID}','{widget.Root_WidgetID}')");
				}
			}

			if (settings is IWidgetView wv && !string.IsNullOrEmpty(altView)) {
				string view = altView.ToLowerInvariant();
				if (view.StartsWith("/view") && view.EndsWith("html")) {
					view = Path.GetFileNameWithoutExtension(view);
				}
				wv.AlternateViewFile = view;
			}

			if (settings is IWidgetRawData rd) { rd.RawWidgetData = widget.ControlProperties ?? string.Empty; }
			if (obj is IWidgetDataObject dataObj && settings != null) { dataObj.WidgetPayload = settings; }
		}

		private string WrapWidgetAdmin(Widget widget, PagePayload page, string widgetText, Dictionary<string, string> lstMenus, string menuTemplate, string wrapperTemplate) {
			string sStatusTemplate = widget.IsWidgetActive
				? "<a href=\"javascript:cmsRemoveWidgetLink('[[ITEM_ID]]');\" id=\"cmsContentRemoveLink\" class=\"cmsWidgetBarLink cmsWidgetBarIconCross\" alt=\"Remove\" title=\"Remove\">  Disable</a>"
				: "<a href=\"javascript:cmsActivateWidgetLink('[[ITEM_ID]]');\" id=\"cmsActivateWidgetLink\" class=\"cmsWidgetBarLink cmsWidgetBarIconActive\" alt=\"Activate\" title=\"Activate\">  Enable</a>";

			var sbWidget = new StringBuilder(wrapperTemplate);
			sbWidget.Replace("[[CMS_STATUS_LINK]]", sStatusTemplate);
			sbWidget.Replace("[[CMS_WIDGET_PATH]]", widget.ControlPath);
			sbWidget.Replace("[[sequence]]", widget.WidgetOrder.ToString());
			sbWidget.Replace("[[ITEM_ID]]", widget.Root_WidgetID.ToString());

			var plug = page.Plugins.FirstOrDefault(p => p.FilePath.Equals(widget.ControlPath, StringComparison.InvariantCultureIgnoreCase));
			string captionPrefix = "";

			if (!widget.IsWidgetActive) captionPrefix += CMSConfigHelper.InactivePagePrefix + " ";
			if (widget.IsRetired) captionPrefix += CMSConfigHelper.RetiredPagePrefix + " ";
			if (widget.IsUnReleased) captionPrefix += CMSConfigHelper.UnreleasedPagePrefix + " ";
			if (widget.IsWidgetPendingDelete) captionPrefix += CMSConfigHelper.PendingDeletePrefix + " ";

			string caption = plug != null ? $"{captionPrefix} {plug.Caption} {(plug.SystemPlugin ? "[CMS]" : "")}" : $"{captionPrefix} UNTITLED";
			sbWidget.Replace("[[CMS_WIDGET_CAPTION]]", caption.Trim());

			var sbMenu = new StringBuilder();

			foreach (var d in lstMenus) {
				sbMenu.AppendLine(menuTemplate.Replace("[[JS_CALL]]", d.Value).Replace("[[CAP]]", d.Key));
			}
			sbWidget.Replace("[[WIDGET_MENU_ITEMS]]", sbMenu.ToString().Trim());
			sbWidget.Replace("[[CMS_WIDGET_CAPTION]]", widget.ControlPath + captionPrefix);
			sbWidget.Replace("[[Þ¤CMS_WIDGET_CONTENT¤Þ]]", widgetText);

			return sbWidget.ToString();
		}

		private object? ResolveWidgetObject(Type objType) {
			object? obj = _helper.ViewContext.HttpContext.RequestServices.GetService(objType);

			if (obj == null) {
				if (objType == typeof(Controller) || typeof(IWidgetController).IsAssignableFrom(objType)) {
					obj = _helper.ViewContext.HttpContext.RequestServices.GetService(objType)
								?? CarrotHttpHelper.HttpContext.RequestServices.GetService(objType);
				} else { obj = Activator.CreateInstance(objType); }
			}
			return obj;
		}

		private string HandleWidgetException(Exception ex, string widgetKey, string controlPath, string logKey) {
			SiteData.WriteDebugException(logKey, ex);
			return new LiteralMessage(ex, widgetKey, controlPath).ToHtmlString();
		}

		private string GetUnsupportedWidgetText(Widget widget, string widgetKey, string? widgetText) {
			if (widgetText == null || widget.ControlPath.ToLowerInvariant().EndsWith(".ascx")) {
				return new LiteralMessage("The widget is not supported.", widgetKey, widget.ControlPath).ToHtmlString();
			}
			return widgetText;
		}
	}
}