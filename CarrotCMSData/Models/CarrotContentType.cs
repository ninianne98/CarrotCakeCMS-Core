namespace Carrotware.CMS.Data.Models {
	public partial class CarrotContentType {
		public CarrotContentType() {
			CarrotRootContents = new HashSet<CarrotRootContent>();
		}

		public Guid ContentTypeId { get; set; }
		public string ContentTypeValue { get; set; } = null!;

		public virtual ICollection<CarrotRootContent> CarrotRootContents { get; set; }
	}
}
