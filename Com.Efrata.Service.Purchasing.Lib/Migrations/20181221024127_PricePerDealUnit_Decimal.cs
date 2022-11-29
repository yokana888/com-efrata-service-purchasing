using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class PricePerDealUnit_Decimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PricePerDealUnitBefore",
                table: "GarmentCorrectionNoteItems",
                type: "decimal(38, 20)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "PricePerDealUnitAfter",
                table: "GarmentCorrectionNoteItems",
                type: "decimal(38, 20)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PricePerDealUnitBefore",
                table: "GarmentCorrectionNoteItems",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38, 20)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PricePerDealUnitAfter",
                table: "GarmentCorrectionNoteItems",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38, 20)");
        }
    }
}
