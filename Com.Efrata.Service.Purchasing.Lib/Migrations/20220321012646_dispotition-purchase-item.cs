using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class dispotitionpurchaseitem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VatId",
                table: "GarmentDispositionPurchaseItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VatRate",
                table: "GarmentDispositionPurchaseItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VatId",
                table: "GarmentDispositionPurchaseItems");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "GarmentDispositionPurchaseItems");
        }
    }
}
