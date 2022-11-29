using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class add_CustomsCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomsCategory",
                table: "GarmentUnitReceiptNoteItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomsCategory",
                table: "GarmentUnitExpenditureNoteItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomsCategory",
                table: "GarmentUnitDeliveryOrderItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomsCategory",
                table: "GarmentDOItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomsCategory",
                table: "GarmentDeliveryOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomsCategory",
                table: "GarmentDeliveryOrderDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomsCategory",
                table: "GarmentUnitReceiptNoteItems");

            migrationBuilder.DropColumn(
                name: "CustomsCategory",
                table: "GarmentUnitExpenditureNoteItems");

            migrationBuilder.DropColumn(
                name: "CustomsCategory",
                table: "GarmentUnitDeliveryOrderItems");

            migrationBuilder.DropColumn(
                name: "CustomsCategory",
                table: "GarmentDOItems");

            migrationBuilder.DropColumn(
                name: "CustomsCategory",
                table: "GarmentDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "CustomsCategory",
                table: "GarmentDeliveryOrderDetails");
        }
    }
}
