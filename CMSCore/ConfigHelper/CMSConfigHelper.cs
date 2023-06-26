using Carrotware.CMS.Data.Models;
using Carrotware.CMS.Interface;
using Carrotware.Web.UI.Components;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Web;
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

namespace Carrotware.CMS.Core {

	public class CMSConfigHelper : IDisposable {
		private CarrotCakeContext db = CarrotCakeContext.Create();

		public CMSConfigHelper() { }

		private enum CMSConfigFileType {
			AdminMod,
			SkinDef,
			PublicCtrl,
			SiteTextWidgets,
		}

		#region IDisposable Members

		public void Dispose() {
			if (db != null) {
				db.Dispose();
			}
		}

		#endregion IDisposable Members

		private static string keyAdminMenuModules = "cms_AdminMenuModules";

		private static string keyAdminToolboxModules = "cms_AdminToolboxModules";

		private static string keyPrimarySite = "cms_PrimarySite";

		private static string keyDynamicSite = "cms_DynamicSite";

		private static string keyTemplateFiles = "cms_TemplateFiles";

		private static string keyTemplates = "cms_Templates";

		private static string keyTxtWidgets = "cms_TxtWidgets";

		private static string keyDynSite = "cms_DynSite_";

		public static string keyAdminContent = "cmsAdminContent";

		public static string keyAdminWidget = "cmsAdminWidget";

		public void ResetConfigs() {
			CarrotHttpHelper.CacheRemove(keyAdminMenuModules);

			CarrotHttpHelper.CacheRemove(keyAdminToolboxModules);

			CarrotHttpHelper.CacheRemove(keyDynamicSite);

			CarrotHttpHelper.CacheRemove(keyPrimarySite);

			CarrotHttpHelper.CacheRemove(keyTemplates);

			CarrotHttpHelper.CacheRemove(keyTxtWidgets);

			CarrotHttpHelper.CacheRemove(keyTemplateFiles);

			string ModuleKey = keyDynSite + DomainName;
			CarrotHttpHelper.CacheRemove(ModuleKey);

			try {
				//VirtualDirectory.RegisterRoutes(true);

				if (SiteData.CurrentSiteExists) {
					SiteData.CurrentSite.LoadTextWidgets();
				}
			} catch (Exception ex) { }

			//if (SiteData.CurrentTrustLevel == AspNetHostingPermissionLevel.Unrestricted) {
			//	HttpRuntime.UnloadAppDomain();
			//}
		}

		public static string DomainName {
			get {
				var domName = CarrotHttpHelper.Current.Request.Host.Value;
				if ((domName.IndexOf(":") > 0) && (domName.EndsWith(":80") || domName.EndsWith(":443"))) {
					domName = domName.Substring(0, domName.IndexOf(":"));
				}

				return domName.ToLowerInvariant();
			}
		}

		public static bool HasAdminModules() {
			using (CMSConfigHelper cmsHelper = new CMSConfigHelper()) {
				return cmsHelper.AdminModules.Any();
			}
		}

		public static FileDataHelper GetFileDataHelper() {
			string fileTypes = null;

			CarrotCakeConfig config = CarrotCakeConfig.GetConfig();
			if (config.FileManagerConfig != null && !string.IsNullOrEmpty(config.FileManagerConfig.BlockedExtensions)) {
				fileTypes = config.FileManagerConfig.BlockedExtensions;
			}

			return new FileDataHelper(fileTypes);
		}

		private static DataSet ReadDataSetConfig(CMSConfigFileType cfg, string sPath) {
			string sPlugCfg = "default.config";
			string sRealPath = CarrotHttpHelper.MapPath(sPath);
			CarrotCakeConfig config = CarrotCakeConfig.GetConfig();

			int iExpectedTblCount = 1;

			switch (cfg) {
				case CMSConfigFileType.SiteTextWidgets:
					sPlugCfg = Path.Join(sRealPath, config.ConfigFileLocation.TextContentProcessors);
					break;

				case CMSConfigFileType.AdminMod:
					sPlugCfg = Path.Join(sRealPath, "Admin.config");
					iExpectedTblCount = 2;
					break;

				case CMSConfigFileType.PublicCtrl:
					sPlugCfg = Path.Join(sRealPath, "Public.config");
					break;

				case CMSConfigFileType.SkinDef:
					sPlugCfg = Path.Join(sRealPath, "Skin.config");
					break;

				default:
					sPlugCfg = Path.Join(sRealPath, "default.config");
					iExpectedTblCount = -1;
					break;
			}

			DataSet ds = new DataSet();
			if (File.Exists(sPlugCfg) && iExpectedTblCount > 0) {
				ds.ReadXml(sPlugCfg);
			}

			if (ds == null) {
				ds = new DataSet();
			}

			int iTblCount = ds.Tables.Count;

			// if dataset has wrong # of tables, build out more tables
			if (iTblCount < iExpectedTblCount) {
				for (int t = iTblCount; t <= iExpectedTblCount; t++) {
					ds.Tables.Add(new DataTable());
					ds.AcceptChanges();
				}
			}

			if (iExpectedTblCount > 0) {
				iTblCount = ds.Tables.Count;

				string table1Name = string.Empty;

				List<string> reqCols0 = new List<string>();
				List<string> reqCols1 = new List<string>();

				switch (cfg) {
					case CMSConfigFileType.AdminMod:
						reqCols0.Add("caption");
						reqCols0.Add("area");
						table1Name = "pluginlist";

						reqCols1.Add("area");
						reqCols1.Add("pluginlabel");
						reqCols1.Add("menuorder");
						reqCols1.Add("action");
						reqCols1.Add("controller");
						reqCols1.Add("usepopup");
						reqCols1.Add("visible");

						break;

					case CMSConfigFileType.PublicCtrl:
						//case CMSConfigFileType.PublicControls:
						reqCols0.Add("filepath");
						reqCols0.Add("crtldesc");
						table1Name = "ctrlfile";

						break;

					case CMSConfigFileType.SkinDef:
						reqCols0.Add("templatefile");
						reqCols0.Add("filedesc");
						table1Name = "pagenames";

						break;

					case CMSConfigFileType.SiteTextWidgets:
						reqCols0.Add("pluginassembly");
						reqCols0.Add("pluginname");
						table1Name = "plugin";

						break;

					default:
						reqCols0.Add("caption");
						reqCols0.Add("pluginid");
						table1Name = "none";

						break;
				}

				if (ds.Tables.Contains(table1Name)) {
					//validate that the dataset has the right table configuration
					DataTable dt0 = ds.Tables[table1Name];
					foreach (string c in reqCols0) {
						if (!dt0.Columns.Contains(c)) {
							DataColumn dc = new DataColumn(c);
							dc.DataType = System.Type.GetType("System.String"); // add if not found

							dt0.Columns.Add(dc);
							dt0.AcceptChanges();
						}
					}

					for (int iTbl = 1; iTbl < iTblCount; iTbl++) {
						DataTable dt = ds.Tables[iTbl];
						foreach (string c in reqCols1) {
							if (!dt.Columns.Contains(c)) {
								DataColumn dc = new DataColumn(c);
								dc.DataType = System.Type.GetType("System.String"); // add if not found

								dt.Columns.Add(dc);
								dt.AcceptChanges();
							}
						}
					}
				}
			}

			return ds;
		}

