using CarrotCake.CMS.Plugins.FAQ2.Data;
using Carrotware.Web.UI.Components;

namespace CarrotCake.CMS.Plugins.FAQ2.Models {

	public class FaqListing {

		public FaqListing() {
			this.Faq = new CarrotFaqCategory();
			this.Items = new PagedData<CarrotFaqItem>();

			this.Items.InitOrderBy(x => x.ItemOrder);
			this.Items.PageSize = 20;
		}

		public CarrotFaqCategory Faq { get; set; }

		public PagedData<CarrotFaqItem> Items { get; set; }
	}
}