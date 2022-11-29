using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Update_Column_Garment_Unit_Delivery_Order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CorrectionId",
                table: "GarmentUnitDeliveryOrders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "CorrectionNo",
                table: "GarmentUnitDeliveryOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DOId",
                table: "GarmentUnitDeliveryOrders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "DONo",
                table: "GarmentUnitDeliveryOrders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectionId",
                table: "GarmentUnitDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "CorrectionNo",
                table: "GarmentUnitDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "DOId",
                table: "GarmentUnitDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "DONo",
                table: "GarmentUnitDeliveryOrders");
        }
    }
}