		public static DateTime CalcNearestFiveMinTime(DateTime dateIn) {
			dateIn = dateIn.AddMinutes(-2);
			int iMin = 5 * (dateIn.Minute / 5);

			DateTime dateOut = dateIn.AddMinutes(0 - dateIn.Minute).AddMinutes(iMin);

			return dateOut;
		}

		public string GetFolderPrefix(string sDirPath) {
			return FileDataHelper.MakeWebFolderPath(sDirPath);
		}

		public List<CMSAdminModule> AdminModules {
			get {
				var _modules = new List<CMSAdminModule>();

				bool bCached = false;

				try {
					_modules = (List<CMSAdminModule>)CarrotHttpHelper.CacheGet(keyAdminMenuModules);
					if (_modules != null) {
						bCached = true;
					}
				} catch {
					bCached = false;
				}

				if (!bCached) {
					List<CMSAdminModuleMenu> _ctrls = new List<CMSAdminModuleMenu>();
					_modules = new List<CMSAdminModule>();

					foreach (var p in _modules) {
						p.PluginMenus = (from c in _ctrls
										 where c.AreaKey == p.AreaKey
										 orderby c.Caption, c.SortOrder
										 select c).ToList();
					}

					_modules = _modules.Union(GetModulesByDirectory()).ToList();

					CarrotHttpHelper.CacheInsert(keyAdminMenuModules, _modules, 5);
				}

				return _modules.OrderBy(m => m.PluginName).ToList();
			}
		}

		private List<CMSPlugin> GetPluginsByDirectory() {
			var _plugins = new List<CMSPlugin>();

			CarrotCakeConfig config = CarrotCakeConfig.GetConfig();

			string sPlugCfg = CarrotHttpHelper.MapPath(config.ConfigFileLocation.PluginPath);

			if (Directory.Exists(sPlugCfg)) {
				string[] subdirs;
				try {
					subdirs = Directory.GetDirectories(sPlugCfg);
				} catch {
					subdirs = null;
				}

				if (subdirs != null) {
					foreach (string theDir in subdirs) {
						string sTplDef = Path.Join(theDir, "Public.config");

						if (File.Exists(sTplDef)) {
							string sPathPrefix = GetFolderPrefix(theDir);
							DataSet ds = ReadDataSetConfig(CMSConfigFileType.PublicCtrl, sPathPrefix);

							var _p2 = (from d in ds.Tables[0].AsEnumerable()
									   select new CMSPlugin {
										   SortOrder = 100,
										   FilePath = d.Field<string>("filepath").NormalizeFilename(),
										   Caption = d.Field<string>("crtldesc")
									   }).Where(x => x.FilePath.Contains(":")).ToList();

							foreach (var p in _p2.Where(x => x.FilePath.ToLowerInvariant().EndsWith("html")).Select(x => x)) {
								string[] path = p.FilePath.Split(':');
								if (path.Length > 2 && !string.IsNullOrEmpty(path[2])
											&& (path[2].ToLowerInvariant().EndsWith(".cshtml") || path[2].ToLowerInvariant().EndsWith(".vbhtml"))) {
									path[2] = Path.Join(sPathPrefix, path[2]).FixPathSlashes();
									p.FilePath = string.Join(":", path);
								}
							}

							var _p3 = (from d in ds.Tables[0].AsEnumerable()
									   select new CMSPlugin {
										   SortOrder = 100,
										   FilePath = Path.Join(sPathPrefix, d.Field<string>("filepath")).NormalizeFilename(),
										   Caption = d.Field<string>("crtldesc")
									   }).Where(x => !x.FilePath.Contains(":")).ToList();

							_plugins = _plugins.Union(_p2).Union(_p3).ToList();
						}
					}
				}
			}

			_plugins.Where(x => x.FilePath.StartsWith("~~/")).ToList().ForEach(r => r.FilePath = r.FilePath.Replace("~~/", "/"));
			_plugins.Where(x => x.FilePath.Contains("//")).ToList().ForEach(r => r.FilePath = r.FilePath.Replace("//", "/"));
			_plugins.Where(x => x.FilePath.StartsWith("~")).ToList().ForEach(r => r.FilePath = r.FilePath.Replace("~", ""));

			return _plugins;
		}

