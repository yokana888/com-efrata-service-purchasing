using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class PRType_GarmentPurchaseRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsValidate",
                table: "GarmentPurchaseRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PRType",
                table: "GarmentPurchaseRequests",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SCId",
                table: "GarmentPurchaseRequests",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "SCNo",
                table: "GarmentPurchaseRequests",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PriceConversion",
                table: "GarmentPurchaseRequestItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<long>(
                name: "PriceUomId",
                table: "GarmentPurchaseRequestItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "PriceUomUnit",
                table: "GarmentPurchaseRequestItems",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsValidate",
                table: "GarmentPurchaseRequests");

            migrationBuilder.DropColumn(
                name: "PRType",
                table: "GarmentPurchaseRequests");

            migrationBuilder.DropColumn(
                name: "SCId",
                table: "GarmentPurchaseRequests");

            migrationBuilder.DropColumn(
                name: "SCNo",
                table: "GarmentPurchaseRequests");

            migrationBuilder.DropColumn(
                name: "PriceConversion",
                table: "GarmentPurchaseRequestItems");

            migrationBuilder.DropColumn(
                name: "PriceUomId",
                table: "GarmentPurchaseRequestItems");

            migrationBuilder.DropColumn(
                name: "PriceUomUnit",
                table: "GarmentPurchaseRequestItems");
        }
    }
}
