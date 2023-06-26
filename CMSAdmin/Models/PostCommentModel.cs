using Carrotware.CMS.Core;
using System;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.CoreMVC.UI.Admin.Models {

	public class PostCommentModel {

		public enum ViewType {
			Unknown,
			PageView,
			BlogIndex,
			ContentIndex
		}

		public PostCommentModel() {
			this.ViewMode = ViewType.Unknown;
		}

		public PostCommentModel(PostComment comment, ViewType viewType)
			: this() {
			this.Comment = comment;
			this.ViewMode = viewType;
			this.Root_ContentID = this.Comment.Root_ContentID;
		}

		public PostComment Comment { get; set; }

		public ViewType ViewMode { get; set; }

		public Guid Root_ContentID { get; set; }
	}
}