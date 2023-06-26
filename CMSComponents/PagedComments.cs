using Carrotware.CMS.Core;
using Carrotware.Web.UI.Components;
using System;
using System.Collections.Generic;
using System.Linq;

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

	public class PagedComments : PagedData<PostComment>, IPagedContent {

		public PagedComments() {
			this.InitOrderBy(x => x.CreateDate, false);
		}

		public string GetUrl(int pageNbr) {
			return String.Format("{0}?{1}={2}", SiteData.CurrentScriptName, this.PageNumbParm, pageNbr);
		}

		public void FetchData() {
			base.ReadPageNbr();
			List<PostComment> lstContents = new List<PostComment>();

			using (ISiteNavHelper navHelper = SiteNavFactory.GetSiteNavHelper()) {
				SiteNav sn = navHelper.FindByFilename(SiteData.CurrentSiteID, SiteData.CurrentScriptName);

				if (sn != null) {
					TotalRecords = PostComment.GetCommentCountByContent(sn.Root_ContentID, !SecurityData.IsAuthEditor);
					lstContents = PostComment.GetCommentsByContentPageNumber(sn.Root_ContentID, this.PageNumberZeroIndex, this.PageSize, this.OrderBy, !SecurityData.IsAuthEditor);
				}
			}

			lstContents.ToList().ForEach(q => CMSConfigHelper.IdentifyLinkAsInactive(q));

			this.DataSource = lstContents;
		}
	}
}