		public List<CMSPlugin> GetPluginsInFolder(string sPathPrefix) {
			var _plugins = new List<CMSPlugin>();

			sPathPrefix = sPathPrefix.FixPathSlashes();

			if (!string.IsNullOrEmpty(sPathPrefix)) {
				DataSet ds = ReadDataSetConfig(CMSConfigFileType.PublicCtrl, sPathPrefix);

				_plugins = (from d in ds.Tables[0].AsEnumerable()
							select new CMSPlugin {
								SortOrder = 100,
								FilePath = Path.Join(sPathPrefix, d.Field<string>("filepath")).NormalizeFilename(),
								Caption = d.Field<string>("crtldesc")
							}).ToList();
			}

			_plugins.Where(x => x.FilePath.StartsWith("~~/")).ToList().ForEach(r => r.FilePath = r.FilePath.Replace("~~/", "/"));
			_plugins.Where(x => x.FilePath.StartsWith("~")).ToList().ForEach(r => r.FilePath = r.FilePath.Replace("~", ""));

			return _plugins;
		}

		public CMSAdminModuleMenu GetCurrentAdminModuleControl() {
			string pf = string.Empty;
			CMSAdminModuleMenu cc = null;

			if (CarrotHttpHelper.QueryString("pf") != null) {
				pf = CarrotHttpHelper.QueryString("pf").ToString();

				CMSAdminModule mod = (from m in AdminModules
									  where m.AreaKey == PluginAreaPath
									  select m).FirstOrDefault();

				cc = (from m in mod.PluginMenus
					  orderby m.Caption, m.SortOrder
					  where m.Action == pf
					  select m).FirstOrDefault();
			}

			return cc;
		}

		public static string PluginAreaPath {
			get {
				string path = CarrotHttpHelper.Current.Request.Path;

				int i1 = path.IndexOf("/") + 1;
				int i2 = path.IndexOf("/", 2) - 1;

				return path.Substring(i1, i2);
			}
		}

		public List<CMSAdminModuleMenu> GetCurrentAdminModuleControlList() {
			HttpRequest request = CarrotHttpHelper.Current.Request;
			string pf = string.Empty;

			CMSAdminModule mod = (from m in AdminModules
								  where m.AreaKey == PluginAreaPath
								  select m).FirstOrDefault();

			return (from m in mod.PluginMenus
					orderby m.Caption, m.SortOrder
					select m).ToList();
		}

		public void GetFile(string sRemoteFile, string sLocalFile) {
			Uri remoteFile = new Uri(sRemoteFile);
			string sServerPath = CarrotHttpHelper.MapPath(sLocalFile);
			bool bExists = File.Exists(sServerPath);

			if (!bExists) {
				using (WebClient webClient = new WebClient()) {
					try {
						webClient.DownloadFile(remoteFile, sServerPath);
						//webClient.DownloadFileAsync(remoteFile, sServerPath);
					} catch (Exception ex) {
						if (ex is WebException) {
							WebException webException = (WebException)ex;
							var resp = (HttpWebResponse)webException.Response;
							if (!(resp.StatusCode == HttpStatusCode.NotFound)) {
								throw;
							}
						} else {
							throw;
						}
					}
				}
			}
		}

		private List<CMSAdminModule> GetModulesByDirectory() {
			var _plugins = new List<CMSAdminModule>();

			CarrotCakeConfig config = CarrotCakeConfig.GetConfig();

			string sPlugCfg = CarrotHttpHelper.MapPath(config.ConfigFileLocation.PluginPath);

			if (Directory.Exists(sPlugCfg)) {
				string[] subdirs;
				try {
					subdirs = Directory.GetDirectories(sPlugCfg);
				} catch {
					subdirs = null;
				}

				if (subdirs != null) {
					foreach (string theDir in subdirs) {
						string sTplDef = theDir + @"\Admin.config";

						if (File.Exists(sTplDef)) {
							string sPathPrefix = GetFolderPrefix(theDir);
							DataSet ds = ReadDataSetConfig(CMSConfigFileType.AdminMod, sPathPrefix);

							var _modules = (from d in ds.Tables[0].AsEnumerable()
											select new CMSAdminModule {
												PluginName = d.Field<string>("caption"),
												AreaKey = d.Field<string>("area")
											}).OrderBy(x => x.PluginName).ToList();

							var _ctrls = (from d in ds.Tables[1].AsEnumerable()
										  select new CMSAdminModuleMenu {
											  Caption = d.Field<string>("pluginlabel"),
											  SortOrder = string.IsNullOrEmpty(d.Field<string>("menuorder")) ? -1 : int.Parse(d.Field<string>("menuorder")),
											  Action = d.Field<string>("action"),
											  Controller = d.Field<string>("controller"),
											  UsePopup = string.IsNullOrEmpty(d.Field<string>("usepopup")) ? false : Convert.ToBoolean(d.Field<string>("usepopup")),
											  IsVisible = string.IsNullOrEmpty(d.Field<string>("visible")) ? false : Convert.ToBoolean(d.Field<string>("visible")),
											  AreaKey = d.Field<string>("area")
										  }).OrderBy(x => x.Caption).OrderBy(x => x.SortOrder).ToList();

							foreach (var p in _modules) {
								p.PluginMenus = (from c in _ctrls
												 where c.AreaKey == p.AreaKey
												 orderby c.Caption, c.SortOrder
												 select c).ToList();
							}

							_plugins = _plugins.Union(_modules).ToList();
						}
					}
				}
			}

			return _plugins;
		}

