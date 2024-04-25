using Carrotware.CMS.Core;
using Carrotware.CMS.CoreMVC.UI.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Xml.Serialization;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, 2024 Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023, April 2024
*/

namespace Carrotware.CMS.CoreMVC.UI.Admin.Controllers {

	[CmsAuthorize]
	public class CmsAdminApiController : ControllerBase, IDisposable {

		public static class ServiceResponse {
			public static string OK { get { return "OK"; } }
			public static string Fail { get { return "FAIL"; } }

			public static bool IsFail(string result) {
				return string.IsNullOrEmpty(result)
					|| result.ToUpperInvariant().Trim() == Fail
					|| result.ToUpperInvariant() != OK;
			}

			public static bool IsOK(string result) {
				if (string.IsNullOrEmpty(result)) {
					return false;
				}

				return result.ToUpperInvariant().Trim() == OK;
			}
		}

		private readonly ILogger _logger;

		public CmsAdminApiController(ILogger<CmsAdminApiController> logger) {
			_logger = logger;
		}

		protected ContentPageHelper pageHelper = new ContentPageHelper();
		protected WidgetHelper widgetHelper = new WidgetHelper();
		protected SiteMapOrderHelper sitemapHelper = new SiteMapOrderHelper();

		protected Guid currentPageGuid = Guid.Empty;
		protected ContentPage filePage = null;

