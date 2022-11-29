using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class DivisonPOExtGarment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DivisionCode",
                table: "GarmentInternalPurchaseOrders",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DivisionId",
                table: "GarmentInternalPurchaseOrders",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DivisionName",
                table: "GarmentInternalPurchaseOrders",
                maxLength: 1000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DivisionCode",
                table: "GarmentInternalPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "GarmentInternalPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "DivisionName",
                table: "GarmentInternalPurchaseOrders");
        }
    }
}