		public List<CMSTextWidgetPicker> GetAllWidgetSettings(Guid siteID) {
			List<TextWidget> lstPreferenced = TextWidget.GetSiteTextWidgets(siteID);

			List<string> lstInUse = lstPreferenced.Select(x => x.TextWidgetAssembly).Distinct().ToList();

			List<string> lstAvail = TextWidgets.Select(x => x.AssemblyString).Distinct().ToList();

			List<CMSTextWidgetPicker> lstExisting = (from p in lstPreferenced
													 join t in TextWidgets on p.TextWidgetAssembly equals t.AssemblyString
													 select new CMSTextWidgetPicker {
														 TextWidgetPickerID = p.TextWidgetID,
														 AssemblyString = p.TextWidgetAssembly,
														 DisplayName = t.DisplayName,
														 ProcessBody = p.ProcessBody,
														 ProcessPlainText = p.ProcessPlainText,
														 ProcessHTMLText = p.ProcessHTMLText,
														 ProcessComment = p.ProcessComment,
														 ProcessSnippet = p.ProcessSnippet,
													 }).ToList();

			List<CMSTextWidgetPicker> lstConfigured1 = (from t in TextWidgets
														where !lstInUse.Contains(t.AssemblyString)
														select new CMSTextWidgetPicker {
															TextWidgetPickerID = Guid.NewGuid(),
															AssemblyString = t.AssemblyString,
															DisplayName = t.DisplayName,
															ProcessBody = false,
															ProcessPlainText = false,
															ProcessHTMLText = false,
															ProcessComment = false,
															ProcessSnippet = false,
														}).ToList();

			lstExisting = lstExisting.Union(lstConfigured1).ToList();

			List<CMSTextWidgetPicker> lstConfigured2 = (from p in lstPreferenced
														where !lstAvail.Contains(p.TextWidgetAssembly)
														select new CMSTextWidgetPicker {
															TextWidgetPickerID = p.TextWidgetID,
															AssemblyString = p.TextWidgetAssembly,
															DisplayName = string.Empty,
															ProcessBody = p.ProcessBody,
															ProcessPlainText = p.ProcessPlainText,
															ProcessHTMLText = p.ProcessHTMLText,
															ProcessComment = p.ProcessComment,
															ProcessSnippet = p.ProcessSnippet,
														}).ToList();

			lstExisting = lstExisting.Union(lstConfigured2).ToList();

			return lstExisting;
		}

		public List<CMSPlugin> ToolboxPlugins {
			get {
				var _plugins = new List<CMSPlugin>();

				bool bCached = false;

				try {
					_plugins = (List<CMSPlugin>)CarrotHttpHelper.CacheGet(keyAdminToolboxModules);
					if (_plugins != null) {
						bCached = true;
					}
				} catch {
					bCached = false;
				}

				if (!bCached) {
					int iSortOrder = 0;

					List<CMSPlugin> _p1 = new List<CMSPlugin>();

					_p1.Add(new CMSPlugin { SystemPlugin = true, SortOrder = iSortOrder++, Caption = "Generic HTML", FilePath = "CLASS:Carrotware.CMS.UI.Components.ContentRichText, Carrotware.CMS.UI.Components" });
					_p1.Add(new CMSPlugin { SystemPlugin = true, SortOrder = iSortOrder++, Caption = "Plain Text", FilePath = "CLASS:Carrotware.CMS.UI.Components.ContentPlainText, Carrotware.CMS.UI.Components" });
					_p1.Add(new CMSPlugin { SystemPlugin = true, SortOrder = iSortOrder++, Caption = "Content Snippet", FilePath = "CLASS:Carrotware.CMS.UI.Components.ContentSnippetText, Carrotware.CMS.UI.Components" });

					_p1.Add(new CMSPlugin { SystemPlugin = true, SortOrder = iSortOrder++, Caption = "Top Level Navigation", FilePath = "CLASS:Carrotware.CMS.UI.Components.TopLevelNavigation, Carrotware.CMS.UI.Components" });
					_p1.Add(new CMSPlugin { SystemPlugin = true, SortOrder = iSortOrder++, Caption = "Two Level Navigation", FilePath = "CLASS:Carrotware.CMS.UI.Components.TwoLevelNavigation, Carrotware.CMS.UI.Components" });

					_p1.Add(new CMSPlugin { SystemPlugin = true, SortOrder = iSortOrder++, Caption = "Child Navigation", FilePath = "CLASS:Carrotware.CMS.UI.Components.ChildNavigation, Carrotware.CMS.UI.Components" });
					_p1.Add(new CMSPlugin { SystemPlugin = true, SortOrder = iSortOrder++, Caption = "Second Level/ Sibling Navigation", FilePath = "CLASS:Carrotware.CMS.UI.Components.SecondLevelNavigation, Carrotware.CMS.UI.Components" });
					_p1.Add(new CMSPlugin { SystemPlugin = true, SortOrder = iSortOrder++, Caption = "Most Recent Updated", FilePath = "CLASS:Carrotware.CMS.UI.Components.MostRecentUpdated, Carrotware.CMS.UI.Components" });

					_p1.Add(new CMSPlugin { SystemPlugin = true, SortOrder = iSortOrder++, Caption = "Multi Level Nav List", FilePath = "CLASS:Carrotware.CMS.UI.Components.MultiLevelNavigation, Carrotware.CMS.UI.Components" });

					_p1.Add(new CMSPlugin { SystemPlugin = true, SortOrder = iSortOrder++, Caption = "IFRAME content wrapper", FilePath = "CLASS:Carrotware.CMS.UI.Components.IFrameWidgetWrapper, Carrotware.CMS.UI.Components" });

					_plugins = _p1.Union(GetPluginsByDirectory()).ToList();

					_plugins.Where(x => x.FilePath.StartsWith("~~/")).ToList().ForEach(r => r.FilePath = r.FilePath.Replace("~~/", "/"));
					_plugins.Where(x => x.FilePath.StartsWith("~")).ToList().ForEach(r => r.FilePath = r.FilePath.Replace("~", ""));

					CarrotHttpHelper.CacheInsert(keyAdminToolboxModules, _plugins, 5);
				}

				return _plugins.OrderBy(p => p.SystemPlugin).OrderBy(p => p.Caption).OrderBy(p => p.SortOrder).ToList();
			}
		}

