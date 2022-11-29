using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Update_URNItems_Update_ReceiptQuantity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ReceiptQuantity",
                table: "GarmentUnitReceiptNoteItems",
                type: "decimal(20, 4)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ReceiptQuantity",
                table: "GarmentUnitReceiptNoteItems",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20, 4)");
        }
    }
}
