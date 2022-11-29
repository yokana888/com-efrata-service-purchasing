using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class addingDispositionValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DispositionAmountCreated",
                table: "GarmentDispositionPurchaseItems",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DispositionAmountPaid",
                table: "GarmentDispositionPurchaseItems",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DispositionQuantityCreated",
                table: "GarmentDispositionPurchaseItems",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DispositionQuantityPaid",
                table: "GarmentDispositionPurchaseItems",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DispositionAmountCreated",
                table: "GarmentDispositionPurchaseItems");

            migrationBuilder.DropColumn(
                name: "DispositionAmountPaid",
                table: "GarmentDispositionPurchaseItems");

            migrationBuilder.DropColumn(
                name: "DispositionQuantityCreated",
                table: "GarmentDispositionPurchaseItems");

            migrationBuilder.DropColumn(
                name: "DispositionQuantityPaid",
                table: "GarmentDispositionPurchaseItems");
        }
    }
}
