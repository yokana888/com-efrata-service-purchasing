using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class DPPVATIsPaidInternalNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DPPVATIsPaid",
                table: "GarmentInvoices",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DPPVATIsPaid",
                table: "GarmentInternNotes",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DPPVATIsPaid",
                table: "GarmentInvoices");

            migrationBuilder.DropColumn(
                name: "DPPVATIsPaid",
                table: "GarmentInternNotes");
        }
    }
}
