using CarrotCake.CMS.Plugins.FAQ2.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CarrotCake.CMS.Plugins.FAQ2.Models {

	public class FaqItems {

		public FaqItems() {
			this.Faq = new CarrotFaqCategory();
			this.Items = new List<CarrotFaqItem>();

			iFaq = 1;
		}

		protected int iFaq = 1;

		[Description("#")]
		[Display(Name = "Count")]
		public int FaqCount {
			get {
				return iFaq++;
			}
		}

		public string FaqCounter() {
			return this.FaqCount.ToString();
		}

		public CarrotFaqCategory Faq { get; set; } = new CarrotFaqCategory();

		public List<CarrotFaqItem> Items { get; set; } = new List<CarrotFaqItem>();
	}
}