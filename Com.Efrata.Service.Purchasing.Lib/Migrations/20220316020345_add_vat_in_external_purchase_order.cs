using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class add_vat_in_external_purchase_order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Vat",
                table: "GarmentExternalPurchaseOrders",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VatId",
                table: "GarmentExternalPurchaseOrders",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Vat",
                table: "GarmentExternalPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "VatId",
                table: "GarmentExternalPurchaseOrders");
        }
    }
}
