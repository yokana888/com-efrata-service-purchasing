using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Update_TableName_ColumnName_Closing_Date : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GarmentClosingDates",
                table: "GarmentClosingDates");

            migrationBuilder.RenameTable(
                name: "GarmentClosingDates",
                newName: "ClosingDate");

            migrationBuilder.RenameColumn(
                name: "ClosingDate",
                table: "ClosingDate",
                newName: "CloseDate");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClosingDate",
                table: "ClosingDate",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ClosingDate",
                table: "ClosingDate");

            migrationBuilder.RenameTable(
                name: "ClosingDate",
                newName: "GarmentClosingDates");

            migrationBuilder.RenameColumn(
                name: "CloseDate",
                table: "GarmentClosingDates",
                newName: "ClosingDate");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarmentClosingDates",
                table: "GarmentClosingDates",
                column: "Id");
        }
    }
}
