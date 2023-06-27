using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carrotware.CMS.Data.Migrations {
	/// <inheritdoc />
	public partial class FixAuthTables : Migration {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropForeignKey(
				name: "FK_AspNetUserRoles_AspNetUsers_UserId",
				table: "AspNetUserRoles");

			migrationBuilder.DropPrimaryKey(
				name: "PK_AspNetUserRoles",
				table: "AspNetUserRoles");

			migrationBuilder.DropColumn(
				name: "AcceptTrackbacks",
				table: "carrot_Sites");

			migrationBuilder.DropColumn(
				name: "SendTrackbacks",
				table: "carrot_Sites");

			migrationBuilder.AlterColumn<string>(
				name: "UserId",
				table: "AspNetUserTokens",
				type: "nvarchar(128)",
				maxLength: 128,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(450)");

			migrationBuilder.AlterColumn<string>(
				name: "Id",
				table: "AspNetUsers",
				type: "nvarchar(128)",
				maxLength: 128,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(450)");

			migrationBuilder.AlterColumn<string>(
				name: "RoleId",
				table: "AspNetUserRoles",
				type: "nvarchar(128)",
				maxLength: 128,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(450)");

			migrationBuilder.AlterColumn<string>(
				name: "UserId",
				table: "AspNetUserRoles",
				type: "nvarchar(128)",
				maxLength: 128,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(450)");

			migrationBuilder.AlterColumn<string>(
				name: "UserId",
				table: "AspNetUserLogins",
				type: "nvarchar(128)",
				maxLength: 128,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(450)");

			migrationBuilder.AlterColumn<string>(
				name: "UserId",
				table: "AspNetUserClaims",
				type: "nvarchar(128)",
				maxLength: 128,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(450)");

			migrationBuilder.AlterColumn<string>(
				name: "Id",
				table: "AspNetRoles",
				type: "nvarchar(128)",
				maxLength: 128,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(450)");

			migrationBuilder.AlterColumn<string>(
				name: "RoleId",
				table: "AspNetRoleClaims",
				type: "nvarchar(128)",
				maxLength: 128,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(450)");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.AddColumn<bool>(
				name: "AcceptTrackbacks",
				table: "carrot_Sites",
				type: "bit",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<bool>(
				name: "SendTrackbacks",
				table: "carrot_Sites",
				type: "bit",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AlterColumn<string>(
				name: "UserId",
				table: "AspNetUserTokens",
				type: "nvarchar(450)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(128)",
				oldMaxLength: 128);

			migrationBuilder.AlterColumn<string>(
				name: "Id",
				table: "AspNetUsers",
				type: "nvarchar(450)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(128)",
				oldMaxLength: 128);

			migrationBuilder.AlterColumn<string>(
				name: "UserId",
				table: "AspNetUserRoles",
				type: "nvarchar(450)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(128)",
				oldMaxLength: 128);

			migrationBuilder.AlterColumn<string>(
				name: "RoleId",
				table: "AspNetUserRoles",
				type: "nvarchar(450)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(128)",
				oldMaxLength: 128);

			migrationBuilder.AlterColumn<string>(
				name: "UserId",
				table: "AspNetUserLogins",
				type: "nvarchar(450)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(128)",
				oldMaxLength: 128);

			migrationBuilder.AlterColumn<string>(
				name: "UserId",
				table: "AspNetUserClaims",
				type: "nvarchar(450)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(128)",
				oldMaxLength: 128);

			migrationBuilder.AlterColumn<string>(
				name: "Id",
				table: "AspNetRoles",
				type: "nvarchar(450)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(128)",
				oldMaxLength: 128);

			migrationBuilder.AlterColumn<string>(
				name: "RoleId",
				table: "AspNetRoleClaims",
				type: "nvarchar(450)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(128)",
				oldMaxLength: 128);

			migrationBuilder.AddPrimaryKey(
				name: "PK_AspNetUserRoles",
				table: "AspNetUserRoles",
				columns: new[] { "UserId", "RoleId" });

			migrationBuilder.AddForeignKey(
				name: "FK_AspNetUserRoles_AspNetUsers_UserId",
				table: "AspNetUserRoles",
				column: "UserId",
				principalTable: "AspNetUsers",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}
