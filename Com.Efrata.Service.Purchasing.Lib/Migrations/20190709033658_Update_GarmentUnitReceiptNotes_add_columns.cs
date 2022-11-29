using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Update_GarmentUnitReceiptNotes_add_columns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DRId",
                table: "GarmentUnitReceiptNotes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "DRNo",
                table: "GarmentUnitReceiptNotes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UENId",
                table: "GarmentUnitReceiptNotes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "UENNo",
                table: "GarmentUnitReceiptNotes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "URNType",
                table: "GarmentUnitReceiptNotes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CorrectionConversion",
                table: "GarmentUnitReceiptNoteItems",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DRId",
                table: "GarmentUnitReceiptNotes");

            migrationBuilder.DropColumn(
                name: "DRNo",
                table: "GarmentUnitReceiptNotes");

            migrationBuilder.DropColumn(
                name: "UENId",
                table: "GarmentUnitReceiptNotes");

            migrationBuilder.DropColumn(
                name: "UENNo",
                table: "GarmentUnitReceiptNotes");

            migrationBuilder.DropColumn(
                name: "URNType",
                table: "GarmentUnitReceiptNotes");

            migrationBuilder.DropColumn(
                name: "CorrectionConversion",
                table: "GarmentUnitReceiptNoteItems");
        }
    }
}
