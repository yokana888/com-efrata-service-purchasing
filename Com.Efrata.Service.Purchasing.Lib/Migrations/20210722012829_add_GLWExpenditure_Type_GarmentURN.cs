using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class add_GLWExpenditure_Type_GarmentURN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "GarmentUnitReceiptNotes",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ExpenditureId",
                table: "GarmentUnitReceiptNotes",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ExpenditureNo",
                table: "GarmentUnitReceiptNotes",
                maxLength: 20,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "GarmentUnitReceiptNotes");

            migrationBuilder.DropColumn(
                name: "ExpenditureId",
                table: "GarmentUnitReceiptNotes");

            migrationBuilder.DropColumn(
                name: "ExpenditureNo",
                table: "GarmentUnitReceiptNotes");
        }
    }
}
