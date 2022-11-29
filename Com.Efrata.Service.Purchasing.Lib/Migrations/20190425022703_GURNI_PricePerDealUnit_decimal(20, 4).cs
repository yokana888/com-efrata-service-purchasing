using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class GURNI_PricePerDealUnit_decimal204 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PricePerDealUnit",
                table: "GarmentUnitReceiptNoteItems",
                type: "decimal(20, 4)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PricePerDealUnit",
                table: "GarmentUnitReceiptNoteItems",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20, 4)");
        }
    }
}
