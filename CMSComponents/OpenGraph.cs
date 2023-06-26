using Carrotware.CMS.Core;
using Carrotware.Web.UI.Components;
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

	public class OpenGraph : BaseWebComponent {
		internal OpenGraph() {
			this.OpenGraphType = OpenGraphTypeDef.Default;
			this.ShowExpirationDate = false;
			this.CmsPage = new PagePayload();
		}

		public OpenGraph(PagePayload pp)
			: this() {
			this.CmsPage = pp;
		}

		public enum OpenGraphTypeDef {
			Default,
			Article,
			Blog,
			Website,
			Book,
			Video,
			Movie,
			Profile
		}

		public bool ShowExpirationDate { get; set; }

		public OpenGraphTypeDef OpenGraphType { get; set; }

		public PagePayload CmsPage { get; set; }

		public override string ToString() {
			return this.ToHtmlString();
		}

		public override string ToHtmlString() {
			var sb = new StringBuilder();
			sb.AppendLine(String.Empty);

			try {
				if (this.CmsPage != null) {
					if (!String.IsNullOrEmpty(this.CmsPage.ThePage.MetaDescription)) {
						sb.AppendLine(CarrotWebHelper.MetaTag("og:description", this.CmsPage.ThePage.MetaDescription).ToString());
					}
					sb.AppendLine(CarrotWebHelper.MetaTag("og:url", this.CmsPage.TheSite.DefaultCanonicalURL).ToString());

					string contType = OpenGraphTypeDef.Default.ToString();

					if (this.OpenGraphType == OpenGraphTypeDef.Default) {
						if (this.CmsPage.ThePage.ContentType == ContentPageType.PageType.BlogEntry) {
							contType = OpenGraphTypeDef.Blog.ToString().ToLowerInvariant();
						} else {
							contType = OpenGraphTypeDef.Article.ToString().ToLowerInvariant();
						}
						if (this.CmsPage.TheSite.Blog_Root_ContentID.HasValue && this.CmsPage.ThePage.Root_ContentID == this.CmsPage.TheSite.Blog_Root_ContentID) {
							contType = OpenGraphTypeDef.Website.ToString().ToLowerInvariant();
						}
					} else {
						contType = this.OpenGraphType.ToString().ToLowerInvariant();
					}

					sb.AppendLine(CarrotWebHelper.MetaTag("og:type", contType).ToString());

					if (!String.IsNullOrEmpty(this.CmsPage.ThePage.TitleBar)) {
						sb.AppendLine(CarrotWebHelper.MetaTag("og:title", this.CmsPage.ThePage.TitleBar).ToString());
					}

					if (!String.IsNullOrEmpty(this.CmsPage.ThePage.Thumbnail)) {
						sb.AppendLine(CarrotWebHelper.MetaTag("og:image", String.Format("{0}/{1}", this.CmsPage.TheSite.MainCanonicalURL, this.CmsPage.ThePage.Thumbnail).Replace(@"//", @"/").Replace(@"//", @"/").Replace(@":/", @"://")).ToString());
					}

					if (!String.IsNullOrEmpty(this.CmsPage.TheSite.SiteName)) {
						sb.AppendLine(CarrotWebHelper.MetaTag("og:site_name", this.CmsPage.TheSite.SiteName).ToString());
					}

					sb.AppendLine(CarrotWebHelper.MetaTag("article:published_time", this.CmsPage.TheSite.ConvertSiteTimeToISO8601(this.CmsPage.ThePage.GoLiveDate)).ToString());
					sb.AppendLine(CarrotWebHelper.MetaTag("article:modified_time", this.CmsPage.TheSite.ConvertSiteTimeToISO8601(this.CmsPage.ThePage.EditDate)).ToString());

					if (this.ShowExpirationDate) {
						sb.AppendLine(CarrotWebHelper.MetaTag("article:expiration_time", this.CmsPage.TheSite.ConvertSiteTimeToISO8601(this.CmsPage.ThePage.RetireDate)).ToString());
					}
				}
			} catch (Exception ex) { }

			return sb.ToString();
		}
	}
}