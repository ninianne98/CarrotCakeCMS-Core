using Carrotware.Web.UI.Components;
using Northwind.Code;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Models {

	public class SelectSkin {

		public SelectSkin() {
			this.SelectedItem = Helper.UseBootstrap5 == false ? Helper.BootstrapColorScheme : Bootstrap.BootstrapColorScheme.NotUsed;
			this.SelectedItem5 = Helper.UseBootstrap5 == true ? Helper.Bootstrap5ColorScheme : Bootstrap5.Bootstrap5ColorScheme.NotUsed;
			this.UseBootstrap5 = Helper.UseBootstrap5;

			var options3 = new List<Skin3Item>();
			var options5 = new List<Skin5Item>();

			foreach (Bootstrap.BootstrapColorScheme enumValue in Enum.GetValues(typeof(Bootstrap.BootstrapColorScheme))) {
				options3.Add(new Skin3Item(enumValue));
			}

			foreach (Bootstrap5.Bootstrap5ColorScheme enumValue in Enum.GetValues(typeof(Bootstrap5.Bootstrap5ColorScheme))) {
				options5.Add(new Skin5Item(enumValue));
			}

			this.Options = options3.OrderBy(x => x.SkinName).ToList();
			this.Options5 = options5.OrderBy(x => x.SkinName).ToList();
		}

		[Display(Name = "Use Bootstrap 5")]
		public bool UseBootstrap5 { get; set; } = true;

		[Display(Name = "Option 3 List")]
		public List<Skin3Item> Options { get; set; }

		[Display(Name = "Option 5 List")]
		public List<Skin5Item> Options5 { get; set; }

		[Display(Name = "Selected 3 Skin")]
		public Bootstrap.BootstrapColorScheme SelectedItem { get; set; }

		[Display(Name = "Selected 5 Skin")]
		public Bootstrap5.Bootstrap5ColorScheme SelectedItem5 { get; set; }
	}
}