		private List<CMSTemplate> GetTemplatesByDirectory() {
			var _plugins = new List<CMSTemplate>();

			CarrotCakeConfig config = CarrotCakeConfig.GetConfig();

			string sPlugCfg = CarrotHttpHelper.MapPath(config.ConfigFileLocation.TemplatePath);

			if (Directory.Exists(sPlugCfg)) {
				string[] subdirs;
				try {
					subdirs = Directory.GetDirectories(sPlugCfg);
				} catch {
					subdirs = null;
				}

				if (subdirs != null) {
					foreach (string theDir in subdirs) {
						string sTplDef = Path.Join(theDir, "Skin.config");

						if (File.Exists(sTplDef)) {
							string sPathPrefix = GetFolderPrefix(theDir);
							DataSet ds = ReadDataSetConfig(CMSConfigFileType.SkinDef, sPathPrefix);

							var _p2 = (from d in ds.Tables[0].AsEnumerable()
									   select new CMSTemplate {
										   TemplatePath = sPathPrefix + d.Field<string>("templatefile").ToLowerInvariant().FixPathSlashes().ToLowerInvariant(),
										   EncodedPath = string.Empty,
										   Caption = d.Field<string>("filedesc")
									   }).ToList();

							_plugins = _plugins.Union(_p2).ToList();

							_plugins.Where(x => x.TemplatePath.StartsWith("~~/")).ToList()
								.ForEach(r => r.TemplatePath = r.TemplatePath.Replace("~~/", "/"));
							_plugins.Where(x => x.TemplatePath.Contains("//")).ToList()
								.ForEach(r => r.TemplatePath = r.TemplatePath.Replace("//", "/"));
							_plugins.Where(x => x.TemplatePath.StartsWith("~")).ToList()
								.ForEach(r => r.TemplatePath = r.TemplatePath.Replace("~", ""));

							_plugins.ForEach(r => r.EncodedPath = EncodeBase64(r.TemplatePath));
						}
					}
				}
			}

			return _plugins;
		}

		public List<CMSTemplate> Templates {
			get {
				List<CMSTemplate> _plugins = null;
				bool bCached = false;

				try {
					_plugins = (List<CMSTemplate>)CarrotHttpHelper.CacheGet(keyTemplates);
					if (_plugins != null) {
						bCached = true;
					}
				} catch {
					bCached = false;
				}

				if (!bCached) {
					var site = SiteData.CurrentSite;
					_plugins = new List<CMSTemplate>();

					var t1 = new CMSTemplate();
					t1.TemplatePath = site.TemplateFilename;
					t1.EncodedPath = EncodeBase64(site.TemplateFilename);
					t1.Caption = string.Format("    {0} [*]  ", CarrotWebHelper.DisplayNameFor<SiteData>(x => x.TemplateFilename));
					_plugins.Add(t1);

					var t2 = new CMSTemplate();
					t2.TemplatePath = site.TemplateBWFilename;
					t2.EncodedPath = EncodeBase64(site.TemplateBWFilename);
					t2.Caption = string.Format("   {0} [*]  ", CarrotWebHelper.DisplayNameFor<SiteData>(x => x.TemplateBWFilename));
					_plugins.Add(t2);
				}

				if (!bCached) {
					var _p2 = GetTemplatesByDirectory();

					_plugins = _plugins.Union(_p2.Where(t => !SiteData.DefaultTemplates.Contains(t.TemplatePath.ToLowerInvariant()))).ToList();

					CarrotHttpHelper.CacheInsert(keyTemplates, _plugins, 3);
				}

				return _plugins.OrderBy(t => t.Caption).ToList();
			}
		}

