using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Update_CorrectionConversion_GarmentURN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CorrectionConversion",
                table: "GarmentUnitReceiptNoteItems",
                type: "decimal(38, 20)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CorrectionConversion",
                table: "GarmentUnitReceiptNoteItems",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38, 20)");
        }
    }
}
