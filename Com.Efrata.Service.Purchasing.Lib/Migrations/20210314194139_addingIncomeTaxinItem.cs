using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class addingIncomeTaxinItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IncomeTaxId",
                table: "GarmentDispositionPurchaseItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IncomeTaxName",
                table: "GarmentDispositionPurchaseItems",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "IncomeTaxRate",
                table: "GarmentDispositionPurchaseItems",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncomeTaxId",
                table: "GarmentDispositionPurchaseItems");

            migrationBuilder.DropColumn(
                name: "IncomeTaxName",
                table: "GarmentDispositionPurchaseItems");

            migrationBuilder.DropColumn(
                name: "IncomeTaxRate",
                table: "GarmentDispositionPurchaseItems");
        }
    }
}
