using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class PaidTax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPayIncomeTax",
                table: "GarmentExternalPurchaseOrders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPayVAT",
                table: "GarmentExternalPurchaseOrders",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPayIncomeTax",
                table: "GarmentExternalPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "IsPayVAT",
                table: "GarmentExternalPurchaseOrders");
        }
    }
}
