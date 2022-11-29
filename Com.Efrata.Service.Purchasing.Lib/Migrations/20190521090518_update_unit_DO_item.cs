using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class update_unit_DO_item : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ReturQuantity",
                table: "GarmentUnitDeliveryOrderItems",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ReturQuantity",
                table: "GarmentUnitDeliveryOrderItems",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
