using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2023, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: June 2023
*/

namespace Carrotware.CMS.Data.Models {
	/*
	dotnet ef dbcontext scaffold "Server=<<connection string>>" Microsoft.EntityFrameworkCore.SqlServer --context CarrotCakeContext -o CarrotCMSData/Models --table membership_UserRole
	dotnet ef dbcontext scaffold "Server=<<connection string>>" Microsoft.EntityFrameworkCore.SqlServer --context CarrotCakeContext -o CarrotCMSData/Models --table membership_UserRole --table membership_Role --table membership_User

	Scaffold-DbContext 'Server=<<connection string>>' Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
	dotnet ef dbcontext scaffold "Server=<<connection string>>" Microsoft.EntityFrameworkCore.SqlServer -p CarrotCMSData.csproj -o Models
	dotnet ef dbcontext scaffold "Server=<<connection string>>" Microsoft.EntityFrameworkCore.SqlServer --context CarrotCakeContext -o CarrotCMSData/Models
	dotnet ef dbcontext scaffold "Server=<<connection string>>" Microsoft.EntityFrameworkCore.SqlServer --context CarrotCakeContext -o Models

	dotnet ef migrations add Initial --context CarrotCakeContext --output-dir Migrations
	dotnet ef migrations add AddNewAuth --context CarrotCakeContext --output-dir Migrations

	Add-Migration Initial -Context CarrotCakeContext -OutputDir Migrations
	Add-Migration AddNewAuth -Context CarrotCakeContext -OutputDir Migrations
	*/

	public partial class CarrotCakeContext : DbContext {

		public static string DBKey { get { return "CarrotwareCMS"; } }

		public CarrotCakeContext() {
		}

		public CarrotCakeContext(DbContextOptions<CarrotCakeContext> options)
			: base(options) {
		}

		public static CarrotCakeContext Create() {
			var optionsBuilder = new DbContextOptionsBuilder<CarrotCakeContext>();

			DataHelper.Configure(DBKey, optionsBuilder);

			return new CarrotCakeContext(optionsBuilder.Options);
		}

		public static DataSet Exec(string query, List<SqlParameter> parms) {
			return DataHelper.ExecDataSet(DBKey, query, parms);
		}

		public static DataSet Exec(string query) {
			return DataHelper.ExecDataSet(DBKey, query, null);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			DataHelper.Configure(DBKey, optionsBuilder);
		}

		//=============================================
		public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

		public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

		public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

		public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

		public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

		public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }

		public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

		//=============================================
		public virtual DbSet<CarrotCategoryContentMapping> CarrotCategoryContentMappings { get; set; } = null!;
		public virtual DbSet<CarrotContent> CarrotContents { get; set; } = null!;
		public virtual DbSet<CarrotContentCategory> CarrotContentCategories { get; set; } = null!;
		public virtual DbSet<CarrotContentComment> CarrotContentComments { get; set; } = null!;
		public virtual DbSet<CarrotContentSnippet> CarrotContentSnippets { get; set; } = null!;
		public virtual DbSet<CarrotContentTag> CarrotContentTags { get; set; } = null!;
		public virtual DbSet<CarrotContentType> CarrotContentTypes { get; set; } = null!;
		public virtual DbSet<CarrotRootContent> CarrotRootContents { get; set; } = null!;
		public virtual DbSet<CarrotRootContentSnippet> CarrotRootContentSnippets { get; set; } = null!;
		public virtual DbSet<CarrotSerialCache> CarrotSerialCache { get; set; } = null!;
		public virtual DbSet<CarrotSite> CarrotSites { get; set; } = null!;
		public virtual DbSet<CarrotTagContentMapping> CarrotTagContentMappings { get; set; } = null!;
		public virtual DbSet<CarrotTextWidget> CarrotTextWidgets { get; set; } = null!;
		public virtual DbSet<CarrotUserData> CarrotUserData { get; set; } = null!;
		public virtual DbSet<CarrotUserSiteMapping> CarrotUserSiteMappings { get; set; } = null!;
		public virtual DbSet<CarrotWidget> CarrotWidgets { get; set; } = null!;
		public virtual DbSet<CarrotWidgetData> CarrotWidgetData { get; set; } = null!;
		public virtual DbSet<vwCarrotCategoryCounted> vwCarrotCategoryCounts { get; set; } = null!;
		public virtual DbSet<vwCarrotCategoryUrl> vwCarrotCategoryUrls { get; set; } = null!;
		public virtual DbSet<vwCarrotComment> vwCarrotComments { get; set; } = null!;
		public virtual DbSet<vwCarrotContent> vwCarrotContents { get; set; } = null!;
		public virtual DbSet<vwCarrotContentChild> vwCarrotContentChildren { get; set; } = null!;
		public virtual DbSet<vwCarrotContentSnippet> vwCarrotContentSnippets { get; set; } = null!;
		public virtual DbSet<vwCarrotEditHistory> vwCarrotEditHistories { get; set; } = null!;
		public virtual DbSet<vwCarrotEditorUrl> vwCarrotEditorUrls { get; set; } = null!;
		public virtual DbSet<vwCarrotTagCounted> vwCarrotTagCounts { get; set; } = null!;
		public virtual DbSet<vwCarrotTagUrl> vwCarrotTagUrls { get; set; } = null!;
		public virtual DbSet<vwCarrotUserData> vwCarrotUserData { get; set; } = null!;
		public virtual DbSet<vwCarrotWidget> vwCarrotWidgets { get; set; } = null!;

