using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class add_retur_garment_DO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ReturQuantity",
                table: "GarmentUnitDeliveryOrderItems",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ReturUomId",
                table: "GarmentUnitDeliveryOrderItems",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturUomUnit",
                table: "GarmentUnitDeliveryOrderItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturQuantity",
                table: "GarmentUnitDeliveryOrderItems");

            migrationBuilder.DropColumn(
                name: "ReturUomId",
                table: "GarmentUnitDeliveryOrderItems");

            migrationBuilder.DropColumn(
                name: "ReturUomUnit",
                table: "GarmentUnitDeliveryOrderItems");
        }
    }
}
