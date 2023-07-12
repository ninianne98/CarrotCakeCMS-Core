namespace Northwind.Models {
	public class MathModel {

		public MathModel() {
			this.Number1 = 25;
			this.Number2 = 5;
		}

		public double Number1 { get; set; }

		public double Number2 { get; set; }

		public double Number3 { get; set; }

		public double GetResult() {
			if (this.Number2 > -0.2 && this.Number2 < 0.2) {
				this.Number3 = Convert.ToInt32(this.Number1) / Convert.ToInt32(this.Number2);
			} else {
				this.Number3 = this.Number1 / this.Number2;
			}

			return this.Number3;
		}

	}
}
