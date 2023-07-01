using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarrotCake.CMS.Plugins.PhotoGallery.Data.Migrations {
	/// <inheritdoc />
	public partial class InitialGallery : Migration {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.CreateTable(
				name: "tblGallery",
				columns: table => new {
					GalleryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					GalleryTitle = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
					SiteID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("tblGallery_PK", x => x.GalleryID);
				});

			migrationBuilder.CreateTable(
				name: "tblGalleryImageMeta",
				columns: table => new {
					GalleryImageMetaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					GalleryImageName = table.Column<string>(type: "varchar(512)", unicode: false, maxLength: 512, nullable: true),
					ImageTitle = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: true),
					ImageMetaData = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
					SiteID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("tblGalleryImageMeta_PK", x => x.GalleryImageMetaID);
				});

			migrationBuilder.CreateTable(
				name: "tblGalleryImage",
				columns: table => new {
					GalleryImageID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					GalleryImage = table.Column<string>(type: "varchar(512)", unicode: false, maxLength: 512, nullable: true),
					ImageOrder = table.Column<int>(type: "int", nullable: true),
					GalleryID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("tblGalleryImage_PK", x => x.GalleryImageID);
					table.ForeignKey(
						name: "tblGallery_tblGalleryImage_FK",
						column: x => x.GalleryID,
						principalTable: "tblGallery",
						principalColumn: "GalleryID");
				});

			migrationBuilder.CreateIndex(
				name: "IX_tblGalleryImage_GalleryID",
				table: "tblGalleryImage",
				column: "GalleryID");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropTable(
				name: "tblGalleryImage");

			migrationBuilder.DropTable(
				name: "tblGalleryImageMeta");

			migrationBuilder.DropTable(
				name: "tblGallery");
		}
	}
}
