using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carrotware.CMS.Data.Migrations {
	/// <inheritdoc />
	public partial class Cleanup : Migration {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropTable(
				name: "carrot_DataInfo");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder) {
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
		}
	}
}
