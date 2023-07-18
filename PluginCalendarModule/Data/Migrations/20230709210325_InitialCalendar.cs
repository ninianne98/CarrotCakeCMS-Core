using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarrotCake.CMS.Plugins.CalendarModule.Data.Migrations {
	public partial class InitialCalendar : Migration {
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.CreateTable(
				name: "tblCalendar",
				columns: table => new {
					CalendarID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
					EventDate = table.Column<DateTime>(type: "datetime", nullable: true),
					EventTitle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
					EventDetail = table.Column<string>(type: "nvarchar(max)", nullable: true),
					IsActive = table.Column<bool>(type: "bit", nullable: true),
					SiteID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
				},
				constraints: table => {
					table.PrimaryKey("tblCalendar_PK", x => x.CalendarID);
				});
		}

		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropTable(
				name: "tblCalendar");
		}
	}
}
