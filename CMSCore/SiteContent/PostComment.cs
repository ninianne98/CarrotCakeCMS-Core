using Carrotware.CMS.Data.Models;
using Carrotware.Web.UI.Components;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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

	public class PostComment {

		public PostComment() { }

		public Guid ContentCommentID { get; set; }
		public Guid Root_ContentID { get; set; }

		[Display(Name = "Date")]
		public DateTime CreateDate { get; set; }

		[Display(Name = "IP Addy")]
		[Required]
		[StringLength(32)]
		public string CommenterIP { get; set; }

		[Display(Name = "Commenter Name")]
		[Required]
		[StringLength(256)]
		public string CommenterName { get; set; }

		[Display(Name = "Commenter Email")]
		[Required]
		[StringLength(256)]
		public string CommenterEmail { get; set; }

		[Display(Name = "Commenter URL")]
		[StringLength(256)]
		public string CommenterURL { get; set; }

		[Display(Name = "Comment Text")]
		[Required]
		//[StringLength(4096)]
		public string PostCommentText { get; set; }

		private string _commentPlain = null;

		public IHtmlContent PostCommentEscaped {
			get {
				if (_commentPlain == null) {
					string cmt = this.PostCommentText ?? string.Empty;
					cmt = cmt.Replace("\r\n", "\n").Replace("<", " &lt; ").Replace(">", " &gt; ").Replace("\r", "\n").Replace("\n", "<br />");
					_commentPlain = SiteData.CurrentSite.UpdateContentComment(cmt);
				}

				return new HtmlString(_commentPlain);
			}
		}

		private string _commentPr = null;

		public IHtmlContent PostCommentProcessed {
			get {
				if (_commentPr == null) {
					string cmt = this.PostCommentText ?? string.Empty;
					_commentPr = SiteData.CurrentSite.UpdateContentComment(cmt);
				}

				return new HtmlString(_commentPr);
			}
		}

		[Display(Name = "Approved")]
		public bool IsApproved { get; set; }

		[Display(Name = "Spam")]
		public bool IsSpam { get; set; }

		[Display(Name = "Page Title")]
		public string NavMenuText { get; set; }

		[Display(Name = "Filename")]
		public string FileName { get; set; }

		public ContentPageType.PageType ContentType { get; set; }

		internal PostComment(vwCarrotComment c) {
			if (c != null) {
				this.ContentCommentID = c.ContentCommentId;
				this.Root_ContentID = c.RootContentId;
				this.CreateDate = SiteData.CurrentSite.ConvertUTCToSiteTime(c.CreateDate);
				this.CommenterIP = c.CommenterIp;
				this.CommenterName = c.CommenterName;
				this.CommenterEmail = c.CommenterEmail;
				this.CommenterURL = c.CommenterUrl;
				this.PostCommentText = c.PostComment;
				this.IsApproved = c.IsApproved;
				this.IsSpam = c.IsSpam;
				this.NavMenuText = c.NavMenuText;
				this.FileName = c.FileName;
				this.ContentType = ContentPageType.GetTypeByID(c.ContentTypeId);
			}
		}

		public void Delete() {
			using (var db = CarrotCakeContext.Create()) {
				var c = CompiledQueries.cqGetContentCommentsTblByID(db, this.ContentCommentID);

				if (c != null) {
					db.CarrotContentComments.Remove(c);
					db.SaveChanges();
				}
			}
		}

		public void Save() {
			using (var db = CarrotCakeContext.Create()) {
				bool bNew = false;
				var c = CompiledQueries.cqGetContentCommentsTblByID(db, this.ContentCommentID);

				if (c == null) {
					c = new CarrotContentComment();
					c.CreateDate = DateTime.UtcNow;
					bNew = true;

					if (this.CreateDate.Year > 1950) {
						c.CreateDate = SiteData.CurrentSite.ConvertSiteTimeToUTC(this.CreateDate);
					}
				}

				if (this.ContentCommentID == Guid.Empty) {
					this.ContentCommentID = Guid.NewGuid();
				}

				c.ContentCommentId = this.ContentCommentID;
				c.RootContentId = this.Root_ContentID;
				c.CommenterIp = this.CommenterIP;
				c.CommenterName = this.CommenterName;
				c.CommenterEmail = this.CommenterEmail;
				c.CommenterUrl = this.CommenterURL;
				c.PostComment = this.PostCommentText;
				c.IsApproved = this.IsApproved;
				c.IsSpam = this.IsSpam;

				if (bNew) {
					db.CarrotContentComments.Add(c);
				}

				db.SaveChanges();

				this.ContentCommentID = c.ContentCommentId;
				this.CreateDate = c.CreateDate;
			}
		}

		public static List<PostComment> GetCommentsByContentPage(Guid rootContentID, bool bActiveOnly) {
			using (var db = CarrotCakeContext.Create()) {
				IQueryable<vwCarrotComment> lstComments = (from c in CannedQueries.GetContentPageComments(db, rootContentID, bActiveOnly)
														   select c);

				return lstComments.Select(x => new PostComment(x)).ToList();
			}
		}

		public static List<PostComment> GetCommentsBySitePageNumber(Guid siteID, int iPageNbr, int iPageSize, string SortBy, ContentPageType.PageType pageType) {
			int startRec = iPageNbr * iPageSize;
			using (var db = CarrotCakeContext.Create()) {
				IQueryable<vwCarrotComment> lstComments = (from c in CannedQueries.GetSiteContentCommentsByPostType(db, siteID, pageType)
														   select c);

				return PaginateComments(lstComments, iPageNbr, iPageSize, SortBy).ToList();
			}
		}

		public static List<PostComment> GetCommentsByContentPageNumber(Guid rootContentID, int iPageNbr, int iPageSize, string SortBy, bool bActiveOnly) {
			int startRec = iPageNbr * iPageSize;
			using (var db = CarrotCakeContext.Create()) {
				IQueryable<vwCarrotComment> lstComments = (from c in CannedQueries.GetContentPageComments(db, rootContentID, bActiveOnly)
														   select c);

				return PaginateComments(lstComments, iPageNbr, iPageSize, SortBy).ToList();
			}
		}

		public static List<PostComment> GetCommentsBySitePageNumber(Guid siteID, int iPageNbr, int iPageSize, string SortBy, ContentPageType.PageType pageType, bool? approved, bool? spam) {
			int startRec = iPageNbr * iPageSize;
			using (var db = CarrotCakeContext.Create()) {
				IQueryable<vwCarrotComment> lstComments = (from c in CannedQueries.GetSiteContentCommentsByPostType(db, siteID, pageType, approved, spam)
														   select c);

				return PaginateComments(lstComments, iPageNbr, iPageSize, SortBy).ToList();
			}
		}

		public static List<PostComment> GetCommentsByContentPageNumber(Guid rootContentID, int iPageNbr, int iPageSize, string SortBy, bool? approved, bool? spam) {
			int startRec = iPageNbr * iPageSize;
			using (var db = CarrotCakeContext.Create()) {
				IQueryable<vwCarrotComment> lstComments = (from c in CannedQueries.GetContentPageComments(db, rootContentID, approved, spam)
														   select c);

				return PaginateComments(lstComments, iPageNbr, iPageSize, SortBy).ToList();
			}
		}

		public static List<PostComment> PaginateComments(IQueryable<vwCarrotComment> lstComments, int iPageNbr, int iPageSize, string SortBy) {
			int startRec = iPageNbr * iPageSize;

			SortParm srt = new SortParm(SortBy);

			if (string.IsNullOrEmpty(srt.SortField)) {
				srt.SortField = "CreateDate";
			}

			if (string.IsNullOrEmpty(srt.SortDirection)) {
				srt.SortDirection = "DESC";
			}

			lstComments = lstComments.SortByParm(srt.SortField, srt.SortDirection);

			return lstComments.Skip(startRec).Take(iPageSize).ToList().Select(v => new PostComment(v)).ToList();
		}

		public static int GetCommentCountBySiteAndType(Guid siteID, ContentPageType.PageType pageType) {
			using (var db = CarrotCakeContext.Create()) {
				return (from c in CannedQueries.GetSiteContentCommentsByPostType(db, siteID, pageType)
						select c).Count();
			}
		}

		public static int GetCommentCountBySiteAndType(Guid siteID, ContentPageType.PageType pageType, bool? approved, bool? spam) {
			using (var db = CarrotCakeContext.Create()) {
				return (from c in CannedQueries.GetSiteContentCommentsByPostType(db, siteID, pageType, approved, spam)
						select c).Count();
			}
		}

		public static int GetCommentCountByContent(Guid rootContentID, bool bActiveOnly) {
			using (var db = CarrotCakeContext.Create()) {
				return (from c in CannedQueries.GetContentPageComments(db, rootContentID, bActiveOnly)
						select c).Count();
			}
		}

		public static int GetCommentCountByContent(Guid rootContentID, bool? approved, bool? spam) {
			using (var db = CarrotCakeContext.Create()) {
				return (from c in CannedQueries.GetContentPageComments(db, rootContentID, approved, spam)
						select c).Count();
			}
		}

		public static int GetCommentCountByContent(Guid siteID, Guid rootContentID, DateTime postDate, string postIP, string sCommentText) {
			using (var db = CarrotCakeContext.Create()) {
				return (from c in CannedQueries.FindCommentsByDate(db, siteID, rootContentID, postDate, postIP, sCommentText)
						select c).Count();
			}
		}

		public static int GetCommentCountByContent(Guid siteID, Guid rootContentID, DateTime postDate, string postIP) {
			using (var db = CarrotCakeContext.Create()) {
				return (from c in CannedQueries.FindCommentsByDate(db, siteID, rootContentID, postDate, postIP)
						select c).Count();
			}
		}

		public static PostComment GetContentCommentByID(Guid contentCommentID) {
			using (var db = CarrotCakeContext.Create()) {
				return new PostComment(CompiledQueries.cqGetContentCommentByID(db, contentCommentID));
			}
		}

		public static int GetAllCommentCountBySite(Guid siteID) {
			using (var db = CarrotCakeContext.Create()) {
				return (from c in CannedQueries.GetSiteContentComments(db, siteID)
						select c).Count();
			}
		}

		public static List<PostComment> GetAllCommentsBySite(Guid siteID) {
			using (var db = CarrotCakeContext.Create()) {
				IQueryable<PostComment> s = (from c in CannedQueries.GetSiteContentComments(db, siteID)
											 select new PostComment(c));

				return s.ToList();
			}
		}

		public override bool Equals(object? obj) {
			//Check for null and compare run-time types.
			if (obj == null || this.GetType() != obj.GetType()) return false;

			if (obj is PostComment) {
				PostComment p = (PostComment)obj;
				return (this.ContentCommentID == p.ContentCommentID)
					&& (this.Root_ContentID == p.Root_ContentID);
			} else {
				return false;
			}
		}

		public override int GetHashCode() {
			return this.ContentCommentID.GetHashCode() ^ this.Root_ContentID.GetHashCode();
		}
	}
}