		// fake for sproc return, see SprocCarrotBlogMonthlyTallies
		public virtual DbSet<CarrotContentTally> CarrotContentTallies { get; set; } = null!;

		//=============================================

		public int SprocCarrotUpdateGoLiveLocal(Guid siteId, string xml) {
			xml = xml.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", string.Empty);
			xml = xml.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", string.Empty);

			object[] paramItems = new object[] {
						new SqlParameter("@site", siteId),
						new SqlParameter("@xml", xml),
					};

			return this.Database.ExecuteSqlRaw("exec dbo.carrot_UpdateGoLiveLocal @SiteID = @site, @xmlDocument = @xml", paramItems);
		}

		public IEnumerable<CarrotContentTally> SprocCarrotBlogMonthlyTallies(Guid siteId, bool activeOnly, int updates) {

			object[] paramItems = new object[] {
						new SqlParameter("@site", siteId),
						new SqlParameter("@act", activeOnly),
						new SqlParameter("@upd", updates),
			};

			return this.CarrotContentTallies.FromSqlRaw("exec dbo.carrot_BlogMonthlyTallies @SiteID = @site, @ActiveOnly = @act, @TakeTop = @upd", paramItems).AsEnumerable();
		}

		//=============================================

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<AspNetRole>(entity => {
				entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
					.IsUnique()
					.HasFilter("([NormalizedName] IS NOT NULL)");

				entity.Property(e => e.Id).HasMaxLength(128);
				entity.Property(e => e.Name).HasMaxLength(256);
				entity.Property(e => e.NormalizedName).HasMaxLength(256);
			});

			modelBuilder.Entity<AspNetRoleClaim>(entity => {
				entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

				entity.Property(e => e.RoleId).HasMaxLength(128);

				entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
			});

			modelBuilder.Entity<AspNetUser>(entity => {
				entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

				entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
					.IsUnique()
					.HasFilter("([NormalizedUserName] IS NOT NULL)");

				entity.Property(e => e.Id).HasMaxLength(128);
				entity.Property(e => e.Email).HasMaxLength(256);
				entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
				entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
				entity.Property(e => e.UserName).HasMaxLength(256);
			});

			modelBuilder.Entity<AspNetUserClaim>(entity => {
				entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

				entity.Property(e => e.UserId).HasMaxLength(128);

				entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
			});

			modelBuilder.Entity<AspNetUserLogin>(entity => {
				entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

				entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

				entity.Property(e => e.LoginProvider).HasMaxLength(128);
				entity.Property(e => e.ProviderKey).HasMaxLength(128);
				entity.Property(e => e.UserId).HasMaxLength(128);

				entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
			});

			modelBuilder.Entity<AspNetUserRole>(entity => {
				entity.HasKey(e => new { e.UserId, e.RoleId });

				entity.HasIndex(e => e.RoleId, "IX_AspNetUserRoles_RoleId");
				entity.HasIndex(e => e.UserId, "IX_AspNetUserRoles_UserId");

				entity.Property(e => e.RoleId).HasMaxLength(128);
				entity.Property(e => e.UserId).HasMaxLength(128);

				entity.HasOne(d => d.Role).WithMany(p => p.AspNetUserRoles).HasForeignKey(d => d.RoleId);
				entity.HasOne(d => d.User).WithMany(p => p.AspNetUserRoles).HasForeignKey(d => d.UserId);
			});

			modelBuilder.Entity<AspNetUserToken>(entity => {
				entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

				entity.Property(e => e.UserId).HasMaxLength(128);
				entity.Property(e => e.LoginProvider).HasMaxLength(128);
				entity.Property(e => e.Name).HasMaxLength(128);

				entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
			});

			//=============================================

