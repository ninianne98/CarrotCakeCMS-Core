using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarrotCake.CMS.Plugins.EventCalendarModule.Data.Migrations {
	public partial class InitialEventCalendar : Migration {
		protected override void Up(MigrationBuilder migrationBuilder) {

			string sql = DataHelper.GetScript("Script01_UpCalendarEvent.sql");
			migrationBuilder.Sql(sql);
		}

		protected override void Down(MigrationBuilder migrationBuilder) {

			string sql = DataHelper.GetScript("Script01_DownCalendarEvent.sql");
			migrationBuilder.Sql(sql);
		}
	}
}
