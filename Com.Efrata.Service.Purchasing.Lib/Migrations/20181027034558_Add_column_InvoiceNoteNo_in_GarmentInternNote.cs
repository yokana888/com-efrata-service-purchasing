using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_column_InvoiceNoteNo_in_GarmentInternNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HasUnitReceiptNote",
                table: "GarmentInternNotes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNoteNo",
                table: "GarmentInternNotes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasUnitReceiptNote",
                table: "GarmentInternNotes");

            migrationBuilder.DropColumn(
                name: "InvoiceNoteNo",
                table: "GarmentInternNotes");
        }
    }
}
