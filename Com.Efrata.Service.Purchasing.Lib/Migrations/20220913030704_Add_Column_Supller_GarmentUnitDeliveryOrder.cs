using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_Column_Supller_GarmentUnitDeliveryOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SupplierReceiptId",
                table: "GarmentUnitDeliveryOrders",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "SupplierReceiptName",
                table: "GarmentUnitDeliveryOrders",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupplierReceiptId",
                table: "GarmentUnitDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "SupplierReceiptName",
                table: "GarmentUnitDeliveryOrders");
        }
    }
}
