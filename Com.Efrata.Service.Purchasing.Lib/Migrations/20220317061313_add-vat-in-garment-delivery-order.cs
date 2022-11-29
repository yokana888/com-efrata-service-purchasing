using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class addvatingarmentdeliveryorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VatId",
                table: "GarmentDeliveryOrders",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "VatRate",
                table: "GarmentDeliveryOrders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VatId",
                table: "GarmentDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "GarmentDeliveryOrders");
        }
    }
}
