using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class addPOserialNumberinvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyName",
                table: "GarmentInvoices");

            migrationBuilder.AddColumn<string>(
                name: "POSerialNumber",
                table: "GarmentInvoiceDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "POSerialNumber",
                table: "GarmentInvoiceDetails");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyName",
                table: "GarmentInvoices",
                nullable: true);
        }
    }
}
