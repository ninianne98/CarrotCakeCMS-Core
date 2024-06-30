namespace Carrotware.CMS.Data.Models {
	public partial class CarrotSerialCache {
		public Guid SerialCacheId { get; set; }
		public Guid SiteId { get; set; }
		public Guid ItemId { get; set; }
		public Guid EditUserId { get; set; }
		public string? KeyType { get; set; }
		public string? SerializedData { get; set; }
		public DateTime EditDate { get; set; }
	}
}