		public List<CMSTextWidget> TextWidgets {
			get {
				List<CMSTextWidget> _plugins = null;
				bool bCached = false;

				try {
					_plugins = (List<CMSTextWidget>)CarrotHttpHelper.CacheGet(keyTxtWidgets);
					if (_plugins != null) {
						bCached = true;
					}
				} catch {
					bCached = false;
				}

				if (!bCached) {
					_plugins = new List<CMSTextWidget>();
				}

				if (!bCached) {
					DataSet ds = ReadDataSetConfig(CMSConfigFileType.SiteTextWidgets, "~/");

					_plugins = (from d in ds.Tables[0].AsEnumerable()
								select new CMSTextWidget {
									AssemblyString = d.Field<string>("pluginassembly"),
									DisplayName = d.Field<string>("pluginname")
								}).ToList();

					CarrotHttpHelper.CacheInsert(keyTxtWidgets, _plugins, 5);
				}

				return _plugins.OrderBy(t => t.DisplayName).ToList();
			}
		}

		public static List<DynamicSite> SiteList {
			get {
				var _sites = new List<DynamicSite>();

				bool bCached = false;

				try {
					_sites = (List<DynamicSite>)CarrotHttpHelper.CacheGet(keyDynamicSite);
					if (_sites != null) {
						bCached = true;
					}
				} catch {
					bCached = false;
				}

				if (!bCached) {
					_sites = CarrotCakeConfig.GetConfig().SiteMapping;

					CarrotHttpHelper.CacheInsert(keyDynamicSite, _sites, 5);
				}
				return _sites;
			}
		}

		public static Guid PrimarySiteID {
			get {
				Guid siteId = Guid.Empty;
				bool bCached = false;

				try {
					var val = CarrotHttpHelper.CacheGet(keyPrimarySite);
					if (val != null) {
						siteId = new Guid(val.ToString());
						bCached = siteId != Guid.Empty;
					}
				} catch {
					bCached = false;
				}

				if (!bCached) {
					siteId = CarrotCakeConfig.GetConfig().MainConfig.SiteID.Value;

					CarrotHttpHelper.CacheInsert(keyPrimarySite, siteId, 3);
				}

				return siteId;
			}
		}

		public static DynamicSite DynSite {
			get {
				DynamicSite _site = new DynamicSite();

				string ModuleKey = keyDynSite + DomainName;
				bool bCached = false;

				try {
					_site = (DynamicSite)CarrotHttpHelper.CacheGet(ModuleKey);
					if (_site != null) {
						bCached = true;
					}
				} catch {
					bCached = false;
				}

				if ((SiteList.Any()) && !bCached) {
					_site = (from ss in SiteList
							 where ss.DomainName == DomainName
							 select ss).FirstOrDefault();

					CarrotHttpHelper.CacheInsert(ModuleKey, _site, 5);
				}
				return _site;
			}
		}

		public static bool CheckRequestedFileExistence(string templateFileName, Guid siteID) {
			var _tmplts = GetTmplateStatus();

			CMSFilePath tmp = _tmplts.Where(x => x.TemplateFile.ToLowerInvariant() == templateFileName.ToLowerInvariant() && x.SiteID == siteID).FirstOrDefault();

			if (tmp == null) {
				tmp = new CMSFilePath(templateFileName, siteID);
				_tmplts.Add(tmp);
#if DEBUG
				Debug.WriteLine(" ================ " + DateTime.UtcNow.ToString() + " ================");
				Debug.WriteLine("Grabbed file : CheckRequestedFileExistence(string templateFileName, Guid siteID) " + templateFileName);
#endif
			}

			SaveTmplateStatus(_tmplts);

			return tmp.FileExists;
		}

		public static bool CheckFileExistence(string templateFileName) {
			var _tmplts = GetTmplateStatus();

			CMSFilePath tmp = _tmplts.Where(x => x.TemplateFile.ToLowerInvariant() == templateFileName.ToLowerInvariant() && x.SiteID == Guid.Empty).FirstOrDefault();

			if (tmp == null) {
				tmp = new CMSFilePath(templateFileName);
				_tmplts.Add(tmp);
#if DEBUG
				Debug.WriteLine(" ================ " + DateTime.UtcNow.ToString() + " ================");
				Debug.WriteLine("Grabbed file : CheckFileExistence(string templateFileName) " + templateFileName);
#endif
			}

			SaveTmplateStatus(_tmplts);

			return tmp.FileExists;
		}

		private static void SaveTmplateStatus(List<CMSFilePath> fileState) {
			CarrotHttpHelper.CacheInsert(keyTemplateFiles, fileState, 10);
		}

		private static List<CMSFilePath> GetTmplateStatus() {
			var _tmplts = new List<CMSFilePath>();

			try { _tmplts = (List<CMSFilePath>)CarrotHttpHelper.CacheGet(keyTemplateFiles); } catch { }

			if (_tmplts == null) {
				_tmplts = new List<CMSFilePath>();
			}

			_tmplts.RemoveAll(x => x.DateChecked < DateTime.UtcNow.AddSeconds(-30));

			_tmplts.RemoveAll(x => x.DateChecked < DateTime.UtcNow.AddSeconds(-10) && x.SiteID != Guid.Empty);

			return _tmplts;
		}

		//=========================

