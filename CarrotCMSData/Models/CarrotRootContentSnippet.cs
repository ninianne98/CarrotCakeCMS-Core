namespace Carrotware.CMS.Data.Models {
	public partial class CarrotRootContentSnippet {
		public CarrotRootContentSnippet() {
			CarrotContentSnippets = new HashSet<CarrotContentSnippet>();
		}

		public Guid RootContentSnippetId { get; set; }
		public Guid SiteId { get; set; }
		public string ContentSnippetName { get; set; } = null!;
		public string ContentSnippetSlug { get; set; } = null!;
		public Guid CreateUserId { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime GoLiveDate { get; set; }
		public DateTime RetireDate { get; set; }
		public bool ContentSnippetActive { get; set; }
		public Guid? HeartbeatUserId { get; set; }
		public DateTime? EditHeartbeat { get; set; }

		public virtual CarrotSite Site { get; set; } = null!;
		public virtual ICollection<CarrotContentSnippet> CarrotContentSnippets { get; set; }
	}
}