			modelBuilder.Entity<CarrotCategoryContentMapping>(entity => {
				entity.HasKey(e => e.CategoryContentMappingId)
					.IsClustered(false);

				entity.ToTable("carrot_CategoryContentMapping");

				entity.Property(e => e.CategoryContentMappingId)
					.HasColumnName("CategoryContentMappingID")
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.ContentCategoryId).HasColumnName("ContentCategoryID");

				entity.Property(e => e.RootContentId).HasColumnName("Root_ContentID");

				entity.HasOne(d => d.ContentCategory)
					.WithMany(p => p.CarrotCategoryContentMappings)
					.HasForeignKey(d => d.ContentCategoryId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_carrot_CategoryContentMapping_ContentCategoryID");

				entity.HasOne(d => d.RootContent)
					.WithMany(p => p.CarrotCategoryContentMappings)
					.HasForeignKey(d => d.RootContentId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_carrot_CategoryContentMapping_Root_ContentID");
			});

			modelBuilder.Entity<CarrotContent>(entity => {
				entity.HasKey(e => e.ContentId);

				entity.ToTable("carrot_Content");

				entity.HasIndex(e => e.EditUserId, "IDX_carrot_Content_EditUserId");

				entity.HasIndex(e => e.RootContentId, "IDX_carrot_Content_Root_ContentID");

				entity.Property(e => e.ContentId)
					.HasColumnName("ContentID")
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.EditDate)
					.HasColumnType("datetime")
					.HasDefaultValueSql("(getdate())");

				entity.Property(e => e.MetaDescription).HasMaxLength(1024);

				entity.Property(e => e.MetaKeyword).HasMaxLength(1024);

				entity.Property(e => e.NavMenuText).HasMaxLength(256);

				entity.Property(e => e.PageHead).HasMaxLength(256);

				entity.Property(e => e.ParentContentId).HasColumnName("Parent_ContentID");

				entity.Property(e => e.RootContentId).HasColumnName("Root_ContentID");

				entity.Property(e => e.TemplateFile).HasMaxLength(256);

				entity.Property(e => e.TitleBar).HasMaxLength(256);

				entity.HasOne(d => d.CreditUser)
					.WithMany(p => p.CarrotContentCreditUsers)
					.HasForeignKey(d => d.CreditUserId)
					.HasConstraintName("carrot_Content_CreditUserId_FK");

				entity.HasOne(d => d.EditUser)
					.WithMany(p => p.CarrotContentEditUsers)
					.HasForeignKey(d => d.EditUserId)
					.HasConstraintName("carrot_Content_EditUserId_FK");

				entity.HasOne(d => d.RootContent)
					.WithMany(p => p.CarrotContents)
					.HasForeignKey(d => d.RootContentId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("carrot_RootContent_carrot_Content_FK");
			});

			modelBuilder.Entity<CarrotContentCategory>(entity => {
				entity.HasKey(e => e.ContentCategoryId)
					.IsClustered(false);

				entity.ToTable("carrot_ContentCategory");

				entity.Property(e => e.ContentCategoryId)
					.HasColumnName("ContentCategoryID")
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.CategorySlug).HasMaxLength(256);

				entity.Property(e => e.CategoryText).HasMaxLength(256);

				entity.Property(e => e.SiteId).HasColumnName("SiteID");

				entity.HasOne(d => d.Site)
					.WithMany(p => p.CarrotContentCategories)
					.HasForeignKey(d => d.SiteId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_carrot_ContentCategory_SiteID");
			});

			modelBuilder.Entity<CarrotContentComment>(entity => {
				entity.HasKey(e => e.ContentCommentId)
					.IsClustered(false);

				entity.ToTable("carrot_ContentComment");

				entity.Property(e => e.ContentCommentId)
					.HasColumnName("ContentCommentID")
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.CommenterEmail).HasMaxLength(256);

				entity.Property(e => e.CommenterIp)
					.HasMaxLength(32)
					.HasColumnName("CommenterIP");

				entity.Property(e => e.CommenterName).HasMaxLength(256);

				entity.Property(e => e.CommenterUrl)
					.HasMaxLength(256)
					.HasColumnName("CommenterURL");

				entity.Property(e => e.CreateDate)
					.HasColumnType("datetime")
					.HasDefaultValueSql("(getdate())");

				entity.Property(e => e.RootContentId).HasColumnName("Root_ContentID");

				entity.HasOne(d => d.RootContent)
					.WithMany(p => p.CarrotContentComments)
					.HasForeignKey(d => d.RootContentId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_carrot_ContentComment_Root_ContentID");
			});

			modelBuilder.Entity<CarrotContentSnippet>(entity => {
				entity.HasKey(e => e.ContentSnippetId);

				entity.ToTable("carrot_ContentSnippet");

				entity.Property(e => e.ContentSnippetId)
					.HasColumnName("ContentSnippetID")
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.EditDate).HasColumnType("datetime");

				entity.Property(e => e.RootContentSnippetId).HasColumnName("Root_ContentSnippetID");

				entity.HasOne(d => d.RootContentSnippet)
					.WithMany(p => p.CarrotContentSnippets)
					.HasForeignKey(d => d.RootContentSnippetId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_carrot_ContentSnippet_Root_ContentSnippetID");
			});

			modelBuilder.Entity<CarrotContentTag>(entity => {
				entity.HasKey(e => e.ContentTagId)
					.IsClustered(false);

				entity.ToTable("carrot_ContentTag");

				entity.Property(e => e.ContentTagId)
					.HasColumnName("ContentTagID")
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.SiteId).HasColumnName("SiteID");

				entity.Property(e => e.TagSlug).HasMaxLength(256);

				entity.Property(e => e.TagText).HasMaxLength(256);

				entity.HasOne(d => d.Site)
					.WithMany(p => p.CarrotContentTags)
					.HasForeignKey(d => d.SiteId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_carrot_ContentTag_SiteID");
			});

			modelBuilder.Entity<CarrotContentType>(entity => {
				entity.HasKey(e => e.ContentTypeId)
					.HasName("carrot_ContentType_PK");

				entity.ToTable("carrot_ContentType");

				entity.Property(e => e.ContentTypeId)
					.HasColumnName("ContentTypeID")
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.ContentTypeValue).HasMaxLength(256);
			});

