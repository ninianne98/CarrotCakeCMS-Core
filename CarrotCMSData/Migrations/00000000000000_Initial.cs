using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carrotware.CMS.Data.Migrations {
	public partial class Initial : Migration {
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.CreateTable(
				name: "carrot_ContentType",
				columns: table => new {
					ContentTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					ContentTypeValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("carrot_ContentType_PK", x => x.ContentTypeID);
				});

			migrationBuilder.CreateTable(
				name: "carrot_DataInfo",
				columns: table => new {
					DataInfoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					DataKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
					DataValue = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_carrot_DataInfo", x => x.DataInfoID)
						.Annotation("SqlServer:Clustered", false);
				});

			migrationBuilder.CreateTable(
				name: "carrot_SerialCache",
				columns: table => new {
					SerialCacheID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					SiteID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					ItemID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					EditUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					KeyType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					SerializedData = table.Column<string>(type: "nvarchar(max)", nullable: true),
					EditDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
				},
				constraints: table => {
					table.PrimaryKey("carrot_SerialCache_PK", x => x.SerialCacheID);
				});

			migrationBuilder.CreateTable(
				name: "carrot_Sites",
				columns: table => new {
					SiteID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					MetaKeyword = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
					MetaDescription = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
					SiteName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					MainURL = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
					BlockIndex = table.Column<bool>(type: "bit", nullable: false),
					SiteTagline = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
					SiteTitlebarPattern = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
					Blog_Root_ContentID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
					Blog_FolderPath = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
					Blog_CategoryPath = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
					Blog_TagPath = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
					Blog_DatePath = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
					Blog_DatePattern = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
					TimeZone = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
					SendTrackbacks = table.Column<bool>(type: "bit", nullable: false),
					AcceptTrackbacks = table.Column<bool>(type: "bit", nullable: false),
					Blog_EditorPath = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("carrot_Sites_PK", x => x.SiteID);
				});

			migrationBuilder.CreateTable(
				name: "membership_Role",
				columns: table => new {
					Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
					Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_membership_Role", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "membership_User",
				columns: table => new {
					Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
					Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
					PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
					SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
					PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
					PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
					TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
					LockoutEndDateUtc = table.Column<DateTime>(type: "datetime", nullable: true),
					LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
					AccessFailedCount = table.Column<int>(type: "int", nullable: false),
					UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_membership_User", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "carrot_ContentCategory",
				columns: table => new {
					ContentCategoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					SiteID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					CategoryText = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
					CategorySlug = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
					IsPublic = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_carrot_ContentCategory", x => x.ContentCategoryID)
						.Annotation("SqlServer:Clustered", false);
					table.ForeignKey(
						name: "FK_carrot_ContentCategory_SiteID",
						column: x => x.SiteID,
						principalTable: "carrot_Sites",
						principalColumn: "SiteID");
				});

			migrationBuilder.CreateTable(
				name: "carrot_ContentTag",
				columns: table => new {
					ContentTagID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					SiteID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					TagText = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
					TagSlug = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
					IsPublic = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_carrot_ContentTag", x => x.ContentTagID)
						.Annotation("SqlServer:Clustered", false);
					table.ForeignKey(
						name: "FK_carrot_ContentTag_SiteID",
						column: x => x.SiteID,
						principalTable: "carrot_Sites",
						principalColumn: "SiteID");
				});

			migrationBuilder.CreateTable(
				name: "carrot_RootContentSnippet",
				columns: table => new {
					Root_ContentSnippetID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					SiteID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					ContentSnippetName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
					ContentSnippetSlug = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
					CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
					GoLiveDate = table.Column<DateTime>(type: "datetime", nullable: false),
					RetireDate = table.Column<DateTime>(type: "datetime", nullable: false),
					ContentSnippetActive = table.Column<bool>(type: "bit", nullable: false),
					Heartbeat_UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
					EditHeartbeat = table.Column<DateTime>(type: "datetime", nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_carrot_RootContentSnippet", x => x.Root_ContentSnippetID);
					table.ForeignKey(
						name: "FK_carrot_RootContentSnippet_SiteID",
						column: x => x.SiteID,
						principalTable: "carrot_Sites",
						principalColumn: "SiteID");
				});

			migrationBuilder.CreateTable(
				name: "carrot_TextWidget",
				columns: table => new {
					TextWidgetID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					SiteID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					TextWidgetAssembly = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
					ProcessBody = table.Column<bool>(type: "bit", nullable: false),
					ProcessPlainText = table.Column<bool>(type: "bit", nullable: false),
					ProcessHTMLText = table.Column<bool>(type: "bit", nullable: false),
					ProcessComment = table.Column<bool>(type: "bit", nullable: false),
					ProcessSnippet = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_carrot_TextWidget", x => x.TextWidgetID)
						.Annotation("SqlServer:Clustered", false);
					table.ForeignKey(
						name: "FK_carrot_TextWidget_SiteID",
						column: x => x.SiteID,
						principalTable: "carrot_Sites",
						principalColumn: "SiteID");
				});

			migrationBuilder.CreateTable(
				name: "carrot_UserData",
				columns: table => new {
					UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					UserNickName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
					FirstName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
					LastName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
					UserBio = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UserKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_carrot_UserData", x => x.UserId)
						.Annotation("SqlServer:Clustered", false);
					table.ForeignKey(
						name: "carrot_UserData_UserKey",
						column: x => x.UserKey,
						principalTable: "membership_User",
						principalColumn: "Id");
				});

			migrationBuilder.CreateTable(
				name: "membership_UserRole",
				columns: table => new {
					UserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
					RoleId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_dbo.membership_UserRole", x => new { x.UserId, x.RoleId });
					table.ForeignKey(
						name: "FK_dbo.membership_UserRole_dbo.membership_Role_RoleId",
						column: x => x.RoleId,
						principalTable: "membership_Role",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_dbo.membership_UserRole_dbo.membership_User_UserId",
						column: x => x.UserId,
						principalTable: "membership_User",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "carrot_ContentSnippet",
				columns: table => new {
					ContentSnippetID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					Root_ContentSnippetID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					IsLatestVersion = table.Column<bool>(type: "bit", nullable: false),
					EditUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					EditDate = table.Column<DateTime>(type: "datetime", nullable: false),
					ContentBody = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_carrot_ContentSnippet", x => x.ContentSnippetID);
					table.ForeignKey(
						name: "FK_carrot_ContentSnippet_Root_ContentSnippetID",
						column: x => x.Root_ContentSnippetID,
						principalTable: "carrot_RootContentSnippet",
						principalColumn: "Root_ContentSnippetID");
				});

			migrationBuilder.CreateTable(
				name: "carrot_RootContent",
				columns: table => new {
					Root_ContentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					SiteID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Heartbeat_UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
					EditHeartbeat = table.Column<DateTime>(type: "datetime", nullable: true),
					FileName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
					PageActive = table.Column<bool>(type: "bit", nullable: false),
					CreateDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
					ContentTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					PageSlug = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					PageThumbnail = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
					GoLiveDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())"),
					RetireDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())"),
					GoLiveDateLocal = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())"),
					ShowInSiteNav = table.Column<bool>(type: "bit", nullable: false),
					CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					ShowInSiteMap = table.Column<bool>(type: "bit", nullable: false),
					BlockIndex = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("carrot_RootContent_PK", x => x.Root_ContentID);
					table.ForeignKey(
						name: "carrot_ContentType_carrot_RootContent_FK",
						column: x => x.ContentTypeID,
						principalTable: "carrot_ContentType",
						principalColumn: "ContentTypeID");
					table.ForeignKey(
						name: "carrot_RootContent_CreateUserId_FK",
						column: x => x.CreateUserId,
						principalTable: "carrot_UserData",
						principalColumn: "UserId");
					table.ForeignKey(
						name: "carrot_Sites_carrot_RootContent_FK",
						column: x => x.SiteID,
						principalTable: "carrot_Sites",
						principalColumn: "SiteID");
				});

			migrationBuilder.CreateTable(
				name: "carrot_UserSiteMapping",
				columns: table => new {
					UserSiteMappingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					SiteID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("carrot_UserSiteMapping_PK", x => x.UserSiteMappingID);
					table.ForeignKey(
						name: "aspnet_Users_carrot_UserSiteMapping_FK",
						column: x => x.UserId,
						principalTable: "carrot_UserData",
						principalColumn: "UserId");
					table.ForeignKey(
						name: "carrot_Sites_carrot_UserSiteMapping_FK",
						column: x => x.SiteID,
						principalTable: "carrot_Sites",
						principalColumn: "SiteID");
				});

			migrationBuilder.CreateTable(
				name: "carrot_CategoryContentMapping",
				columns: table => new {
					CategoryContentMappingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					ContentCategoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Root_ContentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_carrot_CategoryContentMapping", x => x.CategoryContentMappingID)
						.Annotation("SqlServer:Clustered", false);
					table.ForeignKey(
						name: "FK_carrot_CategoryContentMapping_ContentCategoryID",
						column: x => x.ContentCategoryID,
						principalTable: "carrot_ContentCategory",
						principalColumn: "ContentCategoryID");
					table.ForeignKey(
						name: "FK_carrot_CategoryContentMapping_Root_ContentID",
						column: x => x.Root_ContentID,
						principalTable: "carrot_RootContent",
						principalColumn: "Root_ContentID");
				});

			migrationBuilder.CreateTable(
				name: "carrot_Content",
				columns: table => new {
					ContentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					Root_ContentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Parent_ContentID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
					IsLatestVersion = table.Column<bool>(type: "bit", nullable: false),
					TitleBar = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					NavMenuText = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					PageHead = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					PageText = table.Column<string>(type: "nvarchar(max)", nullable: true),
					LeftPageText = table.Column<string>(type: "nvarchar(max)", nullable: true),
					RightPageText = table.Column<string>(type: "nvarchar(max)", nullable: true),
					NavOrder = table.Column<int>(type: "int", nullable: false),
					EditUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
					EditDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
					TemplateFile = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					MetaKeyword = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
					MetaDescription = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
					CreditUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_carrot_Content", x => x.ContentID);
					table.ForeignKey(
						name: "carrot_Content_CreditUserId_FK",
						column: x => x.CreditUserId,
						principalTable: "carrot_UserData",
						principalColumn: "UserId");
					table.ForeignKey(
						name: "carrot_Content_EditUserId_FK",
						column: x => x.EditUserId,
						principalTable: "carrot_UserData",
						principalColumn: "UserId");
					table.ForeignKey(
						name: "carrot_RootContent_carrot_Content_FK",
						column: x => x.Root_ContentID,
						principalTable: "carrot_RootContent",
						principalColumn: "Root_ContentID");
				});

			migrationBuilder.CreateTable(
				name: "carrot_ContentComment",
				columns: table => new {
					ContentCommentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					Root_ContentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					CreateDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
					CommenterIP = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
					CommenterName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
					CommenterEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
					CommenterURL = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
					PostComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
					IsApproved = table.Column<bool>(type: "bit", nullable: false),
					IsSpam = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_carrot_ContentComment", x => x.ContentCommentID)
						.Annotation("SqlServer:Clustered", false);
					table.ForeignKey(
						name: "FK_carrot_ContentComment_Root_ContentID",
						column: x => x.Root_ContentID,
						principalTable: "carrot_RootContent",
						principalColumn: "Root_ContentID");
				});

			migrationBuilder.CreateTable(
				name: "carrot_TagContentMapping",
				columns: table => new {
					TagContentMappingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					ContentTagID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Root_ContentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_carrot_TagContentMapping", x => x.TagContentMappingID)
						.Annotation("SqlServer:Clustered", false);
					table.ForeignKey(
						name: "FK_carrot_TagContentMapping_ContentTagID",
						column: x => x.ContentTagID,
						principalTable: "carrot_ContentTag",
						principalColumn: "ContentTagID");
					table.ForeignKey(
						name: "FK_carrot_TagContentMapping_Root_ContentID",
						column: x => x.Root_ContentID,
						principalTable: "carrot_RootContent",
						principalColumn: "Root_ContentID");
				});

			migrationBuilder.CreateTable(
				name: "carrot_Widget",
				columns: table => new {
					Root_WidgetID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					Root_ContentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					WidgetOrder = table.Column<int>(type: "int", nullable: false),
					PlaceholderName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
					ControlPath = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
					WidgetActive = table.Column<bool>(type: "bit", nullable: false),
					GoLiveDate = table.Column<DateTime>(type: "datetime", nullable: false),
					RetireDate = table.Column<DateTime>(type: "datetime", nullable: false)
				},
				constraints: table => {
					table.PrimaryKey("PK_carrot_Widget", x => x.Root_WidgetID);
					table.ForeignKey(
						name: "carrot_RootContent_carrot_Widget_FK",
						column: x => x.Root_ContentID,
						principalTable: "carrot_RootContent",
						principalColumn: "Root_ContentID");
				});

			migrationBuilder.CreateTable(
				name: "carrot_WidgetData",
				columns: table => new {
					WidgetDataID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					Root_WidgetID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					IsLatestVersion = table.Column<bool>(type: "bit", nullable: false),
					EditDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
					ControlProperties = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_carrot_WidgetData", x => x.WidgetDataID);
					table.ForeignKey(
						name: "carrot_WidgetData_Root_WidgetID_FK",
						column: x => x.Root_WidgetID,
						principalTable: "carrot_Widget",
						principalColumn: "Root_WidgetID");
				});

			migrationBuilder.CreateIndex(
				name: "IX_carrot_CategoryContentMapping_ContentCategoryID",
				table: "carrot_CategoryContentMapping",
				column: "ContentCategoryID");

			migrationBuilder.CreateIndex(
				name: "IX_carrot_CategoryContentMapping_Root_ContentID",
				table: "carrot_CategoryContentMapping",
				column: "Root_ContentID");

			migrationBuilder.CreateIndex(
				name: "IDX_carrot_Content_EditUserId",
				table: "carrot_Content",
				column: "EditUserId");

			migrationBuilder.CreateIndex(
				name: "IDX_carrot_Content_Root_ContentID",
				table: "carrot_Content",
				column: "Root_ContentID");

			migrationBuilder.CreateIndex(
				name: "IX_carrot_Content_CreditUserId",
				table: "carrot_Content",
				column: "CreditUserId");

			migrationBuilder.CreateIndex(
				name: "IX_carrot_ContentCategory_SiteID",
				table: "carrot_ContentCategory",
				column: "SiteID");

			migrationBuilder.CreateIndex(
				name: "IX_carrot_ContentComment_Root_ContentID",
				table: "carrot_ContentComment",
				column: "Root_ContentID");

			migrationBuilder.CreateIndex(
				name: "IX_carrot_ContentSnippet_Root_ContentSnippetID",
				table: "carrot_ContentSnippet",
				column: "Root_ContentSnippetID");

			migrationBuilder.CreateIndex(
				name: "IX_carrot_ContentTag_SiteID",
				table: "carrot_ContentTag",
				column: "SiteID");

			migrationBuilder.CreateIndex(
				name: "IDX_carrot_RootContent_ContentTypeID",
				table: "carrot_RootContent",
				column: "ContentTypeID");

			migrationBuilder.CreateIndex(
				name: "IDX_carrot_RootContent_CreateUserId",
				table: "carrot_RootContent",
				column: "CreateUserId");

			migrationBuilder.CreateIndex(
				name: "IDX_carrot_RootContent_SiteID",
				table: "carrot_RootContent",
				column: "SiteID");

			migrationBuilder.CreateIndex(
				name: "IX_carrot_RootContentSnippet_SiteID",
				table: "carrot_RootContentSnippet",
				column: "SiteID");

			migrationBuilder.CreateIndex(
				name: "IX_carrot_TagContentMapping_ContentTagID",
				table: "carrot_TagContentMapping",
				column: "ContentTagID");

			migrationBuilder.CreateIndex(
				name: "IX_carrot_TagContentMapping_Root_ContentID",
				table: "carrot_TagContentMapping",
				column: "Root_ContentID");

			migrationBuilder.CreateIndex(
				name: "IX_carrot_TextWidget_SiteID",
				table: "carrot_TextWidget",
				column: "SiteID");

			migrationBuilder.CreateIndex(
				name: "IX_carrot_UserData_UserKey",
				table: "carrot_UserData",
				column: "UserKey");

			migrationBuilder.CreateIndex(
				name: "IX_carrot_UserSiteMapping_SiteID",
				table: "carrot_UserSiteMapping",
				column: "SiteID");

			migrationBuilder.CreateIndex(
				name: "IX_carrot_UserSiteMapping_UserId",
				table: "carrot_UserSiteMapping",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_carrot_Widget_Root_ContentID",
				table: "carrot_Widget",
				column: "Root_ContentID");

			migrationBuilder.CreateIndex(
				name: "IX_carrot_WidgetData_Root_WidgetID",
				table: "carrot_WidgetData",
				column: "Root_WidgetID");

			migrationBuilder.CreateIndex(
				name: "RoleNameIndex",
				table: "membership_Role",
				column: "Name",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "UserNameIndex",
				table: "membership_User",
				column: "UserName",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_RoleId",
				table: "membership_UserRole",
				column: "RoleId");

			migrationBuilder.CreateIndex(
				name: "IX_UserId",
				table: "membership_UserRole",
				column: "UserId");

			string sql = DataHelper.GetScript("00_Initial.sql");

			migrationBuilder.Sql(sql);
		}

		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropTable(
				name: "carrot_CategoryContentMapping");

			migrationBuilder.DropTable(
				name: "carrot_Content");

			migrationBuilder.DropTable(
				name: "carrot_ContentComment");

			migrationBuilder.DropTable(
				name: "carrot_ContentSnippet");

			migrationBuilder.DropTable(
				name: "carrot_DataInfo");

			migrationBuilder.DropTable(
				name: "carrot_SerialCache");

			migrationBuilder.DropTable(
				name: "carrot_TagContentMapping");

			migrationBuilder.DropTable(
				name: "carrot_TextWidget");

			migrationBuilder.DropTable(
				name: "carrot_UserSiteMapping");

			migrationBuilder.DropTable(
				name: "carrot_WidgetData");

			migrationBuilder.DropTable(
				name: "membership_UserRole");

			migrationBuilder.DropTable(
				name: "carrot_ContentCategory");

			migrationBuilder.DropTable(
				name: "carrot_RootContentSnippet");

			migrationBuilder.DropTable(
				name: "carrot_ContentTag");

			migrationBuilder.DropTable(
				name: "carrot_Widget");

			migrationBuilder.DropTable(
				name: "membership_Role");

			migrationBuilder.DropTable(
				name: "carrot_RootContent");

			migrationBuilder.DropTable(
				name: "carrot_ContentType");

			migrationBuilder.DropTable(
				name: "carrot_UserData");

			migrationBuilder.DropTable(
				name: "carrot_Sites");

			migrationBuilder.DropTable(
				name: "membership_User");
		}
	}
}
