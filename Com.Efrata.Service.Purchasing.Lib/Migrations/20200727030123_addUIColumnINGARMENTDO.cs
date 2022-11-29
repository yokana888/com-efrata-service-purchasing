using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class addUIColumnINGARMENTDO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UId",
                table: "GarmentDeliveryOrderItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Uid",
                table: "GarmentDeliveryOrderDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UId",
                table: "GarmentDeliveryOrderItems");

            migrationBuilder.DropColumn(
                name: "Uid",
                table: "GarmentDeliveryOrderDetails");
        }
    }
}
