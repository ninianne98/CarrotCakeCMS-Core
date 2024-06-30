namespace Carrotware.CMS.Data.Models {
	public partial class CarrotContent {
		public Guid ContentId { get; set; }
		public Guid RootContentId { get; set; }
		public Guid? ParentContentId { get; set; }
		public bool IsLatestVersion { get; set; }
		public string? TitleBar { get; set; }
		public string? NavMenuText { get; set; }
		public string? PageHead { get; set; }
		public string? PageText { get; set; }
		public string? LeftPageText { get; set; }
		public string? RightPageText { get; set; }
		public int NavOrder { get; set; }
		public Guid? EditUserId { get; set; }
		public DateTime EditDate { get; set; }
		public string? TemplateFile { get; set; }
		public string? MetaKeyword { get; set; }
		public string? MetaDescription { get; set; }
		public Guid? CreditUserId { get; set; }

		public virtual CarrotUserData? CreditUser { get; set; }
		public virtual CarrotUserData? EditUser { get; set; }
		public virtual CarrotRootContent RootContent { get; set; } = null!;
	}
}
