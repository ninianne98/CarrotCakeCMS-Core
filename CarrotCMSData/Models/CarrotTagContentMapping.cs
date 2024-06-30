namespace Carrotware.CMS.Data.Models {
	public partial class CarrotTagContentMapping {
		public Guid TagContentMappingId { get; set; }
		public Guid ContentTagId { get; set; }
		public Guid RootContentId { get; set; }

		public virtual CarrotContentTag ContentTag { get; set; } = null!;
		public virtual CarrotRootContent RootContent { get; set; } = null!;
	}
}
