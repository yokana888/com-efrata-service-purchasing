using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_Column_DOCurrencyRate_on_GarmentUnitDOItem_GarmentUnitReceiptNote_GarmentUnitExpenditureNoteItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DOCurrencyRate",
                table: "GarmentUnitReceiptNotes",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DOCurrencyRate",
                table: "GarmentUnitExpenditureNoteItems",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DOCurrencyRate",
                table: "GarmentUnitDeliveryOrderItems",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DOCurrencyRate",
                table: "GarmentUnitReceiptNotes");

            migrationBuilder.DropColumn(
                name: "DOCurrencyRate",
                table: "GarmentUnitExpenditureNoteItems");

            migrationBuilder.DropColumn(
                name: "DOCurrencyRate",
                table: "GarmentUnitDeliveryOrderItems");
        }
    }
}
