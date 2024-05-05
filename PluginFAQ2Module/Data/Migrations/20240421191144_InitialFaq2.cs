using Microsoft.EntityFrameworkCore.Migrations;

/*
* CarrotCake CMS (MVC Core)
* http://www.carrotware.com/
*
* Copyright 2015, 2024, Samantha Copeland
* Dual licensed under the MIT or GPL Version 3 licenses.
*
* Date: April 2024
*/

#nullable disable

namespace CarrotCake.CMS.Plugins.FAQ2.Data.Migrations {

	/// <inheritdoc />
	public partial class Initial : Migration {

		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder) {
			string sql = DataHelper.GetScript("Script01_UpFaqItem.sql");
			migrationBuilder.Sql(sql);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropTable(
				name: "carrot_FaqItem");

			migrationBuilder.DropTable(
				name: "carrot_FaqCategory");
		}
	}
}
