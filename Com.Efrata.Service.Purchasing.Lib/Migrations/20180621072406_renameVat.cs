using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class renameVat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VatId",
                table: "ExternalPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "VatName",
                table: "ExternalPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "ExternalPurchaseOrders");

            migrationBuilder.AddColumn<string>(
                name: "IncomeTaxId",
                table: "ExternalPurchaseOrders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncomeTaxName",
                table: "ExternalPurchaseOrders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncomeTaxRate",
                table: "ExternalPurchaseOrders",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncomeTaxId",
                table: "ExternalPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "IncomeTaxName",
                table: "ExternalPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "IncomeTaxRate",
                table: "ExternalPurchaseOrders");

            migrationBuilder.AddColumn<string>(
                name: "VatId",
                table: "ExternalPurchaseOrders",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VatName",
                table: "ExternalPurchaseOrders",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VatRate",
                table: "ExternalPurchaseOrders",
                maxLength: 1000,
                nullable: true);
        }
    }
}
