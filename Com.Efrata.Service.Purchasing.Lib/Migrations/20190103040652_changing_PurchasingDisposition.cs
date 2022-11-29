using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class changing_PurchasingDisposition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Investation",
                table: "PurchasingDispositions");

            migrationBuilder.DropColumn(
                name: "InvoiceNo",
                table: "PurchasingDispositions");

            migrationBuilder.AlterColumn<double>(
                name: "CurrencyRate",
                table: "PurchasingDispositions",
                type: "float",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyDescription",
                table: "PurchasingDispositions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncomeTaxBy",
                table: "PurchasingDispositions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyDescription",
                table: "PurchasingDispositions");

            migrationBuilder.DropColumn(
                name: "IncomeTaxBy",
                table: "PurchasingDispositions");

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyRate",
                table: "PurchasingDispositions",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "Investation",
                table: "PurchasingDispositions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNo",
                table: "PurchasingDispositions",
                nullable: true);
        }
    }
}
