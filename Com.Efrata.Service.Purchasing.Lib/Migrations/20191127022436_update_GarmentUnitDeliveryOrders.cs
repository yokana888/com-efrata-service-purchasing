using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class update_GarmentUnitDeliveryOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UENFromId",
                table: "GarmentUnitDeliveryOrders",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "UENFromNo",
                table: "GarmentUnitDeliveryOrders",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UnitDOFromId",
                table: "GarmentUnitDeliveryOrders",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "UnitDOFromNo",
                table: "GarmentUnitDeliveryOrders",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UENFromId",
                table: "GarmentUnitDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "UENFromNo",
                table: "GarmentUnitDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "UnitDOFromId",
                table: "GarmentUnitDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "UnitDOFromNo",
                table: "GarmentUnitDeliveryOrders");
        }
    }
}
