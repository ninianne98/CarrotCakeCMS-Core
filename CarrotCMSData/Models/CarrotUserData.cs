namespace Carrotware.CMS.Data.Models {
	public partial class CarrotUserData {
		public CarrotUserData() {
			CarrotContentCreditUsers = new HashSet<CarrotContent>();
			CarrotContentEditUsers = new HashSet<CarrotContent>();
			CarrotRootContents = new HashSet<CarrotRootContent>();
			CarrotUserSiteMappings = new HashSet<CarrotUserSiteMapping>();
		}

		public Guid UserId { get; set; }
		public string? UserNickName { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? UserBio { get; set; }
		public string? UserKey { get; set; }

		public virtual AspNetUser? UserKeyNavigation { get; set; }
		public virtual ICollection<CarrotContent> CarrotContentCreditUsers { get; set; }
		public virtual ICollection<CarrotContent> CarrotContentEditUsers { get; set; }
		public virtual ICollection<CarrotRootContent> CarrotRootContents { get; set; }
		public virtual ICollection<CarrotUserSiteMapping> CarrotUserSiteMappings { get; set; }
	}
}