			modelBuilder.Entity<CarrotRootContent>(entity => {
				entity.HasKey(e => e.RootContentId)
					.HasName("carrot_RootContent_PK");

				entity.ToTable("carrot_RootContent");

				entity.HasIndex(e => e.ContentTypeId, "IDX_carrot_RootContent_ContentTypeID");

				entity.HasIndex(e => e.CreateUserId, "IDX_carrot_RootContent_CreateUserId");

				entity.HasIndex(e => e.SiteId, "IDX_carrot_RootContent_SiteID");

				entity.Property(e => e.RootContentId)
					.HasColumnName("Root_ContentID")
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.ContentTypeId).HasColumnName("ContentTypeID");

				entity.Property(e => e.CreateDate)
					.HasColumnType("datetime")
					.HasDefaultValueSql("(getdate())");

				entity.Property(e => e.EditHeartbeat).HasColumnType("datetime");

				entity.Property(e => e.FileName).HasMaxLength(256);

				entity.Property(e => e.GoLiveDate)
					.HasColumnType("datetime")
					.HasDefaultValueSql("(getutcdate())");

				entity.Property(e => e.GoLiveDateLocal)
					.HasColumnType("datetime")
					.HasDefaultValueSql("(getutcdate())");

				entity.Property(e => e.HeartbeatUserId).HasColumnName("Heartbeat_UserId");

				entity.Property(e => e.PageSlug).HasMaxLength(256);

				entity.Property(e => e.PageThumbnail).HasMaxLength(128);

				entity.Property(e => e.RetireDate)
					.HasColumnType("datetime")
					.HasDefaultValueSql("(getutcdate())");

				entity.Property(e => e.SiteId).HasColumnName("SiteID");

				entity.HasOne(d => d.ContentType)
					.WithMany(p => p.CarrotRootContents)
					.HasForeignKey(d => d.ContentTypeId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("carrot_ContentType_carrot_RootContent_FK");

				entity.HasOne(d => d.CreateUser)
					.WithMany(p => p.CarrotRootContents)
					.HasForeignKey(d => d.CreateUserId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("carrot_RootContent_CreateUserId_FK");

				entity.HasOne(d => d.Site)
					.WithMany(p => p.CarrotRootContents)
					.HasForeignKey(d => d.SiteId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("carrot_Sites_carrot_RootContent_FK");
			});

			modelBuilder.Entity<CarrotRootContentSnippet>(entity => {
				entity.HasKey(e => e.RootContentSnippetId);

				entity.ToTable("carrot_RootContentSnippet");

				entity.Property(e => e.RootContentSnippetId)
					.HasColumnName("Root_ContentSnippetID")
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.ContentSnippetName).HasMaxLength(256);

				entity.Property(e => e.ContentSnippetSlug).HasMaxLength(128);

				entity.Property(e => e.CreateDate).HasColumnType("datetime");

				entity.Property(e => e.EditHeartbeat).HasColumnType("datetime");

				entity.Property(e => e.GoLiveDate).HasColumnType("datetime");

				entity.Property(e => e.HeartbeatUserId).HasColumnName("Heartbeat_UserId");

				entity.Property(e => e.RetireDate).HasColumnType("datetime");

				entity.Property(e => e.SiteId).HasColumnName("SiteID");

				entity.HasOne(d => d.Site)
					.WithMany(p => p.CarrotRootContentSnippets)
					.HasForeignKey(d => d.SiteId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_carrot_RootContentSnippet_SiteID");
			});

			modelBuilder.Entity<CarrotSerialCache>(entity => {
				entity.HasKey(e => e.SerialCacheId)
					.HasName("carrot_SerialCache_PK");

				entity.ToTable("carrot_SerialCache");

				entity.Property(e => e.SerialCacheId)
					.HasColumnName("SerialCacheID")
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.EditDate)
					.HasColumnType("datetime")
					.HasDefaultValueSql("(getdate())");

				entity.Property(e => e.ItemId).HasColumnName("ItemID");

				entity.Property(e => e.KeyType).HasMaxLength(256);

				entity.Property(e => e.SiteId).HasColumnName("SiteID");
			});

			modelBuilder.Entity<CarrotSite>(entity => {
				entity.HasKey(e => e.SiteId)
					.HasName("carrot_Sites_PK");

				entity.ToTable("carrot_Sites");

				entity.Property(e => e.SiteId)
					.HasColumnName("SiteID")
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.BlogCategoryPath)
					.HasMaxLength(64)
					.HasColumnName("Blog_CategoryPath");

				entity.Property(e => e.BlogDatePath)
					.HasMaxLength(64)
					.HasColumnName("Blog_DatePath");

				entity.Property(e => e.BlogDatePattern)
					.HasMaxLength(32)
					.HasColumnName("Blog_DatePattern");

				entity.Property(e => e.BlogEditorPath)
					.HasMaxLength(64)
					.HasColumnName("Blog_EditorPath");

				entity.Property(e => e.BlogFolderPath)
					.HasMaxLength(64)
					.HasColumnName("Blog_FolderPath");

				entity.Property(e => e.BlogRootContentId).HasColumnName("Blog_Root_ContentID");

				entity.Property(e => e.BlogTagPath)
					.HasMaxLength(64)
					.HasColumnName("Blog_TagPath");

				entity.Property(e => e.MainUrl)
					.HasMaxLength(128)
					.HasColumnName("MainURL");

				entity.Property(e => e.MetaDescription).HasMaxLength(1024);

				entity.Property(e => e.MetaKeyword).HasMaxLength(1024);

				entity.Property(e => e.SiteName).HasMaxLength(256);

				entity.Property(e => e.SiteTagline).HasMaxLength(1024);

				entity.Property(e => e.SiteTitlebarPattern).HasMaxLength(1024);

				entity.Property(e => e.TimeZone).HasMaxLength(128);
			});

