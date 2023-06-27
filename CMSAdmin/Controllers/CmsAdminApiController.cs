using Carrotware.CMS.Core;
using Carrotware.CMS.CoreMVC.UI.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Xml.Serialization;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
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

		public CmsAdminApiController() { }

		protected ContentPageHelper pageHelper = new ContentPageHelper();
		protected WidgetHelper widgetHelper = new WidgetHelper();
		protected SiteMapOrderHelper sitemapHelper = new SiteMapOrderHelper();

		protected Guid CurrentPageGuid = Guid.Empty;
		protected ContentPage filePage = null;

		public ContentPage cmsAdminContent {
			get {
				ContentPage c = null;
				try {
					string sXML = GetSerialized(CMSConfigHelper.keyAdminContent);
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(ContentPage));
					object genpref = null;
					using (StringReader stringReader = new StringReader(sXML)) {
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
					string sXML = string.Empty;
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(ContentPage));
					using (StringWriter stringWriter = new StringWriter()) {
						xmlSerializer.Serialize(stringWriter, value);
						sXML = stringWriter.ToString();
					}
					SaveSerialized(CMSConfigHelper.keyAdminContent, sXML);
				}
			}
		}

		public List<Widget> cmsAdminWidget {
			get {
				List<Widget> c = null;
				string sXML = GetSerialized(CMSConfigHelper.keyAdminWidget);
				//since a page may not have any widgets, initialize it and skip deserializing
				if (!string.IsNullOrEmpty(sXML)) {
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Widget>));
					object genpref = null;
					using (StringReader stringReader = new StringReader(sXML)) {
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
					string sXML = string.Empty;
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Widget>));
					using (StringWriter stringWriter = new StringWriter()) {
						xmlSerializer.Serialize(stringWriter, value);
						sXML = stringWriter.ToString();
					}
					SaveSerialized(CMSConfigHelper.keyAdminWidget, sXML);
				}
			}
		}

		private void SaveSerialized(string sKey, string sData) {
			LoadGuids();

			CMSConfigHelper.SaveSerialized(CurrentPageGuid, sKey, sData);
		}

		private string GetSerialized(string sKey) {
			string sData = string.Empty;
			LoadGuids();

			sData = CMSConfigHelper.GetSerialized(CurrentPageGuid, sKey);

			return sData;
		}

		private bool ClearSerialized(string sKey) {
			LoadGuids();

			return CMSConfigHelper.ClearSerialized(CurrentPageGuid, sKey);
		}

		private void LoadGuids() {
			using (ContentPageHelper pageHelper = new ContentPageHelper()) {
				if (!string.IsNullOrEmpty(CurrentEditPage)) {
					filePage = pageHelper.FindByFilename(SiteData.CurrentSite.SiteID, CurrentEditPage);
					if (filePage != null) {
						CurrentPageGuid = filePage.Root_ContentID;
					}
				} else {
					if (CurrentPageGuid != Guid.Empty) {
						filePage = pageHelper.FindContentByID(SiteData.CurrentSite.SiteID, CurrentPageGuid);
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
				string PageID = model.PageID;

				CurrentPageGuid = new Guid(PageID);

				bool bRet = pageHelper.RecordPageLock(CurrentPageGuid, SiteData.CurrentSite.SiteID, SecurityData.CurrentUserGuid);

				if (bRet) {
					return JsonSerializer.Serialize(SiteData.CurrentSite.Now.ToString());
				} else {
					return JsonSerializer.Serialize(Convert.ToDateTime("12/31/1899").ToString());
				}
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(DateTime.MinValue.ToString());
			}
		}

		[HttpPost]
		public string CancelEditing([FromBody] ApiModel model) {
			try {
				string ThisPage = model.ThisPage;

				CurrentPageGuid = new Guid(ThisPage);

				pageHelper.ResetHeartbeatLock(CurrentPageGuid, SiteData.CurrentSite.SiteID, SecurityData.CurrentUserGuid);

				GetSetUserEditStateAsEmpty();

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string RecordEditorPosition([FromBody] ApiModel model) {
			try {
				string ToolbarState = model.ToolbarState;
				string ToolbarMargin = model.ToolbarMargin;
				string ToolbarScroll = model.ToolbarScroll;
				string WidgetScroll = model.WidgetScroll;
				string SelTabID = model.SelTabID;

				GetSetUserEditState(ToolbarState, ToolbarMargin, ToolbarScroll, WidgetScroll, SelTabID);

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		private void GetSetUserEditStateAsEmpty() {
			GetSetUserEditState(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
		}

		private void GetSetUserEditState(string ToolbarState, string ToolbarMargin, string ToolbarScroll, string WidgetScroll, string SelTabID) {
			UserEditState editor = UserEditState.cmsUserEditState;

			if (editor == null) {
				editor = new UserEditState();
				editor.Init();
			}

			editor.EditorMargin = string.IsNullOrEmpty(ToolbarMargin) ? "L" : ToolbarMargin.ToUpperInvariant();
			editor.EditorOpen = string.IsNullOrEmpty(ToolbarState) ? "true" : ToolbarState.ToLowerInvariant();
			editor.EditorWidgetScrollPosition = string.IsNullOrEmpty(WidgetScroll) ? "0" : WidgetScroll.ToLowerInvariant();
			editor.EditorScrollPosition = string.IsNullOrEmpty(ToolbarScroll) ? "0" : ToolbarScroll.ToLowerInvariant();
			editor.EditorSelectedTabIdx = string.IsNullOrEmpty(SelTabID) ? "0" : SelTabID.ToLowerInvariant();

			if (string.IsNullOrEmpty(ToolbarMargin) && string.IsNullOrEmpty(ToolbarState)) {
				UserEditState.cmsUserEditState = null;
			} else {
				UserEditState.cmsUserEditState = editor;
			}
		}

		[HttpGet]
		public string GetChildPages(string PageID, string CurrPageID) {
			Guid? ParentID = Guid.Empty;
			if (!string.IsNullOrEmpty(PageID)) {
				if (PageID.Length > 20) {
					ParentID = new Guid(PageID);
				}
			}

			Guid ContPageID = Guid.Empty;
			if (!string.IsNullOrEmpty(CurrPageID)) {
				if (CurrPageID.Length > 20) {
					ContPageID = new Guid(CurrPageID);
				}
			}

			List<SiteMapOrder> lstSiteMap = new List<SiteMapOrder>();

			try {
				if (SiteData.CurrentSiteExists) {
					List<SiteMapOrder> lst = sitemapHelper.GetChildPages(SiteData.CurrentSite.SiteID, ParentID, ContPageID);

					lstSiteMap = (from l in lst
								  orderby l.NavOrder, l.NavMenuText
								  where l.Parent_ContentID != ContPageID || l.Parent_ContentID == null
								  select l).ToList();
				}
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);

				throw;
			}

			return JsonSerializer.Serialize(lstSiteMap);
		}

		[HttpGet]
		public string GetPageCrumbs(string PageID, string CurrPageID) {
			Guid? ContentPageID = Guid.Empty;
			if (!string.IsNullOrEmpty(PageID)) {
				if (PageID.Length > 20) {
					ContentPageID = new Guid(PageID);
				}
			}

			Guid ContPageID = Guid.Empty;
			if (!string.IsNullOrEmpty(CurrPageID)) {
				if (CurrPageID.Length > 20) {
					ContPageID = new Guid(CurrPageID);
				}
			}

			List<SiteMapOrder> lstSiteMap = new List<SiteMapOrder>();

			int iLevel = 0;

			int iLenB = 0;
			int iLenA = 1;

			try {
				while (iLenB < iLenA && SiteData.CurrentSiteExists) {
					iLenB = lstSiteMap.Count;

					SiteMapOrder cont = sitemapHelper.GetPageWithLevel(SiteData.CurrentSite.SiteID, ContentPageID, iLevel);

					iLevel++;
					if (cont != null) {
						ContentPageID = cont.Parent_ContentID;
						lstSiteMap.Add(cont);
					}

					iLenA = lstSiteMap.Count;
				}
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);

				throw;
			}

			return JsonSerializer.Serialize(lstSiteMap.OrderByDescending(y => y.NavLevel).ToList());
		}

		[HttpPost]
		public string UpdatePageTemplate([FromBody] ApiModel model) {
			try {
				string TheTemplate = model.TheTemplate;
				string ThisPage = model.ThisPage;

				TheTemplate = CMSConfigHelper.DecodeBase64(TheTemplate);
				CurrentPageGuid = new Guid(ThisPage);
				LoadGuids();

				ContentPage c = cmsAdminContent;

				c.TemplateFile = TheTemplate;

				cmsAdminContent = c;

				GetSetUserEditStateAsEmpty();

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string ValidateBlogFolders(string FolderPath, string DatePath, string CategoryPath, string TagPath, string EditorPath) {
			try {
				string sFolderPath = ContentPageHelper.ScrubSlug(CMSConfigHelper.DecodeBase64(FolderPath));
				string sCategoryPath = ContentPageHelper.ScrubSlug(CMSConfigHelper.DecodeBase64(CategoryPath));
				string sTagPath = ContentPageHelper.ScrubSlug(CMSConfigHelper.DecodeBase64(TagPath));
				string sDatePath = ContentPageHelper.ScrubSlug(CMSConfigHelper.DecodeBase64(DatePath));
				string sEditorPath = ContentPageHelper.ScrubSlug(CMSConfigHelper.DecodeBase64(EditorPath));

				if (string.IsNullOrEmpty(sFolderPath) || string.IsNullOrEmpty(sCategoryPath)
					|| string.IsNullOrEmpty(sTagPath) || string.IsNullOrEmpty(sDatePath)
					|| string.IsNullOrEmpty(sEditorPath)) {
					return JsonSerializer.Serialize(ServiceResponse.Fail);
				}
				if (sFolderPath.Length < 1 || sCategoryPath.Length < 1 || sTagPath.Length < 1 || sDatePath.Length < 1 || sEditorPath.Length < 1) {
					return JsonSerializer.Serialize(ServiceResponse.Fail);
				}

				List<string> lstParms = new List<string>();
				lstParms.Add(sCategoryPath);
				lstParms.Add(sTagPath);
				lstParms.Add(sDatePath);
				lstParms.Add(sEditorPath);

				Dictionary<string, int> ct = (from p in lstParms
											  group p by p into g
											  select new KeyValuePair<string, int>(g.Key, g.Count())).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

				bool bDuplicate = ct.Where(x => x.Value > 1).Any();

				if (SiteData.CurrentSiteExists && !bDuplicate) {
					var exists = pageHelper.ExistingPagesBeginWith(SiteData.CurrentSite.SiteID, sFolderPath);

					if (!exists) {
						return JsonSerializer.Serialize(ServiceResponse.OK);
					}
				}

				if (!SiteData.CurrentSiteExists) {
					return JsonSerializer.Serialize(ServiceResponse.OK);
				}

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			} catch (Exception ex) {
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
				TheSlug = CMSConfigHelper.DecodeBase64(TheSlug);
				TheSlug = ContentPageHelper.ScrubSlug(TheSlug);

				int iCount = ContentCategory.GetSimilar(SiteData.CurrentSite.SiteID, CurrentItemGuid, TheSlug);

				if (iCount < 1) {
					return JsonSerializer.Serialize(ServiceResponse.OK);
				}

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpGet]
		public string ValidateUniqueTag(string TheSlug, string ItemID) {
			try {
				Guid CurrentItemGuid = new Guid(ItemID);
				TheSlug = CMSConfigHelper.DecodeBase64(TheSlug);
				TheSlug = ContentPageHelper.ScrubSlug(TheSlug);

				int iCount = ContentTag.GetSimilar(SiteData.CurrentSite.SiteID, CurrentItemGuid, TheSlug);

				if (iCount < 1) {
					return JsonSerializer.Serialize(ServiceResponse.OK);
				}

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string RecordSnippetHeartbeat(string ItemID) {
			try {
				Guid CurrentItemGuid = new Guid(ItemID);

				ContentSnippet item = GetSnippet(CurrentItemGuid);
				bool bRet = false;

				if (item != null && !item.IsLocked) {
					bRet = item.RecordSnippetLock(SecurityData.CurrentUserGuid);
				}

				if (bRet) {
					return JsonSerializer.Serialize(SiteData.CurrentSite.Now.ToString());
				} else {
					return JsonSerializer.Serialize(Convert.ToDateTime("12/31/1899").ToString());
				}
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(DateTime.MinValue.ToString());
			}
		}

		[HttpPost]
		public string CancelSnippetEditing(string ItemID) {
			try {
				Guid CurrentItemGuid = new Guid(ItemID);
				ContentSnippet item = GetSnippet(CurrentItemGuid);

				if (item != null && !item.IsLocked) {
					item.ResetHeartbeatLock();
				}

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
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

		[HttpPost]
		public string ValidateUniqueSnippet(string TheSlug, string ItemID) {
			try {
				Guid CurrentItemGuid = new Guid(ItemID);
				TheSlug = CMSConfigHelper.DecodeBase64(TheSlug);
				TheSlug = ContentPageHelper.ScrubSlug(TheSlug);

				ContentSnippet item = GetSnippet(CurrentItemGuid);

				if (item != null) {
					CurrentItemGuid = item.Root_ContentSnippetID;
				}

				int iCount = ContentSnippet.GetSimilar(SiteData.CurrentSite.SiteID, CurrentItemGuid, TheSlug);

				if (iCount < 1) {
					return JsonSerializer.Serialize(ServiceResponse.OK);
				}

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			} catch (Exception ex) {
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
					if (string.IsNullOrEmpty(cs.ContentBody)) {
						return "No Data";
					} else {
						if (cs.ContentBody.Length < 768) {
							return cs.ContentBody;
						} else {
							return cs.ContentBody.Substring(0, 700) + "[.....]";
						}
					}
				}

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			}
		}

		[HttpPost]
		public string GenerateNewFilename([FromBody] ApiModel model) {
			try {
				string ThePageTitle = model.ThePageTitle;
				string GoLiveDate = model.GoLiveDate;
				string PageID = model.PageID;
				string Mode = model.Mode;

				CurrentPageGuid = new Guid(PageID);
				DateTime goLiveDate = Convert.ToDateTime(GoLiveDate);
				string sThePageTitle = CMSConfigHelper.DecodeBase64(ThePageTitle);
				var pageType = Mode.ToLowerInvariant() == "page" ? ContentPageType.PageType.ContentEntry : ContentPageType.PageType.BlogEntry;

				return JsonSerializer.Serialize(SiteData.GenerateNewFilename(CurrentPageGuid, sThePageTitle, goLiveDate, pageType));
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);
				return JsonSerializer.Serialize(ServiceResponse.Fail);
			}
		}

		[HttpGet]
		public string ValidateUniqueFilename(string TheFileName, string PageID) {
			try {
				CurrentPageGuid = new Guid(PageID);
				TheFileName = CMSConfigHelper.DecodeBase64(TheFileName);

				return JsonSerializer.Serialize(IsUniqueFilename(TheFileName, CurrentPageGuid));
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		protected string IsUniqueFilename(string theFileName, Guid pageId) {
			try {
				var ret = SiteData.IsUniqueFilename(theFileName, pageId);

				return ret ? ServiceResponse.OK : ServiceResponse.Fail;
			} catch (Exception ex) {
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
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			}
		}

		[HttpGet]
		public string ValidateUniqueBlogFilename(string ThePageSlug, string GoLiveDate, string PageID) {
			try {
				CurrentPageGuid = new Guid(PageID);
				DateTime dateGoLive = Convert.ToDateTime(GoLiveDate);
				ThePageSlug = CMSConfigHelper.DecodeBase64(ThePageSlug);

				return JsonSerializer.Serialize(IsUniqueBlogFilename(ThePageSlug, dateGoLive, CurrentPageGuid));
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			}
		}

		protected string IsUniqueBlogFilename(string pageSlug, DateTime dateGoLive, Guid pageId) {
			try {
				var ret = SiteData.IsUniqueBlogFilename(pageSlug, dateGoLive, pageId);

				return ret ? ServiceResponse.OK : ServiceResponse.Fail;
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);

				return ex.ToString();
			}
		}

		[HttpGet]
		public string GenerateCategoryTagSlug(string TheSlug, string ItemID, string Mode) {
			try {
				Guid CurrentItemGuid = new Guid(ItemID);
				TheSlug = CMSConfigHelper.DecodeBase64(TheSlug);
				TheSlug = ContentPageHelper.ScrubSlug(TheSlug).ToLowerInvariant();
				var matches = 0;
				var count = 0;
				var siteid = SiteData.CurrentSite.SiteID;

				if (Mode.ToLowerInvariant() == "category") {
					matches = ContentCategory.GetSimilar(siteid, CurrentItemGuid, TheSlug);
					if (matches > 0) {
						count = 1;
						while (count < 2000 && matches > 0) {
							TheSlug = string.Format("{0}-{1}", TheSlug, count);
							matches = ContentCategory.GetSimilar(siteid, CurrentItemGuid, TheSlug);
						}
					}
				}
				if (Mode.ToLowerInvariant() == "tag") {
					matches = ContentTag.GetSimilar(siteid, CurrentItemGuid, TheSlug);
					if (matches > 0) {
						count = 1;
						while (count < 2000 && matches > 0) {
							TheSlug = string.Format("{0}-{1}", TheSlug, count);
							matches = ContentTag.GetSimilar(siteid, CurrentItemGuid, TheSlug);
						}
					}
				}

				return JsonSerializer.Serialize(ContentPageHelper.ScrubSlug(TheSlug));
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			}
		}

		[HttpPost]
		public string GenerateSnippetSlug(string TheSlug) {
			try {
				TheSlug = CMSConfigHelper.DecodeBase64(TheSlug).ToLowerInvariant().Trim();

				return JsonSerializer.Serialize(ContentPageHelper.ScrubSlug(TheSlug));
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			}
		}

		[HttpPost]
		public string MoveWidgetToNewZone([FromBody] ApiModel model) {
			try {
				string WidgetTarget = model.WidgetTarget;
				string WidgetDropped = model.WidgetDropped;
				string ThisPage = model.ThisPage;

				CurrentPageGuid = new Guid(ThisPage);
				LoadGuids();
				string[] w = WidgetDropped.Split('\t');

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

				Widget ww1 = (from w1 in cacheWidget
							  where w1.Root_WidgetID == guidWidget
							  select w1).FirstOrDefault();

				if (ww1 != null) {
					ww1.WidgetOrder = -1;
					ww1.PlaceholderName = WidgetTarget;
				}

				List<Widget> ww2 = (from w1 in cacheWidget
									where w1.PlaceholderName.ToLowerInvariant() == WidgetTarget.ToLowerInvariant()
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
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string CacheWidgetUpdate([FromBody] ApiModel model) {
			try {
				string WidgetAddition = model.WidgetAddition;
				string ThisPage = model.ThisPage;

				WidgetAddition = CMSConfigHelper.DecodeBase64(WidgetAddition);
				CurrentPageGuid = new Guid(ThisPage);
				LoadGuids();

				List<Widget> cacheWidget = cmsAdminWidget;

				List<Widget> inputWid = new List<Widget>();
				Dictionary<Guid, int> dictOrder = new Dictionary<Guid, int>();
				int iW = 0;

				WidgetAddition = WidgetAddition.Replace("\r\n", "\n");
				WidgetAddition = WidgetAddition.Replace("\r", "\n");
				string[] arrWidgRows = WidgetAddition.Split('\n');

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
							rWidg.Root_ContentID = CurrentPageGuid;
							rWidg.IsWidgetActive = true;
							rWidg.IsLatestVersion = true;
							rWidg.EditDate = SiteData.CurrentSite.Now;
							inputWid.Add(rWidg);
						}
						iW++;
					}
				}

				foreach (Widget wd1 in inputWid) {
					Widget wd2 = (from d in cacheWidget where d.Root_WidgetID == wd1.Root_WidgetID select d).FirstOrDefault();

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
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string GetWidgetText([FromBody] ApiModel model) {
			try {
				string DBKey = model.DBKey;
				string ThisPage = model.ThisPage;

				CurrentPageGuid = new Guid(ThisPage);
				LoadGuids();
				Guid guidWidget = new Guid(DBKey);

				Widget ww = null;

				try {
					ww = (from w in cmsAdminWidget
						  where w.Root_WidgetID == guidWidget
						  select w).FirstOrDefault();
				} catch (Exception ex) { }

				if (ww == null) {
					ww = widgetHelper.Get(guidWidget);
				}

				if (ww != null) {
					if (string.IsNullOrEmpty(ww.ControlProperties)) {
						return "No Data";
					} else {
						if (ww.ControlProperties.Length < 768) {
							return ww.ControlProperties;
						} else {
							return ww.ControlProperties.Substring(0, 700) + "[.....]";
						}
					}
				}

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			}
		}

		[HttpPost]
		public string GetWidgetVersionText([FromBody] ApiModel model) {
			try {
				string DBKey = model.DBKey;
				string ThisPage = model.ThisPage;

				CurrentPageGuid = new Guid(ThisPage);
				LoadGuids();
				Guid guidWidget = new Guid(DBKey);

				Widget ww = null;

				try {
					ww = (from w in cmsAdminWidget
						  where w.WidgetDataID == guidWidget
						  select w).FirstOrDefault();
				} catch (Exception ex) { }

				if (ww == null) {
					ww = widgetHelper.GetWidgetVersion(guidWidget);
				}

				if (ww != null) {
					if (string.IsNullOrEmpty(ww.ControlProperties)) {
						return "No Data";
					} else {
						if (ww.ControlProperties.Length < 768) {
							return ww.ControlProperties;
						} else {
							return ww.ControlProperties.Substring(0, 700) + "[.....]";
						}
					}
				}

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			}
		}

		[HttpGet]
		public string GetWidgetLatestText(string DBKey, string ThisPage) {
			try {
				CurrentPageGuid = new Guid(ThisPage);
				LoadGuids();
				Guid guidWidget = new Guid(DBKey);
				Widget ww = null;

				try {
					ww = (from w in cmsAdminWidget
						  where w.Root_WidgetID == guidWidget
						  select w).FirstOrDefault();
				} catch (Exception ex) { }

				if (ww == null) {
					ww = widgetHelper.Get(guidWidget);
				}

				if (ww != null) {
					if (string.IsNullOrEmpty(ww.ControlProperties)) {
						return "No Data";
					} else {
						if (ww.ControlProperties.Length < 768) {
							return ww.ControlProperties;
						} else {
							return ww.ControlProperties.Substring(0, 700) + "[.....]";
						}
					}
				}

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ServiceResponse.Fail);
			}
		}

		[HttpPost]
		public string DeleteWidget([FromBody] ApiModel model) {
			try {
				string DBKey = model.DBKey;
				string ThisPage = model.ThisPage;

				CurrentPageGuid = new Guid(ThisPage);
				LoadGuids();
				Guid guidWidget = new Guid(DBKey);

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
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string CopyWidget([FromBody] ApiModel model) {
			try {
				string DBKey = model.DBKey;
				string ThisPage = model.ThisPage;

				CurrentPageGuid = new Guid(ThisPage);
				LoadGuids();

				Guid guidWidget = new Guid(DBKey);

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
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string RemoveWidget([FromBody] ApiModel model) {
			try {
				string DBKey = model.DBKey;
				string ThisPage = model.ThisPage;

				CurrentPageGuid = new Guid(ThisPage);
				LoadGuids();
				Guid guidWidget = new Guid(DBKey);

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
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string ActivateWidget([FromBody] ApiModel model) {
			try {
				string DBKey = model.DBKey;
				string ThisPage = model.ThisPage;

				CurrentPageGuid = new Guid(ThisPage);
				LoadGuids();
				Guid guidWidget = new Guid(DBKey);

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
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string CacheGenericContent([FromBody] ApiModel model) {
			try {
				string ZoneText = model.ZoneText;
				string DBKey = model.DBKey;
				string ThisPage = model.ThisPage;

				ZoneText = CMSConfigHelper.DecodeBase64(ZoneText);
				CurrentPageGuid = new Guid(ThisPage);
				LoadGuids();
				Guid guidWidget = new Guid(DBKey);

				List<Widget> cacheWidget = cmsAdminWidget;

				Widget c = (from w in cacheWidget
							where w.Root_WidgetID == guidWidget
							select w).FirstOrDefault();

				c.ControlProperties = ZoneText;
				c.EditDate = SiteData.CurrentSite.Now;

				cmsAdminWidget = cacheWidget;

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string CacheContentZoneText([FromBody] ApiModel model) {
			try {
				string ZoneText = model.ZoneText;
				string Zone = model.Zone;
				string ThisPage = model.ThisPage;

				ZoneText = CMSConfigHelper.DecodeBase64(ZoneText);
				Zone = CMSConfigHelper.DecodeBase64(Zone);
				CurrentPageGuid = new Guid(ThisPage);
				LoadGuids();
				CurrentEditPage = filePage.FileName.ToLowerInvariant();

				var c = cmsAdminContent;
				c.EditDate = SiteData.CurrentSite.Now;
				c.EditUserId = SecurityData.CurrentUserGuid;
				c.ContentID = Guid.NewGuid();

				if (Zone.ToLowerInvariant() == "c")
					c.PageText = ZoneText;

				if (Zone.ToLowerInvariant() == "l")
					c.LeftPageText = ZoneText;

				if (Zone.ToLowerInvariant() == "r")
					c.RightPageText = ZoneText;

				cmsAdminContent = c;

				return JsonSerializer.Serialize(ServiceResponse.OK);
			} catch (Exception ex) {
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
			}
		}

		[HttpPost]
		public string PublishChanges([FromBody] ApiModel model) {
			try {
				string ThisPage = model.ThisPage;

				CurrentPageGuid = new Guid(ThisPage);
				LoadGuids();
				CurrentEditPage = filePage.FileName.ToLowerInvariant();

				bool bLock = pageHelper.IsPageLocked(CurrentPageGuid, SiteData.CurrentSite.SiteID, SecurityData.CurrentUserGuid);
				Guid guidUser = pageHelper.GetCurrentEditUser(CurrentPageGuid, SiteData.CurrentSite.SiteID);

				if (bLock || guidUser != SecurityData.CurrentUserGuid) {
					return "Cannot publish changes, not current editing user.";
				}

				List<Widget> pageWidgets = widgetHelper.GetWidgets(CurrentPageGuid, true);

				if (cmsAdminContent != null) {
					ContentPage oldContent = pageHelper.FindContentByID(SiteData.CurrentSiteID, CurrentPageGuid);

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
				SiteData.WriteDebugException("webservice", ex);

				return JsonSerializer.Serialize(ex.ToString());
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