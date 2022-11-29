using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Change_Column_Name_in_Garment_Delivery_Order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomsId",
                table: "GarmentDeliveryOrders");

            migrationBuilder.AddColumn<string>(
                name: "InternNo",
                table: "GarmentDeliveryOrders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternNo",
                table: "GarmentDeliveryOrders");

            migrationBuilder.AddColumn<long>(
                name: "CustomsId",
                table: "GarmentDeliveryOrders",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
