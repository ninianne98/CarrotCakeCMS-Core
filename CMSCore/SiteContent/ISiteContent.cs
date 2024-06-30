using Microsoft.AspNetCore.Html;

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

	public interface ISiteContent {
		Guid ContentID { get; set; }
		DateTime CreateDate { get; set; }
		DateTime GoLiveDate { get; set; }
		DateTime RetireDate { get; set; }
		DateTime EditDate { get; set; }
		Guid? EditUserId { get; set; }
		Guid CreateUserId { get; set; }
		string FileName { get; set; }
		string? Thumbnail { get; set; }
		string NavMenuText { get; set; }
		int NavOrder { get; set; }
		bool PageActive { get; set; }
		bool ShowInSiteMap { get; set; }
		bool BlockIndex { get; set; }
		string? PageHead { get; set; }
		string? PageText { get; set; }
		IHtmlContent PageTextPlainSummaryMedium { get; }
		IHtmlContent PageTextPlainSummary { get; }
		IHtmlContent NavigationText { get; }
		IHtmlContent HeadingText { get; }
		Guid? Parent_ContentID { get; set; }
		Guid Root_ContentID { get; set; }
		bool MadeSafe { get; set; }
		Guid SiteID { get; set; }
		bool ShowInSiteNav { get; set; }
		string TemplateFile { get; set; }
		string TitleBar { get; set; }
		ContentPageType.PageType ContentType { get; set; }

		bool IsRetired { get; }
		bool IsUnReleased { get; }

		List<ContentTag> ContentTags { get; set; }
		List<ContentCategory> ContentCategories { get; set; }

		ExtendedUserData GetUserInfo();

		ExtendedUserData GetCreditUserInfo();

		ExtendedUserData BylineUser { get; }
	}
}