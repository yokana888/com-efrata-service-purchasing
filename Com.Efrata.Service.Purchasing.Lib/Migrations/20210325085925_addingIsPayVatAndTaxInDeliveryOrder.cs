using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class addingIsPayVatAndTaxInDeliveryOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPayIncomeTax",
                table: "GarmentDeliveryOrders",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPayVAT",
                table: "GarmentDeliveryOrders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPayIncomeTax",
                table: "GarmentDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "IsPayVAT",
                table: "GarmentDeliveryOrders");
        }
    }
}
