using Carrotware.CMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

	public class SiteMapOrderHelper : IDisposable {
		private CarrotCakeContext _db = CarrotCakeContext.Create();
		//private CarrotCakeContext _db = CompiledQueries.dbConn;

		public SiteMapOrderHelper() { }

		public List<SiteMapOrder> CreateSiteMapList(string sMapText) {
			List<SiteMapOrder> m = new List<SiteMapOrder>();
			sMapText = sMapText.Trim();

			if (!string.IsNullOrEmpty(sMapText)) {
				sMapText = sMapText.Replace("\r\n", "\n");
				var rows = sMapText.Split('\n');
				foreach (string r in rows) {
					if (!string.IsNullOrEmpty(r)) {
						var rr = r.Split('\t');
						SiteMapOrder s = new SiteMapOrder();
						s.NavOrder = int.Parse(rr[0]);
						s.Parent_ContentID = new Guid(rr[1]);
						s.Root_ContentID = new Guid(rr[2]);
						if (s.Parent_ContentID == Guid.Empty) {
							s.Parent_ContentID = null;
						}
						m.Add(s);
					}
				}
			}

			return m;
		}

		public List<SiteMapOrder> ParseChildPageData(string sMapText, Guid contentID) {
			List<SiteMapOrder> m = new List<SiteMapOrder>();
			sMapText = sMapText.Trim();

			var c = (from ct in _db.CarrotContents
					 where ct.RootContentId == contentID
						&& ct.IsLatestVersion == true
					 select ct).FirstOrDefault();

			int iOrder = Convert.ToInt32(c.NavOrder) + 2;

			if (!string.IsNullOrEmpty(sMapText)) {
				sMapText = sMapText.Replace("\r\n", "\n");
				var rows = sMapText.Split('\n');
				foreach (string r in rows) {
					if (!string.IsNullOrEmpty(r)) {
						var rr = r.Split('\t');
						SiteMapOrder s = new SiteMapOrder();
						s.NavOrder = iOrder + int.Parse(rr[0]);
						s.Root_ContentID = new Guid(rr[1]);
						s.Parent_ContentID = contentID;
						m.Add(s);
					}
				}
			}

			return m;
		}

		public List<SiteMapOrder> GetSiteFileList(Guid siteID) {
			List<SiteMapOrder> lstContent = CannedQueries.GetAllContentList(_db, siteID).Select(ct => new SiteMapOrder(ct)).ToList();

			return lstContent;
		}

		public void FixBlogIndex(Guid siteID) {
			SiteData site = SiteData.GetSiteFromCache(siteID);

			if (site.Blog_Root_ContentID.HasValue) {
				// because sometimes the db is manually manipulated, provides way of re-setting blog/index page by validating page is part of the site
				var blogIndexPage = CannedQueries.GetContentByRoot(_db, site.Blog_Root_ContentID.Value).FirstOrDefault();
				Guid contentTypeID = ContentPageType.GetIDByType(ContentPageType.PageType.ContentEntry);
				if (blogIndexPage != null) {
					// found blog, but not in this site or is not a page
					if (blogIndexPage.SiteId != site.SiteID || blogIndexPage.ContentTypeId != contentTypeID) {
						site.Blog_Root_ContentID = null;
						site.Save();
					}
				}
			}
		}

		public void FixOrphanPages(Guid siteID) {
			FixBlogIndex(siteID);

			List<SiteMapOrder> lstContent = CannedQueries.GetAllContentList(_db, siteID).Select(ct => new SiteMapOrder(ct)).ToList();
			List<Guid> lstIDs = lstContent.Select(x => x.Root_ContentID).ToList();

			lstContent.RemoveAll(x => x.Parent_ContentID == null);
			lstContent.RemoveAll(x => lstIDs.Contains(x.Parent_ContentID.Value));

			lstIDs = lstContent.Select(x => x.Root_ContentID).ToList();

			IQueryable<CarrotContent> querySite1 = (from c in _db.CarrotContents
													where c.IsLatestVersion == true
														&& c.ParentContentId != null
														&& lstIDs.Contains(c.RootContentId)
													select c);

			_db.CarrotContents.Where(x => querySite1.Select(x => x.ContentId).Contains(x.ContentId))
						.ExecuteUpdate(y => y.SetProperty(z => z.ParentContentId, (Guid?)null));

			IQueryable<CarrotContent> querySite2 = (from c in _db.CarrotContents
													join rc in _db.CarrotRootContents on c.RootContentId equals rc.RootContentId
													where c.IsLatestVersion == true
													   && c.ParentContentId != null
													   && rc.SiteId == siteID
													   && rc.ContentTypeId == ContentPageType.GetIDByType(ContentPageType.PageType.BlogEntry)
													select c);

			_db.CarrotContents.Where(x => querySite2.Select(x => x.ContentId).Contains(x.ContentId))
						.ExecuteUpdate(y => y.SetProperty(z => z.ParentContentId, (Guid?)null));

			_db.SaveChanges();
		}

		public void UpdateSiteMap(Guid siteID, List<SiteMapOrder> oMap) {
			oMap.Where(m => m.Parent_ContentID == Guid.Empty).ToList().ForEach(m => m.Parent_ContentID = null);

			foreach (SiteMapOrder m in oMap.OrderBy(m => m.NavOrder)) {

				CarrotContent c = (from ct in _db.CarrotContents
								   join r in _db.CarrotRootContents on ct.RootContentId equals r.RootContentId
								   where r.SiteId == siteID
									   && r.RootContentId == m.Root_ContentID
									   && ct.IsLatestVersion == true
								   select ct).FirstOrDefault();

				c.ParentContentId = m.Parent_ContentID;
				c.NavOrder = (m.NavOrder * 10);
			}

			_db.SaveChanges();
		}

		public List<SiteMapOrder> GetChildPages(Guid siteID, Guid? parentID, Guid contentID) {
			List<vwCarrotContent> lstOtherPages = CompiledQueries.GetOtherNotPage(_db, siteID, contentID, parentID).ToList();

			if (!lstOtherPages.Any() && parentID == Guid.Empty) {
				lstOtherPages = CompiledQueries.TopLevelPages(_db, siteID, false).ToList();
			}

			lstOtherPages.RemoveAll(x => x.RootContentId == contentID);
			lstOtherPages.RemoveAll(x => x.ParentContentId == contentID);

			List<SiteMapOrder> lst = (from ct in lstOtherPages
									  select new SiteMapOrder {
										  NavLevel = -1,
										  NavMenuText = (ct.PageActive ? "" : "{*U*} ") + ct.NavMenuText,
										  NavOrder = ct.NavOrder,
										  SiteID = ct.SiteId,
										  FileName = ct.FileName,
										  PageActive = ct.PageActive,
										  ShowInSiteNav = ct.ShowInSiteNav,
										  Parent_ContentID = ct.ParentContentId,
										  Root_ContentID = ct.RootContentId
									  }).ToList();

			return lst;
		}

		public SiteMapOrder GetPageWithLevel(Guid siteID, Guid? contentID, int iLevel) {
			SiteMapOrder cont = (from ct in CompiledQueries.cqGetLatestContentPages(_db, siteID, contentID).ToList()
								 select new SiteMapOrder {
									 NavLevel = iLevel,
									 NavMenuText = (ct.PageActive ? "" : "{*U*} ") + ct.NavMenuText,
									 NavOrder = ct.NavOrder,
									 SiteID = ct.SiteId,
									 FileName = ct.FileName,
									 PageActive = ct.PageActive,
									 ShowInSiteNav = ct.ShowInSiteNav,
									 Parent_ContentID = ct.ParentContentId,
									 Root_ContentID = ct.RootContentId
								 }).FirstOrDefault();

			return cont;
		}

		public List<SiteMapOrder> GetAdminPageList(Guid siteID, Guid contentID) {
			List<SiteMapOrder> lstSite = (from ct in CompiledQueries.ContentNavAll(_db, siteID, false).ToList()
										  select new SiteMapOrder {
											  NavLevel = -1,
											  NavMenuText = ct.NavMenuText,
											  NavOrder = ct.NavOrder,
											  SiteID = ct.SiteId,
											  FileName = ct.FileName,
											  PageActive = ct.PageActive,
											  ShowInSiteNav = ct.ShowInSiteNav,
											  Parent_ContentID = ct.ParentContentId,
											  Root_ContentID = ct.RootContentId
										  }).ToList();

			List<SiteMapOrder> lstSiteMap = new List<SiteMapOrder>();
			int iLevel = 0;
			int iBefore = 0;
			int iAfter = -10;
			int iLvlCounter = 0;
			int iPageCt = lstSite.Count;

			lstSiteMap = (from c in lstSite
						  orderby c.NavOrder, c.NavMenuText
						  where c.Parent_ContentID == null
						   && (c.Root_ContentID != contentID || contentID == Guid.Empty)
						  select new SiteMapOrder {
							  NavLevel = iLevel,
							  NavMenuText = c.NavMenuText,
							  NavOrder = (iLvlCounter++) * iPageCt,
							  SiteID = c.SiteID,
							  FileName = c.FileName,
							  PageActive = c.PageActive,
							  ShowInSiteNav = c.ShowInSiteNav,
							  Parent_ContentID = c.Parent_ContentID,
							  Root_ContentID = c.Root_ContentID
						  }).ToList();

			while (iBefore != iAfter) {
				List<SiteMapOrder> lstLevel = (from z in lstSiteMap
											   where z.NavLevel == iLevel
											   select z).ToList();

				iBefore = lstSiteMap.Count;
				iLevel++;

				iLvlCounter = 0;

				List<SiteMapOrder> lstChild = (from s in lstSite
											   join l in lstLevel on s.Parent_ContentID equals l.Root_ContentID
											   orderby s.NavOrder, s.NavMenuText
											   where (s.Root_ContentID != contentID || contentID == Guid.Empty)
											   select new SiteMapOrder {
												   NavLevel = iLevel,
												   NavMenuText = l.NavMenuText + " > " + s.NavMenuText,
												   NavOrder = l.NavOrder + (iLvlCounter++)
													   + (from s2 in lstSite
														  join l2 in lstLevel on s2.Parent_ContentID equals l2.Root_ContentID
														  where s.Parent_ContentID == s2.Parent_ContentID
																 && s.Root_ContentID != s2.Root_ContentID
														  select s.Root_ContentID).ToList().Count,
												   SiteID = s.SiteID,
												   FileName = s.FileName,
												   PageActive = s.PageActive,
												   ShowInSiteNav = s.ShowInSiteNav,
												   Parent_ContentID = s.Parent_ContentID,
												   Root_ContentID = s.Root_ContentID
											   }).ToList();

				lstSiteMap = (from m in lstSiteMap.Union(lstChild).ToList()
							  orderby m.NavOrder, m.NavMenuText
							  select m).ToList();

				iAfter = lstSiteMap.Count;
			}

			return (from m in lstSiteMap
					orderby m.NavOrder, m.NavMenuText
					select m).ToList();
		}

		#region IDisposable Members

		public void Dispose() {
			if (_db != null) {
				_db.Dispose();
			}
		}

		#endregion IDisposable Members
	}
}