		public ContentPage cmsAdminContent {
			get {
				ContentPage c = null;
				try {
					string xml = GetSerialized(CMSConfigHelper.keyAdminContent);
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(ContentPage));
					object genpref = null;
					using (StringReader stringReader = new StringReader(xml)) {
						genpref = xmlSerializer.Deserialize(stringReader);
					}
					c = genpref as ContentPage;
				} catch { }
				return c;
			}
			set {
				if (value == null) {
					ClearSerialized(CMSConfigHelper.keyAdminContent);
				} else {
					string xml = string.Empty;
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(ContentPage));
					using (StringWriter stringWriter = new StringWriter()) {
						xmlSerializer.Serialize(stringWriter, value);
						xml = stringWriter.ToString();
					}
					SaveSerialized(CMSConfigHelper.keyAdminContent, xml);
				}
			}
		}

		public List<Widget> cmsAdminWidget {
			get {
				List<Widget> c = null;
				string xml = GetSerialized(CMSConfigHelper.keyAdminWidget);
				//since a page may not have any widgets, initialize it and skip deserializing
				if (!string.IsNullOrEmpty(xml)) {
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Widget>));
					object genpref = null;
					using (StringReader stringReader = new StringReader(xml)) {
						genpref = xmlSerializer.Deserialize(stringReader);
					}
					c = genpref as List<Widget>;
				} else {
					c = new List<Widget>();
				}
				return c;
			}
			set {
				if (value == null) {
					ClearSerialized(CMSConfigHelper.keyAdminWidget);
				} else {
					string xml = string.Empty;
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Widget>));
					using (StringWriter stringWriter = new StringWriter()) {
						xmlSerializer.Serialize(stringWriter, value);
						xml = stringWriter.ToString();
					}
					SaveSerialized(CMSConfigHelper.keyAdminWidget, xml);
				}
			}
		}

		private void SaveSerialized(string key, string data) {
			LoadGuids();

			CMSConfigHelper.SaveSerialized(currentPageGuid, key, data);
		}

		private string GetSerialized(string key) {
			string data = string.Empty;
			LoadGuids();

			data = CMSConfigHelper.GetSerialized(currentPageGuid, key);

			return data;
		}

		private bool ClearSerialized(string key) {
			LoadGuids();

			return CMSConfigHelper.ClearSerialized(currentPageGuid, key);
		}

		private void LoadGuids() {
			using (var pageHelper = new ContentPageHelper()) {
				if (!string.IsNullOrEmpty(CurrentEditPage)) {
					filePage = pageHelper.FindByFilename(SiteData.CurrentSite.SiteID, CurrentEditPage);
					if (filePage != null) {
						currentPageGuid = filePage.Root_ContentID;
					}
				} else {
					if (currentPageGuid != Guid.Empty) {
						filePage = pageHelper.FindContentByID(SiteData.CurrentSite.SiteID, currentPageGuid);
						if (filePage != null) {
							CurrentEditPage = filePage.FileName;
						}
					} else {
						filePage = new ContentPage();
					}
				}
			}
		}

		[HttpGet]
		public string GetSiteAdminFolder() {
			return JsonSerializer.Serialize(SiteData.AdminFolderPath);
		}

		private string CurrentEditPage = string.Empty;

		[HttpPost]
		public string RecordHeartbeat([FromBody] ApiModel model) {
			try {
				string pageId = model.PageID;

				currentPageGuid = new Guid(pageId);

				bool ret = pageHelper.RecordPageLock(currentPageGuid, SiteData.CurrentSite.SiteID, SecurityData.CurrentUserGuid);

				if (ret) {
					return JsonSerializer.Serialize(SiteData.CurrentSite.Now.ToString());
				} else {
					return JsonSerializer.Serialize(Convert.ToDateTime("12/31/1899").ToString());
				}
			} catch (Exception ex) {
				_logger.LogError(ex, "RecordHeartbeat");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(DateTime.MinValue.ToString());
			}
		}

		[HttpPost]
		public string CancelEditing([FromBody] ApiModel model) {
			try {
				string thisPage = model.ThisPage;

				currentPageGuid = new Guid(thisPage);

				pageHelper.ResetHeartbeatLock(currentPageGuid, SiteData.CurrentSite.SiteID, SecurityData.CurrentUserGuid);

				GetSetUserEditStateAsEmpty();

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				_logger.LogError(ex, "CancelEditing");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string RecordEditorPosition([FromBody] ApiModel model) {
			try {
				string toolbarState = model.ToolbarState;
				string toolbarMargin = model.ToolbarMargin;
				string toolbarScroll = model.ToolbarScroll;
				string widgetScroll = model.WidgetScroll;
				string selTabId = model.SelTabID;

				GetSetUserEditState(toolbarState, toolbarMargin, toolbarScroll, widgetScroll, selTabId);

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				_logger.LogError(ex, "RecordEditorPosition");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		private void GetSetUserEditStateAsEmpty() {
			GetSetUserEditState(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
		}

		private void GetSetUserEditState(string toolbarState, string toolbarMargin, string toolbarScroll, string widgetScroll, string selTabID) {
			UserEditState editor = UserEditState.cmsUserEditState;

			if (editor == null) {
				editor = new UserEditState();
				editor.Init();
			}

			editor.EditorMargin = string.IsNullOrEmpty(toolbarMargin) ? "L" : toolbarMargin.ToUpperInvariant();
			editor.EditorOpen = string.IsNullOrEmpty(toolbarState) ? "true" : toolbarState.ToLowerInvariant();
			editor.EditorWidgetScrollPosition = string.IsNullOrEmpty(widgetScroll) ? "0" : widgetScroll.ToLowerInvariant();
			editor.EditorScrollPosition = string.IsNullOrEmpty(toolbarScroll) ? "0" : toolbarScroll.ToLowerInvariant();
			editor.EditorSelectedTabIdx = string.IsNullOrEmpty(selTabID) ? "0" : selTabID.ToLowerInvariant();

			if (string.IsNullOrEmpty(toolbarMargin) && string.IsNullOrEmpty(toolbarState)) {
				UserEditState.cmsUserEditState = null;
			} else {
				UserEditState.cmsUserEditState = editor;
			}
		}

		[HttpGet]
		public string GetChildPages(string PageID, string CurrPageID) {
			Guid? parentId = Guid.Empty;
			if (!string.IsNullOrEmpty(PageID)) {
				if (PageID.Length > 20) {
					parentId = new Guid(PageID);
				}
			}

			Guid contPageId = Guid.Empty;
			if (!string.IsNullOrEmpty(CurrPageID)) {
				if (CurrPageID.Length > 20) {
					contPageId = new Guid(CurrPageID);
				}
			}

			List<SiteMapOrder> lstSiteMap = new List<SiteMapOrder>();

			try {
				if (SiteData.CurrentSiteExists) {
					List<SiteMapOrder> lst = sitemapHelper.GetChildPages(SiteData.CurrentSite.SiteID, parentId, contPageId);

					lstSiteMap = (from l in lst
								  orderby l.NavOrder, l.NavMenuText
								  where l.Parent_ContentID != contPageId || l.Parent_ContentID == null
								  select l).ToList();
				}
			} catch (Exception ex) {
				_logger.LogError(ex, "GetChildPages");
				SiteData.WriteDebugException("webservice", ex);

				throw;
			}

			return JsonSerializer.Serialize(lstSiteMap);
		}

		[HttpGet]
		public string GetPageCrumbs(string PageID, string CurrPageID) {
			Guid? contentPageId = Guid.Empty;
			Guid contPageId = Guid.Empty;
			List<SiteMapOrder> lstSiteMap = new List<SiteMapOrder>();

			try {
				if (!string.IsNullOrEmpty(PageID)) {
					if (PageID.Length > 20) {
						contentPageId = new Guid(PageID);
					}
				}

				if (!string.IsNullOrEmpty(CurrPageID)) {
					if (CurrPageID.Length > 20) {
						contPageId = new Guid(CurrPageID);
					}
				}

				int iLevel = 0;

				int iLenB = 0;
				int iLenA = 1;

				while (iLenB < iLenA && SiteData.CurrentSiteExists) {
					iLenB = lstSiteMap.Count;

					SiteMapOrder cont = sitemapHelper.GetPageWithLevel(SiteData.CurrentSite.SiteID, contentPageId, iLevel);

					iLevel++;
					if (cont != null) {
						contentPageId = cont.Parent_ContentID;
						lstSiteMap.Add(cont);
					}

					iLenA = lstSiteMap.Count;
				}
			} catch (Exception ex) {
				_logger.LogError(ex, "GetPageCrumbs");
				SiteData.WriteDebugException("webservice", ex);

				throw;
			}

			return JsonSerializer.Serialize(lstSiteMap.OrderByDescending(y => y.NavLevel).ToList());
		}

		[HttpPost]
		public string UpdatePageTemplate([FromBody] ApiModel model) {
			try {
				string theTemplate = model.TheTemplate;
				string thisPage = model.ThisPage;

				theTemplate = CMSConfigHelper.DecodeBase64(theTemplate);
				currentPageGuid = new Guid(thisPage);
				LoadGuids();

				ContentPage c = cmsAdminContent;

				c.TemplateFile = theTemplate;

				cmsAdminContent = c;

				GetSetUserEditStateAsEmpty();

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				_logger.LogError(ex, "UpdatePageTemplate");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string ValidateBlogFolders([FromBody] ApiModel model) {
			try {
				string folderPath = ContentPageHelper.ScrubSlug(CMSConfigHelper.DecodeBase64(model.FolderPath));
				string categoryPath = ContentPageHelper.ScrubSlug(CMSConfigHelper.DecodeBase64(model.CategoryPath));
				string tagPath = ContentPageHelper.ScrubSlug(CMSConfigHelper.DecodeBase64(model.TagPath));
				string datePath = ContentPageHelper.ScrubSlug(CMSConfigHelper.DecodeBase64(model.DatePath));
				string editorPath = ContentPageHelper.ScrubSlug(CMSConfigHelper.DecodeBase64(model.EditorPath));

				if (string.IsNullOrEmpty(folderPath) || string.IsNullOrEmpty(categoryPath)
					|| string.IsNullOrEmpty(tagPath) || string.IsNullOrEmpty(datePath)
					|| string.IsNullOrEmpty(editorPath)) {
					return JsonSerializer.Serialize(ServiceResponse.Fail);
				}
				if (folderPath.Length < 1 || categoryPath.Length < 1 || tagPath.Length < 1 || datePath.Length < 1 || editorPath.Length < 1) {
					return JsonSerializer.Serialize(ServiceResponse.Fail);
				}

				var lstParms = new List<string>();
				lstParms.Add(categoryPath);
				lstParms.Add(tagPath);
				lstParms.Add(datePath);
				lstParms.Add(editorPath);

				Dictionary<string, int> ct = (from p in lstParms
											  group p by p into g
											  select new KeyValuePair<string, int>(g.Key, g.Count())).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

				bool bDuplicate = ct.Where(x => x.Value > 1).Any();

				if (SiteData.CurrentSiteExists && !bDuplicate) {
					var exists = pageHelper.ExistingPagesBeginWith(SiteData.CurrentSite.SiteID, folderPath);

					if (!exists) {
						return JsonSerializer.Serialize(ServiceResponse.OK);
					}
				}

				if (!SiteData.CurrentSiteExists) {
					return JsonSerializer.Serialize(ServiceResponse.OK);
				}

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			} catch (Exception ex) {
				_logger.LogError(ex, "ValidateBlogFolders");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpGet]
		public string FindUsers(string searchTerm) {
			string search = CMSConfigHelper.DecodeBase64(searchTerm);

			List<UserProfile> lstUsers = SecurityData.GetUserProfileSearch(search);

			return JsonSerializer.Serialize(lstUsers.OrderBy(x => x.UserName).ToList());
		}

		[HttpGet]
		public string FindCreditUsers(string searchTerm) {
			string search = CMSConfigHelper.DecodeBase64(searchTerm);

			List<UserProfile> lstUsers = SecurityData.GetCreditUserProfileSearch(search);

			return JsonSerializer.Serialize(lstUsers.OrderBy(x => x.UserName).ToList());
		}

		[HttpGet]
		public string ValidateUniqueCategory(string TheSlug, string ItemID) {
			try {
				Guid CurrentItemGuid = new Guid(ItemID);
				var theSlug = CMSConfigHelper.DecodeBase64(TheSlug);
				theSlug = ContentPageHelper.ScrubSlug(theSlug);

				int iCount = ContentCategory.GetSimilar(SiteData.CurrentSite.SiteID, CurrentItemGuid, theSlug);

				if (iCount < 1) {
					return JsonSerializer.Serialize(ServiceResponse.OK);
				}

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			} catch (Exception ex) {
				_logger.LogError(ex, "ValidateUniqueCategory");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpGet]
		public string ValidateUniqueTag(string TheSlug, string ItemID) {
			try {
				Guid CurrentItemGuid = new Guid(ItemID);
				var theSlug = CMSConfigHelper.DecodeBase64(TheSlug);
				theSlug = ContentPageHelper.ScrubSlug(theSlug);

				int iCount = ContentTag.GetSimilar(SiteData.CurrentSite.SiteID, CurrentItemGuid, theSlug);

				if (iCount < 1) {
					return JsonSerializer.Serialize(ServiceResponse.OK);
				}

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			} catch (Exception ex) {
				_logger.LogError(ex, "ValidateUniqueTag");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string RecordSnippetHeartbeat([FromBody] ApiModel model) {
			try {
				Guid CurrentItemGuid = new Guid(model.ItemID);

				ContentSnippet item = GetSnippet(CurrentItemGuid);
				bool ret = false;

				if (item != null && !item.IsLocked) {
					ret = item.RecordSnippetLock(SecurityData.CurrentUserGuid);
				}

				if (ret) {
					return JsonSerializer.Serialize(SiteData.CurrentSite.Now.ToString());
				} else {
					return JsonSerializer.Serialize(Convert.ToDateTime("12/31/1899").ToString());
				}
			} catch (Exception ex) {
				_logger.LogError(ex, "RecordSnippetHeartbeat");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(DateTime.MinValue.ToString());
			}
		}

		[HttpPost]
		public string CancelSnippetEditing([FromBody] ApiModel model) {
			try {
				Guid currentItemGuid = new Guid(model.ItemID);
				ContentSnippet item = GetSnippet(currentItemGuid);

				if (item != null && !item.IsLocked) {
					item.ResetHeartbeatLock();
				}

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				_logger.LogError(ex, "CancelSnippetEditing");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		private ContentSnippet GetSnippet(Guid snippetID) {
			ContentSnippet item = ContentSnippet.Get(snippetID);

			if (item == null) {
				item = ContentSnippet.GetVersion(snippetID);
			}

			return item;
		}

		[HttpGet]
		public string ValidateUniqueSnippet(string TheSlug, string ItemID) {
			try {
				Guid currentItemGuid = new Guid(ItemID);
				var theSlug = CMSConfigHelper.DecodeBase64(TheSlug);
				theSlug = ContentPageHelper.ScrubSlug(theSlug);

				ContentSnippet item = GetSnippet(currentItemGuid);

				if (item != null) {
					currentItemGuid = item.Root_ContentSnippetID;
				}

				int count = ContentSnippet.GetSimilar(SiteData.CurrentSite.SiteID, currentItemGuid, theSlug);

				if (count < 1) {
					return JsonSerializer.Serialize(ServiceResponse.OK);
				}

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			} catch (Exception ex) {
				_logger.LogError(ex, "ValidateUniqueSnippet");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string GetSnippetVersionText(string DBKey) {
			try {
				Guid guidSnippet = new Guid(DBKey);

				ContentSnippet cs = ContentSnippet.GetVersion(guidSnippet);

				if (cs != null) {
					return ReturnLongString(cs.ContentBody);
				}

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				_logger.LogError(ex, "GetSnippetVersionText");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			}
		}

		[HttpPost]
		public string GenerateNewFilename([FromBody] ApiModel model) {
			try {
				string thePageTitle = model.ThePageTitle;
				string goLiveDateString = model.GoLiveDate;
				string pageID = model.PageID;
				string mode = model.Mode;

				currentPageGuid = new Guid(pageID);
				DateTime goLiveDate = Convert.ToDateTime(goLiveDateString);
				string sThePageTitle = CMSConfigHelper.DecodeBase64(thePageTitle);
				var pageType = mode.ToLowerInvariant() == "page" ? ContentPageType.PageType.ContentEntry : ContentPageType.PageType.BlogEntry;

				return JsonSerializer.Serialize(SiteData.GenerateNewFilename(currentPageGuid, sThePageTitle, goLiveDate, pageType));
			} catch (Exception ex) {
				_logger.LogError(ex, "GenerateNewFilename");
				SiteData.WriteDebugException("webservice", ex);
				return JsonSerializer.Serialize(ServiceResponse.Fail);
			}
		}

		[HttpGet]
		public string ValidateUniqueFilename(string TheFileName, string PageID) {
			try {
				currentPageGuid = new Guid(PageID);
				TheFileName = CMSConfigHelper.DecodeBase64(TheFileName);

				return JsonSerializer.Serialize(IsUniqueFilename(TheFileName, currentPageGuid));
			} catch (Exception ex) {
				_logger.LogError(ex, "ValidateUniqueFilename");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		protected string IsUniqueFilename(string theFileName, Guid pageId) {
			try {
				var ret = SiteData.IsUniqueFilename(theFileName, pageId);

				return ret ? ServiceResponse.OK : ServiceResponse.Fail;
			} catch (Exception ex) {
				_logger.LogError(ex, "IsUniqueFilename");
				SiteData.WriteDebugException("webservice", ex);

				return ex.ToString();
			}
		}

		[HttpGet]
		public string GenerateBlogFilePrefix(string ThePageSlug, string GoLiveDate) {
			try {
				DateTime goLiveDate = Convert.ToDateTime(GoLiveDate);
				ThePageSlug = CMSConfigHelper.DecodeBase64(ThePageSlug);

				return JsonSerializer.Serialize(ContentPageHelper.CreateFileNameFromSlug(SiteData.CurrentSite, goLiveDate, ThePageSlug));
			} catch (Exception ex) {
				_logger.LogError(ex, "GenerateBlogFilePrefix");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			}
		}

		[HttpGet]
		public string ValidateUniqueBlogFilename(string ThePageSlug, string GoLiveDate, string PageID) {
			try {
				currentPageGuid = new Guid(PageID);
				DateTime dateGoLive = Convert.ToDateTime(GoLiveDate);
				ThePageSlug = CMSConfigHelper.DecodeBase64(ThePageSlug);

				return JsonSerializer.Serialize(IsUniqueBlogFilename(ThePageSlug, dateGoLive, currentPageGuid));
			} catch (Exception ex) {
				_logger.LogError(ex, "ValidateUniqueBlogFilename");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			}
		}

		protected string IsUniqueBlogFilename(string pageSlug, DateTime dateGoLive, Guid pageId) {
			try {
				var ret = SiteData.IsUniqueBlogFilename(pageSlug, dateGoLive, pageId);

				return ret ? ServiceResponse.OK : ServiceResponse.Fail;
			} catch (Exception ex) {
				_logger.LogError(ex, "IsUniqueBlogFilename");
				SiteData.WriteDebugException("webservice", ex);

				return ex.ToString();
			}
		}

		[HttpGet]
		public string GenerateCategoryTagSlug(string TheSlug, string ItemID, string Mode) {
			try {
				Guid currentItemGuid = new Guid(ItemID);
				var theSlug = CMSConfigHelper.DecodeBase64(TheSlug);
				theSlug = ContentPageHelper.ScrubSlug(theSlug).ToLowerInvariant();
				var matches = 0;
				var count = 0;
				var siteid = SiteData.CurrentSite.SiteID;

				if (Mode.ToLowerInvariant() == "category") {
					matches = ContentCategory.GetSimilar(siteid, currentItemGuid, theSlug);
					if (matches > 0) {
						count = 1;
						while (count < 2000 && matches > 0) {
							theSlug = string.Format("{0}-{1}", theSlug, count);
							matches = ContentCategory.GetSimilar(siteid, currentItemGuid, theSlug);
						}
					}
				}
				if (Mode.ToLowerInvariant() == "tag") {
					matches = ContentTag.GetSimilar(siteid, currentItemGuid, theSlug);
					if (matches > 0) {
						count = 1;
						while (count < 2000 && matches > 0) {
							theSlug = string.Format("{0}-{1}", theSlug, count);
							matches = ContentTag.GetSimilar(siteid, currentItemGuid, theSlug);
						}
					}
				}

				return JsonSerializer.Serialize(ContentPageHelper.ScrubSlug(theSlug));
			} catch (Exception ex) {
				_logger.LogError(ex, "GenerateCategoryTagSlug");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			}
		}

		[HttpPost]
		public string GenerateSnippetSlug([FromBody] ApiModel model) {
			try {
				var theSlug = model.TheSlug;
				theSlug = CMSConfigHelper.DecodeBase64(theSlug).ToLowerInvariant().Trim();

				return JsonSerializer.Serialize(ContentPageHelper.ScrubSlug(theSlug));
			} catch (Exception ex) {
				_logger.LogError(ex, "GenerateSnippetSlug");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			}
		}

		[HttpPost]
		public string MoveWidgetToNewZone([FromBody] ApiModel model) {
			try {
				string widgetTarget = model.WidgetTarget;
				string widgetDropped = model.WidgetDropped;
				string thisPage = model.ThisPage;

				currentPageGuid = new Guid(thisPage);
				LoadGuids();
				string[] w = widgetDropped.Split('\t');

				Guid guidWidget = Guid.Empty;
				if (w.Length > 2) {
					if (w[2].ToString().Length == Guid.Empty.ToString().Length) {
						guidWidget = new Guid(w[2]);
					}
				} else {
					if (w[0].ToString().Length == Guid.Empty.ToString().Length) {
						guidWidget = new Guid(w[0]);
					}
				}

				List<Widget> cacheWidget = cmsAdminWidget;

				var ww1 = (from w1 in cacheWidget
						   where w1.Root_WidgetID == guidWidget
						   select w1).FirstOrDefault();

				if (ww1 != null) {
					ww1.WidgetOrder = -1;
					ww1.PlaceholderName = widgetTarget;
				}

				List<Widget> ww2 = (from w1 in cacheWidget
									where w1.PlaceholderName.ToLowerInvariant() == widgetTarget.ToLowerInvariant()
									&& w1.WidgetOrder >= 0
									orderby w1.WidgetOrder, w1.EditDate
									select w1).ToList();

				int iW = 1;
				foreach (var w2 in ww2) {
					w2.WidgetOrder = iW++;
				}

				cmsAdminWidget = cacheWidget;
				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				_logger.LogError(ex, "MoveWidgetToNewZone");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string CacheWidgetUpdate([FromBody] ApiModel model) {
			try {
				string widgetAddition = model.WidgetAddition;
				string thisPage = model.ThisPage;

				widgetAddition = CMSConfigHelper.DecodeBase64(widgetAddition);
				currentPageGuid = new Guid(thisPage);
				LoadGuids();

				List<Widget> cacheWidget = cmsAdminWidget;

				List<Widget> inputWid = new List<Widget>();
				Dictionary<Guid, int> dictOrder = new Dictionary<Guid, int>();
				int iW = 0;

				widgetAddition = widgetAddition.Replace("\r\n", "\n");
				widgetAddition = widgetAddition.Replace("\r", "\n");
				string[] arrWidgRows = widgetAddition.Split('\n');

				foreach (string arrWidgCell in arrWidgRows) {
					if (!string.IsNullOrEmpty(arrWidgCell)) {
						bool bGoodWidget = false;
						string[] w = arrWidgCell.Split('\t');

						Widget rWidg = new Widget();
						if (w[2].ToLowerInvariant().EndsWith(".cshtml") || w[2].ToLowerInvariant().EndsWith(".vbhtml")
								|| w[2].ToLowerInvariant().Contains(":") || w[2].ToLowerInvariant().Contains("|")) {
							rWidg.ControlPath = w[2];
							rWidg.Root_WidgetID = Guid.NewGuid();

							DateTime dtSite = CMSConfigHelper.CalcNearestFiveMinTime(SiteData.CurrentSite.Now);
							rWidg.GoLiveDate = dtSite;
							rWidg.RetireDate = dtSite.AddYears(200);

							bGoodWidget = true;
						} else {
							if (w[2].ToString().Length == Guid.Empty.ToString().Length) {
								rWidg.Root_WidgetID = new Guid(w[2]);
								bGoodWidget = true;
							}
						}
						if (bGoodWidget) {
							dictOrder.Add(rWidg.Root_WidgetID, iW);

							rWidg.WidgetDataID = Guid.NewGuid();
							rWidg.IsPendingChange = true;
							rWidg.PlaceholderName = w[1].Substring(4);
							rWidg.WidgetOrder = int.Parse(w[0]);
							rWidg.Root_ContentID = currentPageGuid;
							rWidg.IsWidgetActive = true;
							rWidg.IsLatestVersion = true;
							rWidg.EditDate = SiteData.CurrentSite.Now;
							inputWid.Add(rWidg);
						}
						iW++;
					}
				}

				foreach (Widget wd1 in inputWid) {
					var wd2 = (from d in cacheWidget where d.Root_WidgetID == wd1.Root_WidgetID select d).FirstOrDefault();

					if (wd2 == null) {
						cacheWidget.Add(wd1);
					} else {
						wd2.EditDate = SiteData.CurrentSite.Now;
						wd2.PlaceholderName = wd1.PlaceholderName; // if moving zones

						int i = cacheWidget.IndexOf(wd2);
						cacheWidget[i].WidgetOrder = wd1.WidgetOrder;

						int? mainSort = (from entry in dictOrder
										 where entry.Key == wd1.Root_WidgetID
										 select entry.Value).FirstOrDefault();

						if (mainSort != null) {
							cacheWidget[i].WidgetOrder = Convert.ToInt32(mainSort);
						}
					}
				}

				cmsAdminWidget = cacheWidget;
				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				_logger.LogError(ex, "CacheWidgetUpdate");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string GetWidgetText([FromBody] ApiModel model) {
			try {
				string dbKey = model.DBKey;
				string thisPage = model.ThisPage;

				currentPageGuid = new Guid(thisPage);
				LoadGuids();
				Guid guidWidget = new Guid(dbKey);

				Widget? ww = null;

				try {
					ww = (from w in cmsAdminWidget
						  where w.Root_WidgetID == guidWidget
						  select w).FirstOrDefault();
				} catch (Exception ex) { }

				if (ww == null) {
					ww = widgetHelper.Get(guidWidget);
				}

				if (ww != null) {
					return ReturnLongString(ww.ControlProperties);
				}

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				_logger.LogError(ex, "GetWidgetText");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			}
		}

		[HttpPost]
		public string GetWidgetVersionText([FromBody] ApiModel model) {
			try {
				string dbKey = model.DBKey;
				string thisPage = model.ThisPage;

				currentPageGuid = new Guid(thisPage);
				LoadGuids();
				Guid guidWidget = new Guid(dbKey);

				Widget? ww = null;

				try {
					ww = (from w in cmsAdminWidget
						  where w.WidgetDataID == guidWidget
						  select w).FirstOrDefault();
				} catch (Exception ex) { }

				if (ww == null) {
					ww = widgetHelper.GetWidgetVersion(guidWidget);
				}

				if (ww != null) {
					return ReturnLongString(ww.ControlProperties);
				}

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				_logger.LogError(ex, "GetWidgetVersionText");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			}
		}

		[HttpGet]
		public string GetWidgetLatestText(string DBKey, string ThisPage) {
			try {
				currentPageGuid = new Guid(ThisPage);
				LoadGuids();
				Guid guidWidget = new Guid(DBKey);
				Widget? ww = null;

				try {
					ww = (from w in cmsAdminWidget
						  where w.Root_WidgetID == guidWidget
						  select w).FirstOrDefault();
				} catch (Exception ex) { }

				if (ww == null) {
					ww = widgetHelper.Get(guidWidget);
				}

				if (ww != null) {
					return ReturnLongString(ww.ControlProperties);
				}

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				_logger.LogError(ex, "GetWidgetLatestText");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			}
		}

		[HttpPost]
		public string DeleteWidget([FromBody] ApiModel model) {
			try {
				string dbKey = model.DBKey;
				string thisPage = model.ThisPage;

				currentPageGuid = new Guid(thisPage);
				LoadGuids();
				Guid guidWidget = new Guid(dbKey);

				var cacheWidget = cmsAdminWidget;

				List<Widget> ww = (from w in cacheWidget
								   where w.Root_WidgetID == guidWidget
								   select w).ToList();

				if (ww != null) {
					foreach (var w in ww) {
						w.IsWidgetPendingDelete = true;
						w.IsWidgetActive = false;
						w.EditDate = SiteData.CurrentSite.Now;
					}
				}

				cmsAdminWidget = cacheWidget;

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				_logger.LogError(ex, "DeleteWidget");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string CopyWidget([FromBody] ApiModel model) {
			try {
				string dbKey = model.DBKey;
				string thisPage = model.ThisPage;

				currentPageGuid = new Guid(thisPage);
				LoadGuids();

				Guid guidWidget = new Guid(dbKey);

				List<Widget> cacheWidget = cmsAdminWidget;

				List<Widget> ww = (from w in cacheWidget
								   where w.Root_WidgetID == guidWidget
									&& w.IsLatestVersion == true
								   select w).ToList();

				if (ww != null) {
					foreach (var w in ww) {
						Guid newWidget = Guid.NewGuid();

						Widget wCpy = new Widget {
							Root_ContentID = w.Root_ContentID,
							Root_WidgetID = newWidget,
							WidgetDataID = Guid.NewGuid(),
							PlaceholderName = w.PlaceholderName,
							ControlPath = w.ControlPath,
							ControlProperties = w.ControlProperties,
							IsLatestVersion = true,
							IsPendingChange = true,
							IsWidgetActive = true,
							IsWidgetPendingDelete = false,
							WidgetOrder = w.WidgetOrder,
							GoLiveDate = w.GoLiveDate,
							RetireDate = w.RetireDate,
							EditDate = SiteData.CurrentSite.Now
						};

						cacheWidget.Add(wCpy);
					}
				}

				cmsAdminWidget = cacheWidget;

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				_logger.LogError(ex, "CopyWidget");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string RemoveWidget([FromBody] ApiModel model) {
			try {
				string dbKey = model.DBKey;
				string thisPage = model.ThisPage;

				currentPageGuid = new Guid(thisPage);
				LoadGuids();
				Guid guidWidget = new Guid(dbKey);

				List<Widget> cacheWidget = cmsAdminWidget;

				List<Widget> ww = (from w in cacheWidget
								   where w.Root_WidgetID == guidWidget
								   select w).ToList();

				if (ww != null) {
					foreach (var w in ww) {
						w.IsWidgetActive = false;
						w.EditDate = SiteData.CurrentSite.Now;
					}
				}

				cmsAdminWidget = cacheWidget;

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				_logger.LogError(ex, "RemoveWidget");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string ActivateWidget([FromBody] ApiModel model) {
			try {
				string dbKey = model.DBKey;
				string thisPage = model.ThisPage;

				currentPageGuid = new Guid(thisPage);
				LoadGuids();
				Guid guidWidget = new Guid(dbKey);

				List<Widget> cacheWidget = cmsAdminWidget;

				List<Widget> ww = (from w in cacheWidget
								   where w.Root_WidgetID == guidWidget
								   select w).ToList();

				if (ww != null) {
					foreach (var w in ww) {
						w.IsWidgetActive = true;
						w.EditDate = SiteData.CurrentSite.Now;
					}
				}

				cmsAdminWidget = cacheWidget;

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				_logger.LogError(ex, "ActivateWidget");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string CacheGenericContent([FromBody] ApiModel model) {
			try {
				string zoneText = model.ZoneText;
				string dbKey = model.DBKey;
				string thisPage = model.ThisPage;

				zoneText = CMSConfigHelper.DecodeBase64(zoneText);
				currentPageGuid = new Guid(thisPage);
				LoadGuids();
				Guid guidWidget = new Guid(dbKey);

				List<Widget> cacheWidget = cmsAdminWidget;

				Widget? c = (from w in cacheWidget
							 where w.Root_WidgetID == guidWidget
							 select w).FirstOrDefault();

				c.ControlProperties = zoneText;
				c.EditDate = SiteData.CurrentSite.Now;

				cmsAdminWidget = cacheWidget;

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				_logger.LogError(ex, "CacheGenericContent");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string CacheContentZoneText([FromBody] ApiModel model) {
			try {
				string zoneText = model.ZoneText;
				string zone = model.Zone;
				string thisPage = model.ThisPage;

				zoneText = CMSConfigHelper.DecodeBase64(zoneText);
				zone = CMSConfigHelper.DecodeBase64(zone);
				currentPageGuid = new Guid(thisPage);
				LoadGuids();
				CurrentEditPage = filePage.FileName.ToLowerInvariant();

				var c = cmsAdminContent;
				c.EditDate = SiteData.CurrentSite.Now;
				c.EditUserId = SecurityData.CurrentUserGuid;
				c.ContentID = Guid.NewGuid();

				if (zone.ToLowerInvariant() == "c")
					c.PageText = zoneText;

				if (zone.ToLowerInvariant() == "l")
					c.LeftPageText = zoneText;

				if (zone.ToLowerInvariant() == "r")
					c.RightPageText = zoneText;

				cmsAdminContent = c;

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				_logger.LogError(ex, "CacheContentZoneText");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string PublishChanges([FromBody] ApiModel model) {
			try {
				string thisPage = model.ThisPage;

				currentPageGuid = new Guid(thisPage);
				LoadGuids();
				CurrentEditPage = filePage.FileName.ToLowerInvariant();

				bool isLocked = pageHelper.IsPageLocked(currentPageGuid, SiteData.CurrentSite.SiteID, SecurityData.CurrentUserGuid);
				Guid guidUser = pageHelper.GetCurrentEditUser(currentPageGuid, SiteData.CurrentSite.SiteID);

				if (isLocked || guidUser != SecurityData.CurrentUserGuid) {
					return "Cannot publish changes, not current editing user.";
				}

				List<Widget> pageWidgets = widgetHelper.GetWidgets(currentPageGuid, true);

				if (cmsAdminContent != null) {
					ContentPage oldContent = pageHelper.FindContentByID(SiteData.CurrentSiteID, currentPageGuid);

					ContentPage newContent = cmsAdminContent;
					newContent.ContentID = Guid.NewGuid();
					newContent.NavOrder = oldContent.NavOrder;
					newContent.Parent_ContentID = oldContent.Parent_ContentID;
					newContent.EditUserId = SecurityData.CurrentUserGuid;
					newContent.EditDate = SiteData.CurrentSite.Now;

					foreach (var wd in cmsAdminWidget) {
						wd.EditDate = SiteData.CurrentSite.Now;
						wd.Save();
					}

					newContent.SavePageEdit();

					if (newContent.ContentType == ContentPageType.PageType.BlogEntry) {
						pageHelper.ResolveDuplicateBlogURLs(newContent.SiteID);
					}

					cmsAdminWidget = new List<Widget>();
					cmsAdminContent = null;
				}

				GetSetUserEditStateAsEmpty();

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				_logger.LogError(ex, "PublishChanges");
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		protected string ReturnLongString(string jsonText) {
			if (string.IsNullOrEmpty(jsonText)) {
				return JsonSerializer.Serialize("No Data");
			} else {
				if (jsonText.Length < 768) {
					return JsonSerializer.Serialize(jsonText);
				} else {
					return JsonSerializer.Serialize(jsonText.Substring(0, 700) + "[.....]");
				}
			}
		}

		public void Dispose() {
			if (pageHelper != null) {
				pageHelper.Dispose();
			}
			if (widgetHelper != null) {
				widgetHelper.Dispose();
			}
			if (sitemapHelper != null) {
				sitemapHelper.Dispose();
			}
		}
	}
}