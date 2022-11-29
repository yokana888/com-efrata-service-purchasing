using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_Column_Conversion_and_BasicPrice_in_GarmentUnitExpenditureNoteItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BasicPrice",
                table: "GarmentUnitExpenditureNoteItems",
                type: "decimal(38, 4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Conversion",
                table: "GarmentUnitExpenditureNoteItems",
                type: "decimal(38, 20)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasicPrice",
                table: "GarmentUnitExpenditureNoteItems");

            migrationBuilder.DropColumn(
                name: "Conversion",
                table: "GarmentUnitExpenditureNoteItems");
        }
    }
}
