using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carrotware.CMS.Data.Migrations {
	public partial class AuthUserFK : Migration {
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.AddForeignKey(
				name: "FK_carrot_UserData_AspNetUsers",
				table: "carrot_UserData",
				column: "UserKey",
				principalTable: "AspNetUsers",
				principalColumn: "Id");

			string sql = DataHelper.GetScript("20230611_Update.sql");
			migrationBuilder.Sql(sql);
		}

		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropForeignKey(
				name: "FK_carrot_UserData_AspNetUsers",
				table: "carrot_UserData");
		}
	}
}
