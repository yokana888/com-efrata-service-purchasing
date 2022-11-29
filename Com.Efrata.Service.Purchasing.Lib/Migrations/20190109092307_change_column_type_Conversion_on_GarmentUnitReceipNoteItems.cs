using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class change_column_type_Conversion_on_GarmentUnitReceipNoteItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Conversion",
                table: "GarmentUnitReceiptNoteItems",
                type: "decimal(38, 20)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Conversion",
                table: "GarmentUnitReceiptNoteItems",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38, 20)");
        }
    }
}
