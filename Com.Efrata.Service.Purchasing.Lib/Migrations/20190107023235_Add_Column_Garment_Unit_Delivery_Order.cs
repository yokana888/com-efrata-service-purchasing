using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_Column_Garment_Unit_Delivery_Order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StorageRequestCode",
                table: "GarmentUnitDeliveryOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StorageRequestId",
                table: "GarmentUnitDeliveryOrders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "StorageRequestName",
                table: "GarmentUnitDeliveryOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DesignColor",
                table: "GarmentUnitDeliveryOrderItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StorageRequestCode",
                table: "GarmentUnitDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "StorageRequestId",
                table: "GarmentUnitDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "StorageRequestName",
                table: "GarmentUnitDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "DesignColor",
                table: "GarmentUnitDeliveryOrderItems");
        }
    }
}
