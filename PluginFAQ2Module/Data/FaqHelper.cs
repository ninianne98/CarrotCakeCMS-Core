using Microsoft.EntityFrameworkCore;
using System.Reflection;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2024, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: April 2024
*/

namespace CarrotCake.CMS.Plugins.FAQ2.Data {

	public class FaqHelper : IDisposable {
		protected FaqContext db = FaqContext.GetDataContext();

		public FaqHelper() { }

		public FaqHelper(Guid siteId) {
			this.SiteId = siteId;
		}

		public Guid SiteId { get; set; }

		public CarrotFaqItem? FaqItemGetByID(Guid faqItemId) {
			var ff = (from f in db.CarrotFaqItems
					  where f.FaqItemId == faqItemId
					  select f).FirstOrDefault();

			return ff;
		}

		public List<CarrotFaqItem> FaqItemListGetByFaqCategoryID(Guid faqCategoryId) {
			var ff = (from f in db.CarrotFaqItems
					  where f.FaqCategoryId == faqCategoryId
					  orderby f.ItemOrder
					  select f).ToList();

			return ff;
		}

		public List<CarrotFaqItem> FaqItemListPublicGetByFaqCategoryID(Guid faqCategoryId, Guid siteId) {
			var ff = (from f in db.CarrotFaqItems
					  join fc in db.CarrotFaqCategories on f.FaqCategoryId equals fc.FaqCategoryId
					  where f.FaqCategoryId == faqCategoryId
							   && f.IsActive == true && fc.SiteId == siteId
					  orderby f.ItemOrder
					  select f).ToList();

			return ff;
		}

		public List<CarrotFaqItem> FaqItemListPublicTopGetByFaqCategoryID(Guid faqCategoryId, Guid siteId, int takeCount) {
			if (takeCount < 0) {
				takeCount = 1;
			}

			if (takeCount > 100) {
				takeCount = 100;
			}

			var ff = (from f in db.CarrotFaqItems
					  join fc in db.CarrotFaqCategories on f.FaqCategoryId equals fc.FaqCategoryId
					  where f.FaqCategoryId == faqCategoryId
							   && f.IsActive == true && fc.SiteId == siteId
					  orderby f.ItemOrder
					  select f).Take(takeCount).ToList();

			return ff;
		}

		public CarrotFaqItem FaqItemListPublicRandGetByFaqCategoryID(Guid faqCategoryId, Guid siteId) {
			Random rand = new Random();

			int toSkip = rand.Next(0, (from f in db.CarrotFaqItems
									   join fc in db.CarrotFaqCategories on f.FaqCategoryId equals fc.FaqCategoryId
									   where f.FaqCategoryId == faqCategoryId
												&& f.IsActive == true && fc.SiteId == siteId
									   orderby f.ItemOrder
									   select f).Count());

			var ff = (from f in db.CarrotFaqItems
					  join fc in db.CarrotFaqCategories on f.FaqCategoryId equals fc.FaqCategoryId
					  where f.FaqCategoryId == faqCategoryId
							   && f.IsActive == true && fc.SiteId == siteId
					  orderby f.ItemOrder
					  select f).Skip(toSkip).FirstOrDefault();

			return ff;
		}

		public void FAQImageCleanup(Guid faqCategoryId, List<Guid> lst) {
			var lstDel = (from f in db.CarrotFaqItems
						  where f.FaqCategoryId == faqCategoryId
								&& !lst.Contains(f.FaqItemId)
						  select f.FaqItemId).ToList();

			if (lstDel.Any()) {
				db.CarrotFaqItems.Where(x => lstDel.Contains(x.FaqItemId)).ExecuteDelete();
				db.SaveChanges();
			}
		}

		public CarrotFaqCategory CategoryGetByID(Guid faqCategoryId) {
			var ge = (from c in db.CarrotFaqCategories
					  where c.SiteId == this.SiteId
							 && c.FaqCategoryId == faqCategoryId
					  select c).FirstOrDefault();

			return ge;
		}

		public List<CarrotFaqCategory> CategoryListGetBySiteID() {
			return CategoryListGetBySiteID(this.SiteId);
		}

		public List<CarrotFaqCategory> CategoryListGetBySiteID(Guid siteId) {
			var ge = (from c in db.CarrotFaqCategories
					  orderby c.FaqTitle
					  where c.SiteId == siteId
					  select c).ToList();

			return ge;
		}

		public CarrotFaqCategory Save(CarrotFaqCategory item) {
			if (item.FaqCategoryId == Guid.Empty) {
				item.FaqCategoryId = Guid.NewGuid();
			}

			if (!db.CarrotFaqCategories.Where(x => x.FaqCategoryId == item.FaqCategoryId).Any()) {
				db.CarrotFaqCategories.Add(item);
			}

			db.SaveChanges();

			return item;
		}

		public CarrotFaqItem Save(CarrotFaqItem item) {
			if (item.FaqItemId == Guid.Empty) {
				item.FaqItemId = Guid.NewGuid();
			}

			if (!db.CarrotFaqItems.Where(x => x.FaqItemId == item.FaqItemId).Any()) {
				db.CarrotFaqItems.Add(item);
			}

			db.SaveChanges();

			return item;
		}

		public bool DeleteItem(Guid ItemGuid) {
			var itm = (from c in db.CarrotFaqItems
					   where c.FaqItemId == ItemGuid
					   select c).FirstOrDefault();

			db.CarrotFaqItems.Remove(itm);

			db.SaveChanges();

			return true;
		}

		public static string ReadEmbededScript(string sResouceName) {
			string sReturn = null;

			Assembly _assembly = Assembly.GetExecutingAssembly();
			using (var stream = new StreamReader(_assembly.GetManifestResourceStream(sResouceName))) {
				sReturn = stream.ReadToEnd();
			}

			return sReturn;
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