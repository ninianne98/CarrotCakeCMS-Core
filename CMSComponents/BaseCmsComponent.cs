﻿using Carrotware.CMS.Core;
using Carrotware.CMS.Interface;
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

	public abstract class BaseCmsComponent : BaseWebComponent, ICmsChildrenComponent, ICmsMainComponent, IWidget {
		protected ISiteNavHelper navHelper = SiteNavFactory.GetSiteNavHelper();
		protected HtmlTag _topTag = new HtmlTag("ul");

		public BaseCmsComponent()
			: base() {
			LoadData();

			try {
				var name = this.GetType().ToString().Split('.');
				this.ElementId = name[name.Length - 1].ToLowerInvariant();
			} catch {
				this.ElementId = "component";
			}

			this.CssSelected = "selected";
			this.CssAnchor = string.Empty;
			this.CssULClassTop = "parent";
			this.CssULClassLower = "children";
			this.CssHasChildren = "sub";

			this.SiteID = SiteData.CurrentSite.SiteID;
		}

		public virtual bool MultiLevel {
			get {
				return false;
			}
		}

		[Widget(WidgetAttribute.FieldMode.TextBox)]
		public string CssClass { get; set; }

		public string CssItem { get; set; }
		public string CssAnchor { get; set; }

		[Widget(WidgetAttribute.FieldMode.TextBox)]
		public string CssSelected { get; set; }

		[Widget(WidgetAttribute.FieldMode.TextBox)]
		public string CssULClassTop { get; set; }

		[Widget(WidgetAttribute.FieldMode.TextBox)]
		public string CssULClassLower { get; set; }

		[Widget(WidgetAttribute.FieldMode.TextBox)]
		public string CssHasChildren { get; set; }

		public object ItemAttributes { get; set; }
		public object AnchorAttributes { get; set; }
		public object SelectedAttributes { get; set; }
		public object ULClassTopAttributes { get; set; }
		public object ULClassLowerAttributes { get; set; }
		public object HasChildrenAttributes { get; set; }

		public void SetItemAttributes(object attribute) {
			this.ItemAttributes = attribute;
		}

		public void SetAnchorAttributes(object attribute) {
			this.AnchorAttributes = attribute;
		}

		public void SetSelectedAttributes(object attribute) {
			this.SelectedAttributes = attribute;
		}

		public void SetULClassTopAttributes(object attribute) {
			this.ULClassTopAttributes = attribute;
		}

		public void SetULClassLowerAttributes(object attribute) {
			this.ULClassLowerAttributes = attribute;
		}

		public void SetHasChildrenAttributes(object attribute) {
			this.HasChildrenAttributes = attribute;
		}

		public string ElementId { get; set; }

		public List<SiteNav> NavigationData { get; set; }

		//================

		public Guid PageWidgetID { get; set; }

		public Guid RootContentID { get; set; }

		public Guid SiteID { get; set; }

		[Widget(WidgetAttribute.FieldMode.TextBox)]
		public string WidgetClientID { get; set; }

		public virtual bool EnableEdit {
			get { return false; }
		}

		public bool IsBeingEdited { get; set; }

		public bool IsDynamicInserted { get; set; }

		public Dictionary<string, string> PublicParmValues { get; set; }

		public virtual Dictionary<string, string> JSEditFunctions {
			get {
				return new Dictionary<string, string>();
			}
		}

		void IWidget.LoadData() {
			if (this.PublicParmValues == null) {
				this.PublicParmValues = new Dictionary<string, string>();
			}

			this.LoadData();
		}

		#region Common Parser Routines

		public string GetParmValue(string sKey) {
			return this.PublicParmValues.GetParmValue(sKey, sKey);
		}

		public string GetParmValue(string sKey, string sDefault) {
			return this.PublicParmValues.GetParmValue(sKey, sDefault);
		}

		public string GetParmValue(string sKey, bool bDefault) {
			return this.PublicParmValues.GetParmValue(sKey, bDefault);
		}

		public string GetParmValue(string sKey, int iDefault) {
			return this.PublicParmValues.GetParmValue(sKey, iDefault);
		}

		public string GetParmValueDefaultEmpty(string sKey, string sDefault) {
			return this.PublicParmValues.GetParmValueDefaultEmpty(sKey, sDefault);
		}

		public List<string> GetParmValueList(string sKey) {
			return this.PublicParmValues.GetParmValueList(sKey);
		}

		#endregion Common Parser Routines

		//================

		protected virtual void LoadData() {
			_topTag = new HtmlTag("ul");
			this.NavigationData = new List<SiteNav>();
		}

		protected virtual void TweakData() {
			this.NavigationData = ControlUtilities.TweakData(this.NavigationData);
		}

		public virtual List<SiteNav> GetTopNav() {
			if (this.MultiLevel) {
				return this.NavigationData.Where(ct => ct.Parent_ContentID == null).OrderBy(ct => ct.NavMenuText).OrderBy(ct => ct.NavOrder).ToList();
			} else {
				return this.NavigationData.OrderBy(ct => ct.NavMenuText).OrderBy(ct => ct.NavOrder).ToList();
			}
		}

		public virtual List<SiteNav> GetChildren(Guid rootContentID) {
			return this.NavigationData.Where(ct => ct.Parent_ContentID == rootContentID).OrderBy(ct => ct.NavMenuText).OrderBy(ct => ct.NavOrder).ToList();
		}

		protected SiteNav IsContained(List<SiteNav> navCrumbs, Guid rootContentID) {
			return navCrumbs.Where(ct => ct.Root_ContentID == rootContentID && ct.NavOrder > 0).FirstOrDefault();
		}

		protected StringBuilder WriteListPrefix(StringBuilder output) {
			_topTag = new HtmlTag("ul");
			_topTag.SetAttribute("id", this.ElementId);
			_topTag.MergeAttribute("class", this.CssULClassTop);
			_topTag.MergeAttribute("class", this.CssClass);
			_topTag.MergeAttributes(this.ULClassTopAttributes);

			output.Append(_topTag.OpenTag());

			return output;
		}

		protected StringBuilder WriteListSuffix(StringBuilder output) {
			output.Append(_topTag.CloseTag());
			return output;
		}

		protected string ParentFileName { get; set; }

		private int _itemNumber = 0;

		protected virtual StringBuilder WriteTopLevel(StringBuilder output) {
			List<SiteNav> lstNav = GetTopNav();
			SiteNav parentPageNav = ControlUtilities.GetParentPage();
			List<SiteNav> lstNavTree = ControlUtilities.GetPageNavTree().OrderByDescending(x => x.NavOrder).ToList();

			this.ParentFileName = parentPageNav.FileName.ToLowerInvariant();

			if (lstNav != null && lstNav.Any()) {
				WriteListPrefix(output);

				foreach (SiteNav c1 in lstNav) {
					List<SiteNav> cc = GetChildren(c1.Root_ContentID);

					var item = new HtmlTag("li");
					var link = new HtmlTag("a");

					item.MergeAttribute("class", this.CssItem);
					item.MergeAttributes(this.ItemAttributes);

					link.MergeAttribute("class", this.CssAnchor);
					link.MergeAttributes(this.AnchorAttributes);

					if (this.MultiLevel) {
						item.MergeAttribute("class", "level1");

						if (cc != null && cc.Any()) {
							item.MergeAttribute("class", string.Format(" level1-haschildren {0}", this.CssHasChildren));
							item.MergeAttributes(this.HasChildrenAttributes);
						}
					}

					if (SiteData.IsFilenameCurrentPage(c1.FileName)
							|| (c1.NavOrder == 0 && SiteData.IsCurrentLikelyHomePage)
							|| (IsContained(lstNavTree, c1.Root_ContentID) != null)
							|| ControlUtilities.AreFilenamesSame(c1.FileName, this.ParentFileName)) {
						item.MergeAttribute("class", this.CssSelected);
						item.MergeAttributes(this.SelectedAttributes);

						link.MergeAttribute("class", this.CssSelected);
						link.MergeAttributes(this.SelectedAttributes);
					}

					_itemNumber++;

					item.SetAttribute("id", string.Format("listitem{0}", _itemNumber));

					output.Append(item.OpenTag());

					link.Uri = c1.FileName;
					link.InnerHtml = c1.NavMenuText;

					output.Append(link.RenderTag());

					if (this.MultiLevel && cc != null && cc.Any()) {
						LoadChildren(output, c1.Root_ContentID, _itemNumber, 2);
					}

					output.Append(item.CloseTag());
					output.AppendLine();
				}

				WriteListSuffix(output);
			} else {
				var item = new HtmlTag("span");
				item.SetAttribute("id", this.ElementId);
				item.MergeAttribute("style", "display: none;");
				output.AppendLine(item.RenderTag());
			}

			return output;
		}

		protected virtual StringBuilder LoadChildren(StringBuilder output, Guid rootContentID, int iParent, int iLevel) {
			List<SiteNav> lstNav = GetChildren(rootContentID);

			if (lstNav != null && lstNav.Any()) {
				var childList = new HtmlTag("ul");

				childList.SetAttribute("id", string.Format("listitem{0}-childlist", iParent));
				childList.MergeAttribute("class", string.Format("childlist childlevel{0}", iLevel));
				childList.MergeAttribute("class", this.CssULClassLower);
				childList.MergeAttributes(this.ULClassLowerAttributes);

				output.Append(childList.OpenTag());

				foreach (SiteNav c2 in lstNav) {
					List<SiteNav> cc = GetChildren(c2.Root_ContentID);
					var childItem = new HtmlTag("li");
					var childLink = new HtmlTag("a");

					if (this.MultiLevel) {
						childItem.MergeAttribute("class", string.Format("level{0}", iLevel));

						if (cc != null && cc.Any()) {
							childItem.MergeAttribute("class", string.Format("level{0}-haschildren {1}", iLevel, this.CssHasChildren));
							childItem.MergeAttributes(this.HasChildrenAttributes);
						}
					}

					if (SiteData.IsFilenameCurrentPage(c2.FileName)
							|| ControlUtilities.AreFilenamesSame(c2.FileName, this.ParentFileName)) {
						childItem.MergeAttribute("class", this.CssSelected);
					}

					_itemNumber++;

					childItem.SetAttribute("id", string.Format("listitem{0}", _itemNumber));
					childItem.MergeAttribute("class", "child-nav");
					output.Append(childItem.OpenTag());

					childLink.Uri = c2.FileName;
					childLink.InnerHtml = c2.NavMenuText;
					output.Append(childLink.RenderTag());

					if (cc != null && cc.Any()) {
						LoadChildren(output, c2.Root_ContentID, _itemNumber, iLevel + 1);
					}

					output.AppendLine(childItem.CloseTag());
					output.AppendLine();
				}

				output.AppendLine(childList.CloseTag());
			}

			return output;
		}

		public override string GetHtml() {
			TweakData();

			var output = new StringBuilder();
			output = WriteTopLevel(output);

			return ControlUtilities.HtmlFormat(output);
		}
	}
}