		public static TimeZoneInfo GetLocalTimeZoneInfo() {
			TimeZoneInfo oTZ = TimeZoneInfo.Local;

			return oTZ;
		}

		public static TimeZoneInfo GetSiteTimeZoneInfo(string timeZoneIdentifier) {
			TimeZoneInfo oTZ = GetLocalTimeZoneInfo();

			if (!string.IsNullOrEmpty(timeZoneIdentifier)) {
				try { oTZ = TimeZoneInfo.FindSystemTimeZoneById(timeZoneIdentifier); } catch { }
			}

			return oTZ;
		}

		public static DateTime ConvertUTCToSiteTime(DateTime dateUTC, string timeZoneIdentifier) {
			TimeZoneInfo oTZ = GetSiteTimeZoneInfo(timeZoneIdentifier);

			return TimeZoneInfo.ConvertTimeFromUtc(dateUTC, oTZ);
		}

		public static DateTime ConvertSiteTimeToUTC(DateTime dateSite, string timeZoneIdentifier) {
			TimeZoneInfo oTZ = GetSiteTimeZoneInfo(timeZoneIdentifier);

			return TimeZoneInfo.ConvertTimeToUtc(dateSite, oTZ);
		}

		//===================

		public static string InactivePagePrefix {
			get {
				return "&#9746; ";
			}
		}

		public static string RetiredPagePrefix {
			get {
				return "&#9851; ";
			}
		}

		public static string UnreleasedPagePrefix {
			get {
				return "&#9888; ";
			}
		}

		public static string PendingDeletePrefix {
			get {
				return "&#9940; ";
			}
		}

		public static List<SiteNav> TweakData(List<SiteNav> navs) {
			if (navs != null) {
				navs.RemoveAll(x => x.ShowInSiteNav == false && x.ContentType == ContentPageType.PageType.ContentEntry);

				navs.ForEach(q => FixNavLinkText(q));
			}

			return navs;
		}

		public static string EncodeNavText(string text) {
			return HttpUtility.HtmlEncode(text);
		}

		public static SiteNav FixNavLinkText(SiteNav nav) {
			if (nav != null && !nav.MadeSafe) {
				nav.MadeSafe = true;
				nav.NavMenuText = EncodeNavText(nav.NavMenuText);
				nav.PageHead = EncodeNavText(nav.PageHead);
				nav.TitleBar = EncodeNavText(nav.TitleBar);

				if (!nav.PageActive) {
					nav.NavMenuText = InactivePagePrefix + nav.NavMenuText;
					nav.PageHead = InactivePagePrefix + nav.PageHead;
					nav.TitleBar = InactivePagePrefix + nav.TitleBar;
				}
				if (nav.IsRetired) {
					nav.NavMenuText = RetiredPagePrefix + nav.NavMenuText;
					nav.PageHead = RetiredPagePrefix + nav.PageHead;
					nav.TitleBar = RetiredPagePrefix + nav.TitleBar;
				}
				if (nav.IsUnReleased) {
					nav.NavMenuText = UnreleasedPagePrefix + nav.NavMenuText;
					nav.PageHead = UnreleasedPagePrefix + nav.PageHead;
					nav.TitleBar = UnreleasedPagePrefix + nav.TitleBar;
				}
			}
			return nav;
		}

		public static ContentPage FixNavLinkText(ContentPage cp) {
			if (cp != null && !cp.MadeSafe) {
				cp.MadeSafe = true;
				cp.NavMenuText = EncodeNavText(cp.NavMenuText);
				cp.PageHead = EncodeNavText(cp.PageHead);
				cp.TitleBar = EncodeNavText(cp.TitleBar);

				if (!cp.PageActive) {
					cp.NavMenuText = InactivePagePrefix + cp.NavMenuText;
					cp.PageHead = InactivePagePrefix + cp.PageHead;
					cp.TitleBar = InactivePagePrefix + cp.TitleBar;
				}
				if (cp.IsRetired) {
					cp.NavMenuText = RetiredPagePrefix + cp.NavMenuText;
					cp.PageHead = RetiredPagePrefix + cp.PageHead;
					cp.TitleBar = RetiredPagePrefix + cp.TitleBar;
				}
				if (cp.IsUnReleased) {
					cp.NavMenuText = UnreleasedPagePrefix + cp.NavMenuText;
					cp.PageHead = UnreleasedPagePrefix + cp.PageHead;
					cp.TitleBar = UnreleasedPagePrefix + cp.TitleBar;
				}
			}

			return cp;
		}

		public static PostComment IdentifyLinkAsInactive(PostComment pc) {
			if (pc != null) {
				if (!pc.IsApproved) {
					pc.CommenterName = InactivePagePrefix + pc.CommenterName;
				}
				if (pc.IsSpam) {
					pc.CommenterName = RetiredPagePrefix + pc.CommenterName;
				}
			}

			return pc;
		}

		//=====================

		public static string DecodeBase64(string ValIn) {
			return Utils.DecodeBase64(ValIn);
		}

		public static string EncodeBase64(string ValIn) {
			return Utils.EncodeBase64(ValIn);
		}

		public void OverrideKey(Guid guidContentID) {
			filePage = null;
			using (ContentPageHelper pageHelper = new ContentPageHelper()) {
				filePage = pageHelper.FindContentByID(SiteData.CurrentSiteID, guidContentID);
			}
		}

