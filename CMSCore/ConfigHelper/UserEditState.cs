using Carrotware.CMS.Interface;

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

	public class UserEditState {

		public UserEditState() { }

		public void Init() {
			this.EditorMargin = "L";
			this.EditorOpen = "true";
			this.EditorScrollPosition = "0";
			this.EditorWidgetScrollPosition = "0";
			this.EditorSelectedTabIdx = "0";
		}

		public string EditorMargin { get; set; }

		public string EditorOpen { get; set; }

		public string EditorScrollPosition { get; set; }

		public string EditorWidgetScrollPosition { get; set; }

		public string EditorSelectedTabIdx { get; set; }

		public static string ContentKey {
			get {
				if (SecurityData.IsAuthenticated) {
					return "cms_UserEditState_" + SecurityData.CurrentUser.UserName.ToLowerInvariant();
				} else {
					return "cms_UserEditState_anonymous";
				}
			}
		}

		//cache the settings but only long enough for the page to save & refresh
		public static UserEditState cmsUserEditState {
			get {
				UserEditState c = null;
				try { c = (UserEditState)CarrotHttpHelper.CacheGet(ContentKey); } catch { }
				return c;
			}
			set {
				if (value == null) {
					CarrotHttpHelper.CacheRemove(ContentKey);
				} else {
					CarrotHttpHelper.CacheInsert(ContentKey, value, 1);
				}
			}
		}
	}
}