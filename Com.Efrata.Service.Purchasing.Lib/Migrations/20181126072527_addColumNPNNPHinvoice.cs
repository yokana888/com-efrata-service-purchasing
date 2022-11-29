using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class addColumNPNNPHinvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyName",
                table: "GarmentBeacukais");

            migrationBuilder.AddColumn<string>(
                name: "NPH",
                table: "GarmentInvoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NPN",
                table: "GarmentInvoices",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NPH",
                table: "GarmentInvoices");

            migrationBuilder.DropColumn(
                name: "NPN",
                table: "GarmentInvoices");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyName",
                table: "GarmentBeacukais",
                nullable: true);
        }
    }
}
