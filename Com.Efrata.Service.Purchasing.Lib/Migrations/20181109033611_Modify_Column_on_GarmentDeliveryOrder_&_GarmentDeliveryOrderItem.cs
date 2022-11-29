using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Modify_Column_on_GarmentDeliveryOrder__GarmentDeliveryOrderItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "GarmentDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "GarmentDeliveryOrders");

            migrationBuilder.AddColumn<string>(
                name: "DOCurrencyCode",
                table: "GarmentDeliveryOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DOCurrencyId",
                table: "GarmentDeliveryOrders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DOCurrencyRate",
                table: "GarmentDeliveryOrders",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "GarmentDeliveryOrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CurrencyId",
                table: "GarmentDeliveryOrderItems",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DOCurrencyCode",
                table: "GarmentDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "DOCurrencyId",
                table: "GarmentDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "DOCurrencyRate",
                table: "GarmentDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "GarmentDeliveryOrderItems");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "GarmentDeliveryOrderItems");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "GarmentDeliveryOrders",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CurrencyId",
                table: "GarmentDeliveryOrders",
                nullable: true);
        }
    }
}
