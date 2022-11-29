using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class updateColumninvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "SupplierId",
                table: "GarmentInvoices",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "GarmentInvoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CurrencyId",
                table: "GarmentInvoices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyName",
                table: "GarmentInvoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasInternNote",
                table: "GarmentInvoices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "PRItemId",
                table: "GarmentInvoiceDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "PRNo",
                table: "GarmentInvoiceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentDueDays",
                table: "GarmentInvoiceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "GarmentInvoiceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentType",
                table: "GarmentInvoiceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RONo",
                table: "GarmentInvoiceDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "GarmentInvoices");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "GarmentInvoices");

            migrationBuilder.DropColumn(
                name: "CurrencyName",
                table: "GarmentInvoices");

            migrationBuilder.DropColumn(
                name: "HasInternNote",
                table: "GarmentInvoices");

            migrationBuilder.DropColumn(
                name: "PRItemId",
                table: "GarmentInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "PRNo",
                table: "GarmentInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "PaymentDueDays",
                table: "GarmentInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "GarmentInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "GarmentInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "RONo",
                table: "GarmentInvoiceDetails");

            migrationBuilder.AlterColumn<string>(
                name: "SupplierId",
                table: "GarmentInvoices",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
