using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class change_Column_Garment_Delivery_Order_and_Garment_Delivery_Order_item_and_Garment_Delivery_Order_Detail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncomeTaxId",
                table: "GarmentDeliveryOrderItems");

            migrationBuilder.DropColumn(
                name: "IncomeTaxName",
                table: "GarmentDeliveryOrderItems");

            migrationBuilder.DropColumn(
                name: "IncomeTaxRate",
                table: "GarmentDeliveryOrderItems");

            migrationBuilder.DropColumn(
                name: "UseIncomeTax",
                table: "GarmentDeliveryOrderItems");

            migrationBuilder.DropColumn(
                name: "UseVat",
                table: "GarmentDeliveryOrderItems");

            migrationBuilder.AddColumn<int>(
                name: "IncomeTaxId",
                table: "GarmentDeliveryOrders",
                type: "int",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncomeTaxName",
                table: "GarmentDeliveryOrders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "IncomeTaxRate",
                table: "GarmentDeliveryOrders",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCorrection",
                table: "GarmentDeliveryOrders",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UseIncomeTax",
                table: "GarmentDeliveryOrders",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UseVat",
                table: "GarmentDeliveryOrders",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PricePerDealUnitCorrection",
                table: "GarmentDeliveryOrderDetails",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PriceTotalCorrection",
                table: "GarmentDeliveryOrderDetails",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "QuantityCorrection",
                table: "GarmentDeliveryOrderDetails",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncomeTaxId",
                table: "GarmentDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "IncomeTaxName",
                table: "GarmentDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "IncomeTaxRate",
                table: "GarmentDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "IsCorrection",
                table: "GarmentDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "UseIncomeTax",
                table: "GarmentDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "UseVat",
                table: "GarmentDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "PricePerDealUnitCorrection",
                table: "GarmentDeliveryOrderDetails");

            migrationBuilder.DropColumn(
                name: "PriceTotalCorrection",
                table: "GarmentDeliveryOrderDetails");

            migrationBuilder.DropColumn(
                name: "QuantityCorrection",
                table: "GarmentDeliveryOrderDetails");

            migrationBuilder.AddColumn<int>(
                name: "IncomeTaxId",
                table: "GarmentDeliveryOrderItems",
                maxLength: 255,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IncomeTaxName",
                table: "GarmentDeliveryOrderItems",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "IncomeTaxRate",
                table: "GarmentDeliveryOrderItems",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "UseIncomeTax",
                table: "GarmentDeliveryOrderItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseVat",
                table: "GarmentDeliveryOrderItems",
                nullable: false,
                defaultValue: false);
        }
    }
}
