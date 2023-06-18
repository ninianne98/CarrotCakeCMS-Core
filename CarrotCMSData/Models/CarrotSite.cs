using System;
using System.Collections.Generic;

namespace Carrotware.CMS.Data.Models {
	public partial class CarrotSite {
		public CarrotSite() {
			CarrotContentCategories = new HashSet<CarrotContentCategory>();
			CarrotContentTags = new HashSet<CarrotContentTag>();
			CarrotRootContentSnippets = new HashSet<CarrotRootContentSnippet>();
			CarrotRootContents = new HashSet<CarrotRootContent>();
			CarrotTextWidgets = new HashSet<CarrotTextWidget>();
			CarrotUserSiteMappings = new HashSet<CarrotUserSiteMapping>();
		}

		public Guid SiteId { get; set; }
		public string? MetaKeyword { get; set; }
		public string? MetaDescription { get; set; }
		public string? SiteName { get; set; }
		public string? MainUrl { get; set; }
		public bool BlockIndex { get; set; }
		public string? SiteTagline { get; set; }
		public string? SiteTitlebarPattern { get; set; }
		public Guid? BlogRootContentId { get; set; }
		public string? BlogFolderPath { get; set; }
		public string? BlogCategoryPath { get; set; }
		public string? BlogTagPath { get; set; }
		public string? BlogDatePath { get; set; }
		public string? BlogDatePattern { get; set; }
		public string? TimeZone { get; set; }
		public bool SendTrackbacks { get; set; }
		public bool AcceptTrackbacks { get; set; }
		public string? BlogEditorPath { get; set; }

		public virtual ICollection<CarrotContentCategory> CarrotContentCategories { get; set; }
		public virtual ICollection<CarrotContentTag> CarrotContentTags { get; set; }
		public virtual ICollection<CarrotRootContentSnippet> CarrotRootContentSnippets { get; set; }
		public virtual ICollection<CarrotRootContent> CarrotRootContents { get; set; }
		public virtual ICollection<CarrotTextWidget> CarrotTextWidgets { get; set; }
		public virtual ICollection<CarrotUserSiteMapping> CarrotUserSiteMappings { get; set; }
	}
}
