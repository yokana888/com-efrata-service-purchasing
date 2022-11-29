using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class addDispositionCreateAndPaidPerDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DispositionAmountCreated",
                table: "GarmentDispositionPurchaseDetailss",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DispositionAmountPaid",
                table: "GarmentDispositionPurchaseDetailss",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DispositionQuantityCreated",
                table: "GarmentDispositionPurchaseDetailss",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DispositionQuantityPaid",
                table: "GarmentDispositionPurchaseDetailss",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DispositionAmountCreated",
                table: "GarmentDispositionPurchaseDetailss");

            migrationBuilder.DropColumn(
                name: "DispositionAmountPaid",
                table: "GarmentDispositionPurchaseDetailss");

            migrationBuilder.DropColumn(
                name: "DispositionQuantityCreated",
                table: "GarmentDispositionPurchaseDetailss");

            migrationBuilder.DropColumn(
                name: "DispositionQuantityPaid",
                table: "GarmentDispositionPurchaseDetailss");
        }
    }
}
