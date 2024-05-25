using static Carrotware.Web.UI.Components.Bootstrap;
using static Carrotware.Web.UI.Components.Bootstrap5;

namespace Northwind.Models {

	public class Skin3Item {

		public Skin3Item() {
			this.Skin = BootstrapColorScheme.NotUsed;
			this.SkinName = this.Skin.ToString();
		}

		public Skin3Item(BootstrapColorScheme skin) {
			this.Skin = skin;
			this.SkinName = this.Skin.ToString();
		}

		public BootstrapColorScheme Skin { get; set; }
		public string SkinName { get; set; }
	}

	//=============================

	public class Skin5Item {

		public Skin5Item() {
			this.Skin = Bootstrap5ColorScheme.NotUsed;
			this.SkinName = this.Skin.ToString();
		}

		public Skin5Item(Bootstrap5ColorScheme skin) {
			this.Skin = skin;
			this.SkinName = this.Skin.ToString();
		}

		public Bootstrap5ColorScheme Skin { get; set; }
		public string SkinName { get; set; }
	}
}