using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class StockOpnameItem_Clone_DOItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarmentStockOpnameItems_GarmentDOItems_DOItemId",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropIndex(
                name: "IX_GarmentStockOpnameItems_DOItemId",
                table: "GarmentStockOpnameItems");

            migrationBuilder.AddColumn<double>(
                name: "DOCurrencyRate",
                table: "GarmentStockOpnameItems",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "DOItemNo",
                table: "GarmentStockOpnameItems",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DesignColor",
                table: "GarmentStockOpnameItems",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DetailReferenceId",
                table: "GarmentStockOpnameItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "EPOItemId",
                table: "GarmentStockOpnameItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "POId",
                table: "GarmentStockOpnameItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "POItemId",
                table: "GarmentStockOpnameItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "POSerialNumber",
                table: "GarmentStockOpnameItems",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PRItemId",
                table: "GarmentStockOpnameItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "GarmentStockOpnameItems",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "GarmentStockOpnameItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "GarmentStockOpnameItems",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RO",
                table: "GarmentStockOpnameItems",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SmallQuantity",
                table: "GarmentStockOpnameItems",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "SmallUomId",
                table: "GarmentStockOpnameItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "SmallUomUnit",
                table: "GarmentStockOpnameItems",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorageCode",
                table: "GarmentStockOpnameItems",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StorageId",
                table: "GarmentStockOpnameItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "StorageName",
                table: "GarmentStockOpnameItems",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UId",
                table: "GarmentStockOpnameItems",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "URNItemId",
                table: "GarmentStockOpnameItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "UnitCode",
                table: "GarmentStockOpnameItems",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UnitId",
                table: "GarmentStockOpnameItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "UnitName",
                table: "GarmentStockOpnameItems",
                maxLength: 1000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DOCurrencyRate",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "DOItemNo",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "DesignColor",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "DetailReferenceId",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "EPOItemId",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "POId",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "POItemId",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "POSerialNumber",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "PRItemId",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "RO",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "SmallQuantity",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "SmallUomId",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "SmallUomUnit",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "StorageCode",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "StorageId",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "StorageName",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "UId",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "URNItemId",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "UnitCode",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "GarmentStockOpnameItems");

            migrationBuilder.DropColumn(
                name: "UnitName",
                table: "GarmentStockOpnameItems");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentStockOpnameItems_DOItemId",
                table: "GarmentStockOpnameItems",
                column: "DOItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_GarmentStockOpnameItems_GarmentDOItems_DOItemId",
                table: "GarmentStockOpnameItems",
                column: "DOItemId",
                principalTable: "GarmentDOItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