		public void OverrideKey(string sPageName) {
			filePage = null;
			using (ContentPageHelper pageHelper = new ContentPageHelper()) {
				filePage = pageHelper.FindByFilename(SiteData.CurrentSiteID, sPageName);
			}
		}

		protected ContentPage filePage = null;

		protected void LoadGuids() {
			if (filePage == null) {
				using (ContentPageHelper pageHelper = new ContentPageHelper()) {
					if (SiteData.IsPageSampler && filePage == null) {
						filePage = ContentPageHelper.GetSamplerView();
					} else {
						if (SiteData.CurrentScriptName.ToLowerInvariant().StartsWith(SiteData.AdminFolderPath)) {
							Guid guidPage = Guid.Empty;
							if (!string.IsNullOrEmpty(CarrotHttpHelper.QueryString("pageid"))) {
								guidPage = new Guid(CarrotHttpHelper.QueryString("pageid").ToString());
							}
							filePage = pageHelper.FindContentByID(SiteData.CurrentSiteID, guidPage);
						} else {
							filePage = pageHelper.FindByFilename(SiteData.CurrentSiteID, SiteData.CurrentScriptName);
						}
					}
				}
			}
		}

		public ContentPage cmsAdminContent {
			get {
				ContentPage c = null;
				try {
					string sXML = GetSerialized(keyAdminContent);
					if (!string.IsNullOrEmpty(sXML)) {
						XmlSerializer xmlSerializer = new XmlSerializer(typeof(ContentPage));
						Object genpref = null;
						using (StringReader stringReader = new StringReader(sXML)) {
							genpref = xmlSerializer.Deserialize(stringReader);
						}
						c = genpref as ContentPage;
					}
				} catch (Exception ex) { }
				return c;
			}
			set {
				if (value == null) {
					ClearSerialized(keyAdminContent);
				} else {
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(ContentPage));
					string sXML = string.Empty;
					using (StringWriter stringWriter = new StringWriter()) {
						xmlSerializer.Serialize(stringWriter, value);
						sXML = stringWriter.ToString();
					}
					SaveSerialized(keyAdminContent, sXML);
				}
			}
		}

		public List<Widget> cmsAdminWidget {
			get {
				List<Widget> c = null;
				string sXML = GetSerialized(keyAdminWidget);
				if (!string.IsNullOrEmpty(sXML)) {
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Widget>));
					Object genpref = null;
					using (StringReader stringReader = new StringReader(sXML)) {
						genpref = xmlSerializer.Deserialize(stringReader);
					}
					c = genpref as List<Widget>;
				}
				return c;
			}
			set {
				if (value == null) {
					ClearSerialized(keyAdminWidget);
				} else {
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Widget>));
					string sXML = string.Empty;
					using (StringWriter stringWriter = new StringWriter()) {
						xmlSerializer.Serialize(stringWriter, value);
						sXML = stringWriter.ToString();
					}
					SaveSerialized(keyAdminWidget, sXML);
				}
			}
		}

		public static void SaveSerialized(Guid itemID, string sKey, string sData) {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				bool bAdd = false;

				CarrotSerialCache itm = CompiledQueries.SearchSeriaCache(_db, itemID, sKey);

				if (itm == null) {
					bAdd = true;
					itm = new CarrotSerialCache();
					itm.SerialCacheId = Guid.NewGuid();
					itm.SiteId = SiteData.CurrentSiteID;
					itm.ItemId = itemID;
					itm.EditUserId = SecurityData.CurrentUserGuid;
					itm.KeyType = sKey;
				}

				itm.SerializedData = sData;
				itm.EditDate = DateTime.UtcNow;

				if (bAdd) {
					_db.CarrotSerialCache.Add(itm);
				}
				_db.SaveChanges();
			}
		}

		public static string GetSerialized(Guid itemID, string sKey) {
			string sData = string.Empty;
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				CarrotSerialCache itm = CompiledQueries.SearchSeriaCache(_db, itemID, sKey);

				if (itm != null) {
					sData = itm.SerializedData;
				}
			}

			return sData;
		}

		public static bool ClearSerialized(Guid itemID, string sKey) {
			bool bRet = false;
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				CarrotSerialCache itm = CompiledQueries.SearchSeriaCache(_db, itemID, sKey);

				if (itm != null) {
					_db.CarrotSerialCache.Remove(itm);
					_db.SaveChanges();
					bRet = true;
				}
			}
			return bRet;
		}

		private void SaveSerialized(string sKey, string sData) {
			LoadGuids();
			if (filePage != null) {
				CMSConfigHelper.SaveSerialized(filePage.Root_ContentID, sKey, sData);
			}
		}

		private string GetSerialized(string sKey) {
			string sData = string.Empty;
			LoadGuids();

			if (filePage != null) {
				sData = CMSConfigHelper.GetSerialized(filePage.Root_ContentID, sKey);
			}
			return sData;
		}

		private bool ClearSerialized(string sKey) {
			LoadGuids();
			if (filePage != null) {
				return CMSConfigHelper.ClearSerialized(filePage.Root_ContentID, sKey);
			} else {
				return false;
			}
		}

		public static void CleanUpSerialData() {
			using (CarrotCakeContext _db = CarrotCakeContext.Create()) {
				_db.CarrotSerialCache.Where(c => c.EditDate < DateTime.UtcNow.AddHours(-6)).ExecuteDelete();
				_db.SaveChanges();
			}
		}
	}
}