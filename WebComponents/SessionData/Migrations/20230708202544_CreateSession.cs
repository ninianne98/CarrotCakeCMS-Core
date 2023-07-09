using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carrotware.Web.UI.Components.SessionData.Migrations {
	public partial class CreateSession : Migration {
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.CreateTable(
				name: "AspNetCache",
				columns: table => new {
					Id = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, collation: "SQL_Latin1_General_CP1_CS_AS"),
					Value = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
					ExpiresAtTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					SlidingExpirationInSeconds = table.Column<long>(type: "bigint", nullable: true),
					AbsoluteExpiration = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("PK_AspNetCache", x => x.Id);
				});

			migrationBuilder.CreateIndex(
				name: "Index_ExpiresAtTime",
				table: "AspNetCache",
				column: "ExpiresAtTime");
		}

		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropTable(
				name: "AspNetCache");
		}
	}
}