			modelBuilder.Entity<CarrotTagContentMapping>(entity => {
				entity.HasKey(e => e.TagContentMappingId)
					.IsClustered(false);

				entity.ToTable("carrot_TagContentMapping");

				entity.Property(e => e.TagContentMappingId)
					.HasColumnName("TagContentMappingID")
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.ContentTagId).HasColumnName("ContentTagID");

				entity.Property(e => e.RootContentId).HasColumnName("Root_ContentID");

				entity.HasOne(d => d.ContentTag)
					.WithMany(p => p.CarrotTagContentMappings)
					.HasForeignKey(d => d.ContentTagId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_carrot_TagContentMapping_ContentTagID");

				entity.HasOne(d => d.RootContent)
					.WithMany(p => p.CarrotTagContentMappings)
					.HasForeignKey(d => d.RootContentId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_carrot_TagContentMapping_Root_ContentID");
			});

			modelBuilder.Entity<CarrotTextWidget>(entity => {
				entity.HasKey(e => e.TextWidgetId)
					.IsClustered(false);

				entity.ToTable("carrot_TextWidget");

				entity.Property(e => e.TextWidgetId)
					.HasColumnName("TextWidgetID")
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.ProcessHtmlText).HasColumnName("ProcessHTMLText");

				entity.Property(e => e.SiteId).HasColumnName("SiteID");

				entity.Property(e => e.TextWidgetAssembly).HasMaxLength(256);

				entity.HasOne(d => d.Site)
					.WithMany(p => p.CarrotTextWidgets)
					.HasForeignKey(d => d.SiteId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_carrot_TextWidget_SiteID");
			});

			modelBuilder.Entity<CarrotUserData>(entity => {
				entity.HasKey(e => e.UserId)
					.IsClustered(false);

				entity.ToTable("carrot_UserData");

				entity.Property(e => e.UserId).ValueGeneratedNever();

				entity.Property(e => e.FirstName).HasMaxLength(64);

				entity.Property(e => e.LastName).HasMaxLength(64);

				entity.Property(e => e.UserKey).HasMaxLength(128);

				entity.Property(e => e.UserNickName).HasMaxLength(64);

				entity.HasOne(d => d.UserKeyNavigation)
					.WithMany(p => p.CarrotUserData)
					.HasForeignKey(d => d.UserKey)
					.HasConstraintName("FK_carrot_UserData_AspNetUsers");
			});

			modelBuilder.Entity<CarrotUserSiteMapping>(entity => {
				entity.HasKey(e => e.UserSiteMappingId)
					.HasName("carrot_UserSiteMapping_PK");

				entity.ToTable("carrot_UserSiteMapping");

				entity.Property(e => e.UserSiteMappingId)
					.HasColumnName("UserSiteMappingID")
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.SiteId).HasColumnName("SiteID");

				entity.HasOne(d => d.Site)
					.WithMany(p => p.CarrotUserSiteMappings)
					.HasForeignKey(d => d.SiteId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("carrot_Sites_carrot_UserSiteMapping_FK");

				entity.HasOne(d => d.User)
					.WithMany(p => p.CarrotUserSiteMappings)
					.HasForeignKey(d => d.UserId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("aspnet_Users_carrot_UserSiteMapping_FK");
			});

			modelBuilder.Entity<CarrotWidget>(entity => {
				entity.HasKey(e => e.RootWidgetId);

				entity.ToTable("carrot_Widget");

				entity.Property(e => e.RootWidgetId)
					.HasColumnName("Root_WidgetID")
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.ControlPath).HasMaxLength(1024);

				entity.Property(e => e.GoLiveDate).HasColumnType("datetime");

				entity.Property(e => e.PlaceholderName).HasMaxLength(256);

				entity.Property(e => e.RetireDate).HasColumnType("datetime");

				entity.Property(e => e.RootContentId).HasColumnName("Root_ContentID");

				entity.HasOne(d => d.RootContent)
					.WithMany(p => p.CarrotWidgets)
					.HasForeignKey(d => d.RootContentId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("carrot_RootContent_carrot_Widget_FK");
			});

			modelBuilder.Entity<CarrotWidgetData>(entity => {
				entity.HasKey(e => e.WidgetDataId);

				entity.ToTable("carrot_WidgetData");

				entity.Property(e => e.WidgetDataId)
					.HasColumnName("WidgetDataID")
					.HasDefaultValueSql("(newid())");

				entity.Property(e => e.EditDate)
					.HasColumnType("datetime")
					.HasDefaultValueSql("(getdate())");

				entity.Property(e => e.RootWidgetId).HasColumnName("Root_WidgetID");

				entity.HasOne(d => d.RootWidget)
					.WithMany(p => p.CarrotWidgetData)
					.HasForeignKey(d => d.RootWidgetId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("carrot_WidgetData_Root_WidgetID_FK");
			});

			modelBuilder.Entity<vwCarrotCategoryCounted>(entity => {
				entity.ToView("vw_carrot_CategoryCounted");

				entity.Property(e => e.CategorySlug).HasMaxLength(256);

				entity.Property(e => e.CategoryText).HasMaxLength(256);

				entity.Property(e => e.ContentCategoryId).HasColumnName("ContentCategoryID");

				entity.Property(e => e.SiteId).HasColumnName("SiteID");
			});

			modelBuilder.Entity<vwCarrotCategoryUrl>(entity => {
				entity.ToView("vw_carrot_CategoryURL");

				entity.Property(e => e.CategoryText).HasMaxLength(256);

				entity.Property(e => e.CategoryUrl).HasMaxLength(387);

				entity.Property(e => e.ContentCategoryId).HasColumnName("ContentCategoryID");

				entity.Property(e => e.EditDate).HasColumnType("datetime");

				entity.Property(e => e.SiteId).HasColumnName("SiteID");
			});

			modelBuilder.Entity<vwCarrotComment>(entity => {
				entity.HasKey(e => e.ContentCommentId);

				entity.ToView("vw_carrot_Comment");

				entity.Property(e => e.CommenterEmail).HasMaxLength(256);

				entity.Property(e => e.CommenterIp)
					.HasMaxLength(32)
					.HasColumnName("CommenterIP");

				entity.Property(e => e.CommenterName).HasMaxLength(256);

				entity.Property(e => e.CommenterUrl)
					.HasMaxLength(256)
					.HasColumnName("CommenterURL");

				entity.Property(e => e.ContentCommentId).HasColumnName("ContentCommentID");

				entity.Property(e => e.ContentTypeId).HasColumnName("ContentTypeID");

				entity.Property(e => e.ContentTypeValue).HasMaxLength(256);

				entity.Property(e => e.CreateDate).HasColumnType("datetime");

				entity.Property(e => e.FileName).HasMaxLength(256);

				entity.Property(e => e.GoLiveDate).HasColumnType("datetime");

				entity.Property(e => e.NavMenuText).HasMaxLength(256);

				entity.Property(e => e.PageHead).HasMaxLength(256);

				entity.Property(e => e.RetireDate).HasColumnType("datetime");

				entity.Property(e => e.RootContentId).HasColumnName("Root_ContentID");

				entity.Property(e => e.SiteId).HasColumnName("SiteID");

				entity.Property(e => e.TitleBar).HasMaxLength(256);
			});

			modelBuilder.Entity<vwCarrotContent>(entity => {
				entity.HasKey(e => e.ContentId);

				entity.ToView("vw_carrot_Content");

				entity.Property(e => e.ContentId).HasColumnName("ContentID");

				entity.Property(e => e.ContentTypeId).HasColumnName("ContentTypeID");

				entity.Property(e => e.ContentTypeValue).HasMaxLength(256);

				entity.Property(e => e.CreateDate).HasColumnType("datetime");

				entity.Property(e => e.EditDate).HasColumnType("datetime");

				entity.Property(e => e.EditHeartbeat).HasColumnType("datetime");

				entity.Property(e => e.FileName).HasMaxLength(256);

				entity.Property(e => e.GoLiveDate).HasColumnType("datetime");

				entity.Property(e => e.GoLiveDateLocal).HasColumnType("datetime");

				entity.Property(e => e.HeartbeatUserId).HasColumnName("Heartbeat_UserId");

				entity.Property(e => e.MetaDescription).HasMaxLength(1024);

				entity.Property(e => e.MetaKeyword).HasMaxLength(1024);

				entity.Property(e => e.NavMenuText).HasMaxLength(256);

				entity.Property(e => e.PageHead).HasMaxLength(256);

				entity.Property(e => e.PageSlug).HasMaxLength(256);

				entity.Property(e => e.PageThumbnail).HasMaxLength(128);

				entity.Property(e => e.ParentContentId).HasColumnName("Parent_ContentID");

				entity.Property(e => e.RetireDate).HasColumnType("datetime");

				entity.Property(e => e.RootContentId).HasColumnName("Root_ContentID");

				entity.Property(e => e.SiteId).HasColumnName("SiteID");

				entity.Property(e => e.TemplateFile).HasMaxLength(256);

				entity.Property(e => e.TimeZone).HasMaxLength(128);

				entity.Property(e => e.TitleBar).HasMaxLength(256);
			});

			modelBuilder.Entity<vwCarrotContentChild>(entity => {
				entity.HasKey(e => e.RootContentId);

				entity.ToView("vw_carrot_ContentChild");

				entity.Property(e => e.FileName).HasMaxLength(256);

				entity.Property(e => e.GoLiveDate).HasColumnType("datetime");

				entity.Property(e => e.ParentContentId).HasColumnName("Parent_ContentID");

				entity.Property(e => e.ParentFileName).HasMaxLength(256);

				entity.Property(e => e.ParentGoLiveDate).HasColumnType("datetime");

				entity.Property(e => e.ParentRetireDate).HasColumnType("datetime");

				entity.Property(e => e.RetireDate).HasColumnType("datetime");

				entity.Property(e => e.RootContentId).HasColumnName("Root_ContentID");

				entity.Property(e => e.SiteId).HasColumnName("SiteID");
			});

			modelBuilder.Entity<vwCarrotContentSnippet>(entity => {
				entity.HasKey(e => e.ContentSnippetId);

				entity.ToView("vw_carrot_ContentSnippet");

				entity.Property(e => e.ContentSnippetId).HasColumnName("ContentSnippetID");

				entity.Property(e => e.ContentSnippetName).HasMaxLength(256);

				entity.Property(e => e.ContentSnippetSlug).HasMaxLength(128);

				entity.Property(e => e.CreateDate).HasColumnType("datetime");

				entity.Property(e => e.EditDate).HasColumnType("datetime");

				entity.Property(e => e.EditHeartbeat).HasColumnType("datetime");

				entity.Property(e => e.GoLiveDate).HasColumnType("datetime");

				entity.Property(e => e.HeartbeatUserId).HasColumnName("Heartbeat_UserId");

				entity.Property(e => e.RetireDate).HasColumnType("datetime");

				entity.Property(e => e.RootContentSnippetId).HasColumnName("Root_ContentSnippetID");

				entity.Property(e => e.SiteId).HasColumnName("SiteID");
			});

			modelBuilder.Entity<vwCarrotEditHistory>(entity => {
				entity.HasKey(e => e.ContentId);

				entity.ToView("vw_carrot_EditHistory");

				entity.Property(e => e.ContentId).HasColumnName("ContentID");

				entity.Property(e => e.ContentTypeId).HasColumnName("ContentTypeID");

				entity.Property(e => e.ContentTypeValue).HasMaxLength(256);

				entity.Property(e => e.CreateDate).HasColumnType("datetime");

				entity.Property(e => e.CreateEmail).HasMaxLength(256);

				entity.Property(e => e.CreateUserName).HasMaxLength(256);

				entity.Property(e => e.EditDate).HasColumnType("datetime");

				entity.Property(e => e.EditEmail).HasMaxLength(256);

				entity.Property(e => e.EditUserName).HasMaxLength(256);

				entity.Property(e => e.FileName).HasMaxLength(256);

				entity.Property(e => e.GoLiveDate).HasColumnType("datetime");

				entity.Property(e => e.NavMenuText).HasMaxLength(256);

				entity.Property(e => e.PageHead).HasMaxLength(256);

				entity.Property(e => e.RetireDate).HasColumnType("datetime");

				entity.Property(e => e.RootContentId).HasColumnName("Root_ContentID");

				entity.Property(e => e.SiteId).HasColumnName("SiteID");

				entity.Property(e => e.TitleBar).HasMaxLength(256);
			});

			modelBuilder.Entity<vwCarrotEditorUrl>(entity => {
				entity.HasKey(e => e.UserId);

				entity.ToView("vw_carrot_EditorURL");

				entity.Property(e => e.EditDate).HasColumnType("datetime");

				entity.Property(e => e.LoweredEmail).HasMaxLength(256);

				entity.Property(e => e.SiteId).HasColumnName("SiteID");

				entity.Property(e => e.UserName).HasMaxLength(256);

				entity.Property(e => e.UserUrl).HasMaxLength(387);
			});

			modelBuilder.Entity<vwCarrotTagCounted>(entity => {
				entity.HasKey(e => e.ContentTagId);

				entity.ToView("vw_carrot_TagCounted");

				entity.Property(e => e.ContentTagId).HasColumnName("ContentTagID");

				entity.Property(e => e.SiteId).HasColumnName("SiteID");

				entity.Property(e => e.TagSlug).HasMaxLength(256);

				entity.Property(e => e.TagText).HasMaxLength(256);
			});

			modelBuilder.Entity<vwCarrotTagUrl>(entity => {
				entity.HasKey(e => e.ContentTagId);

				entity.ToView("vw_carrot_TagURL");

				entity.Property(e => e.ContentTagId).HasColumnName("ContentTagID");

				entity.Property(e => e.EditDate).HasColumnType("datetime");

				entity.Property(e => e.SiteId).HasColumnName("SiteID");

				entity.Property(e => e.TagText).HasMaxLength(256);

				entity.Property(e => e.TagUrl).HasMaxLength(387);
			});

			modelBuilder.Entity<vwCarrotUserData>(entity => {
				entity.HasKey(e => e.UserId);

				entity.ToView("vw_carrot_UserData");

				entity.Property(e => e.Email).HasMaxLength(256);

				entity.Property(e => e.FirstName).HasMaxLength(64);

				entity.Property(e => e.Id).HasMaxLength(128);

				entity.Property(e => e.LastName).HasMaxLength(64);

				entity.Property(e => e.LockoutEndDateUtc).HasColumnType("datetime");

				entity.Property(e => e.UserKey).HasMaxLength(128);

				entity.Property(e => e.UserName).HasMaxLength(256);

				entity.Property(e => e.UserNickName).HasMaxLength(64);
			});

			modelBuilder.Entity<vwCarrotWidget>(entity => {
				entity.HasKey(e => e.RootWidgetId);

				entity.ToView("vw_carrot_Widget");

				entity.Property(e => e.ControlPath).HasMaxLength(1024);

				entity.Property(e => e.EditDate).HasColumnType("datetime");

				entity.Property(e => e.GoLiveDate).HasColumnType("datetime");

				entity.Property(e => e.PlaceholderName).HasMaxLength(256);

				entity.Property(e => e.RetireDate).HasColumnType("datetime");

				entity.Property(e => e.RootContentId).HasColumnName("Root_ContentID");

				entity.Property(e => e.RootWidgetId).HasColumnName("Root_WidgetID");

				entity.Property(e => e.SiteId).HasColumnName("SiteID");

				entity.Property(e => e.WidgetDataId).HasColumnName("WidgetDataID");
			});

			OnModelCreatingPartial(modelBuilder);
		}

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
	}
}