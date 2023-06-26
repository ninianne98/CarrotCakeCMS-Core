using Carrotware.CMS.Data.Models;
using Microsoft.EntityFrameworkCore;
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

namespace Carrotware.CMS.Core {

	public class WidgetHelper : IDisposable {
		private CarrotCakeContext db = CarrotCakeContext.Create();
		//private CarrotCakeContext db = CompiledQueries.dbConn;

		public WidgetHelper() { }

		public Widget Get(Guid rootWidgetID) {
			return new Widget(rootWidgetID);
		}

		public List<Widget> GetWidgets(Guid rootContentID, bool bActiveOnly) {
			List<Widget> w = (from r in CompiledQueries.cqGetLatestWidgets(db, rootContentID, bActiveOnly)
							  select new Widget(r)).ToList();

			return w;
		}

		public List<Widget> GetWidgetVersionHistory(Guid rootWidgetID) {
			List<Widget> w = (from r in CompiledQueries.cqGetWidgetVersionHistory_VW(db, rootWidgetID)
							  select new Widget(r)).ToList();

			return w;
		}

		public Widget GetWidgetVersion(Guid widgetDataID) {
			Widget w = new Widget(CompiledQueries.cqGetWidgetDataByID_VW(db, widgetDataID));

			return w;
		}

		public void RemoveVersions(List<Guid> lstDel) {
			db.CarrotWidgetData.Where(w => lstDel.Contains(w.WidgetDataId)
											&& w.IsLatestVersion != true).ExecuteDelete();
			db.SaveChanges();
		}

		public void Delete(Guid widgetDataID) {
			CarrotWidgetData w = CompiledQueries.cqGetWidgetDataByID_TBL(db, widgetDataID);

			if (w != null) {
				db.CarrotWidgetData.Remove(w);
				db.SaveChanges();
			}
		}

		public void Disable(Guid rootWidgetID) {
			CarrotWidget w = CompiledQueries.cqGetRootWidget(db, rootWidgetID);

			if (w != null) {
				w.WidgetActive = false;
				db.SaveChanges();
			}
		}

		public void SetStatusList(Guid rootContentID, List<Guid> lstWidgetIDs, bool widgetStatus) {
			var query = (from w in CannedQueries.GetWidgetsByRootContent(db, rootContentID)
						 where lstWidgetIDs.Contains(w.RootWidgetId)
							   && w.WidgetActive != widgetStatus
						 select w);

			db.CarrotWidgets.Where(x => query.Select(x => x.RootWidgetId).Contains(x.RootWidgetId))
					.ExecuteUpdate(y => y.SetProperty(z => z.WidgetActive, widgetStatus));

			db.SaveChanges();
		}

		public void DeleteAll(Guid rootWidgetID) {
			var w = CompiledQueries.cqGetRootWidget(db, rootWidgetID);

			bool bPendingDel = false;

			if (w != null) {
				db.CarrotWidgetData.Where(x => x.RootWidgetId == rootWidgetID).ExecuteDelete();

				db.CarrotWidgets.Remove(w);
				bPendingDel = true;
			}

			if (bPendingDel) {
				db.SaveChanges();
			}
		}

		#region IDisposable Members

		public void Dispose() {
			if (db != null) {
				db.Dispose();
			}
		}

		#endregion IDisposable Members
	}
}