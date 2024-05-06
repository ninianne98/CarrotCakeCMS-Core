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
		protected FaqContext _db;

		public FaqHelper(Guid siteId) {
			this.SiteId = siteId;

			_db = FaqContext.GetDataContext();
		}

		public Guid SiteId { get; set; }

		public CarrotFaqItem? FaqItemGetByID(Guid faqItemId) {
			return (from f in _db.CarrotFaqItems
					where f.FaqItemId == faqItemId
					select f).FirstOrDefault();
		}

		public IQueryable<CarrotFaqItem> FaqItemListGetByFaqCategoryID(Guid faqCategoryId) {
			return (from f in _db.CarrotFaqItems
					where f.FaqCategoryId == faqCategoryId
					orderby f.ItemOrder
					select f);
		}

		public int FaqItemListGetByFaqCategoryIDCount(Guid faqCategoryId) {
			return FaqItemListGetByFaqCategoryID(faqCategoryId).Count();
		}

		public List<CarrotFaqItem> FaqItemListPublicGetByFaqCategoryID(Guid faqCategoryId) {
			return (from f in _db.CarrotFaqItems
					join fc in _db.CarrotFaqCategories on f.FaqCategoryId equals fc.FaqCategoryId
					where f.FaqCategoryId == faqCategoryId
							 && f.IsActive == true && fc.SiteId == this.SiteId
					orderby f.ItemOrder
					select f).ToList();
		}

		public List<CarrotFaqItem> FaqItemListPublicTopGetByFaqCategoryID(Guid faqCategoryId, int takeCount) {
			if (takeCount < 0) {
				takeCount = 1;
			}

			if (takeCount > 100) {
				takeCount = 100;
			}

			return (from f in _db.CarrotFaqItems
					join fc in _db.CarrotFaqCategories on f.FaqCategoryId equals fc.FaqCategoryId
					where f.FaqCategoryId == faqCategoryId
							 && f.IsActive == true && fc.SiteId == this.SiteId
					orderby f.ItemOrder
					select f).Take(takeCount).ToList();
		}

		public CarrotFaqItem? FaqItemListPublicRandGetByFaqCategoryID(Guid faqCategoryId) {
			Random rand = new Random();

			int toSkip = rand.Next(0, (from f in _db.CarrotFaqItems
									   join fc in _db.CarrotFaqCategories on f.FaqCategoryId equals fc.FaqCategoryId
									   where f.FaqCategoryId == faqCategoryId
												&& f.IsActive == true && fc.SiteId == this.SiteId
									   orderby f.ItemOrder
									   select f).Count());

			return (from f in _db.CarrotFaqItems
					join fc in _db.CarrotFaqCategories on f.FaqCategoryId equals fc.FaqCategoryId
					where f.FaqCategoryId == faqCategoryId
							 && f.IsActive == true && fc.SiteId == this.SiteId
					orderby f.ItemOrder
					select f).Skip(toSkip).FirstOrDefault();
		}

		public void FAQImageCleanup(Guid faqCategoryId, List<Guid> lst) {
			var lstDel = (from f in _db.CarrotFaqItems
						  where f.FaqCategoryId == faqCategoryId
								&& !lst.Contains(f.FaqItemId)
						  select f.FaqItemId).ToList();

			if (lstDel.Any()) {
				_db.CarrotFaqItems.Where(x => lstDel.Contains(x.FaqItemId)).ExecuteDelete();
				_db.SaveChanges();
			}
		}

		public CarrotFaqCategory? CategoryGetByID(Guid faqCategoryId) {
			return (from c in _db.CarrotFaqCategories
					where c.SiteId == this.SiteId
						   && c.FaqCategoryId == faqCategoryId
					select c).FirstOrDefault();
		}

		public IQueryable<CarrotFaqCategory> CategoryListGetBySiteID() {
			return (from c in _db.CarrotFaqCategories
					orderby c.FaqTitle
					where c.SiteId == this.SiteId
					select c);
		}

		public int CategoryListGetBySiteIDCount() {
			return CategoryListGetBySiteID().Count();
		}

		public CarrotFaqCategory Save(CarrotFaqCategory item) {
			if (item.FaqCategoryId == Guid.Empty) {
				item.FaqCategoryId = Guid.NewGuid();
			}

			if (!_db.CarrotFaqCategories.Where(x => x.FaqCategoryId == item.FaqCategoryId).Any()) {
				_db.CarrotFaqCategories.Add(item);
			}

			_db.SaveChanges();

			return item;
		}

		public CarrotFaqItem Save(CarrotFaqItem item) {
			if (item.FaqItemId == Guid.Empty) {
				item.FaqItemId = Guid.NewGuid();
			}

			if (!_db.CarrotFaqItems.Where(x => x.FaqItemId == item.FaqItemId).Any()) {
				_db.CarrotFaqItems.Add(item);
			}

			_db.SaveChanges();

			return item;
		}

		public bool DeleteItem(Guid ItemGuid) {
			var itm = (from c in _db.CarrotFaqItems
					   where c.FaqItemId == ItemGuid
					   select c).FirstOrDefault();

			if (itm != null) {
				_db.CarrotFaqItems.Remove(itm);
				_db.SaveChanges();
			}

			return true;
		}

		public static string ReadEmbededScript(string resouceName) {
			string ret = null;

			Assembly assembly = Assembly.GetExecutingAssembly();
			using (var stream = new StreamReader(assembly.GetManifestResourceStream(resouceName))) {
				ret = stream.ReadToEnd();
			}

			return ret;
		}

		#region IDisposable Members

		public void Dispose() {
			if (_db != null) {
				_db.Dispose();
			}
		}

		#endregion IDisposable Members
	}
}