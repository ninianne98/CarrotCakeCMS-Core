namespace Carrotware.CMS.Data.Models {
	public partial class CarrotRootContent {
		public CarrotRootContent() {
			CarrotCategoryContentMappings = new HashSet<CarrotCategoryContentMapping>();
			CarrotContentComments = new HashSet<CarrotContentComment>();
			CarrotContents = new HashSet<CarrotContent>();
			CarrotTagContentMappings = new HashSet<CarrotTagContentMapping>();
			CarrotWidgets = new HashSet<CarrotWidget>();
		}

		public Guid RootContentId { get; set; }
		public Guid SiteId { get; set; }
		public Guid? HeartbeatUserId { get; set; }
		public DateTime? EditHeartbeat { get; set; }
		public string FileName { get; set; } = null!;
		public bool PageActive { get; set; }
		public DateTime CreateDate { get; set; }
		public Guid ContentTypeId { get; set; }
		public string? PageSlug { get; set; }
		public string? PageThumbnail { get; set; }
		public DateTime GoLiveDate { get; set; }
		public DateTime RetireDate { get; set; }
		public DateTime GoLiveDateLocal { get; set; }
		public bool ShowInSiteNav { get; set; }
		public Guid CreateUserId { get; set; }
		public bool ShowInSiteMap { get; set; }
		public bool BlockIndex { get; set; }

		public virtual CarrotContentType ContentType { get; set; } = null!;
		public virtual CarrotUserData CreateUser { get; set; } = null!;
		public virtual CarrotSite Site { get; set; } = null!;
		public virtual ICollection<CarrotCategoryContentMapping> CarrotCategoryContentMappings { get; set; }
		public virtual ICollection<CarrotContentComment> CarrotContentComments { get; set; }
		public virtual ICollection<CarrotContent> CarrotContents { get; set; }
		public virtual ICollection<CarrotTagContentMapping> CarrotTagContentMappings { get; set; }
		public virtual ICollection<CarrotWidget> CarrotWidgets { get; set; }
	}
}
