using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class incometaxIdIncomeTaxRateIncomeTaxName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncomeTaxType",
                table: "GarmentInvoices");

            migrationBuilder.AddColumn<long>(
                name: "IncomeTaxId",
                table: "GarmentInvoices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "IncomeTaxName",
                table: "GarmentInvoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "IncomeTaxRate",
                table: "GarmentInvoices",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncomeTaxId",
                table: "GarmentInvoices");

            migrationBuilder.DropColumn(
                name: "IncomeTaxName",
                table: "GarmentInvoices");

            migrationBuilder.DropColumn(
                name: "IncomeTaxRate",
                table: "GarmentInvoices");

            migrationBuilder.AddColumn<string>(
                name: "IncomeTaxType",
                table: "GarmentInvoices",
                nullable: true);
        }
    }
}
