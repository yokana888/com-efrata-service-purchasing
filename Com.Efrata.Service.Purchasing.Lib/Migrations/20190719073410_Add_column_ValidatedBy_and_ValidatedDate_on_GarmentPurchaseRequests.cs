using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_column_ValidatedBy_and_ValidatedDate_on_GarmentPurchaseRequests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ValidatedBy",
                table: "GarmentPurchaseRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidatedDate",
                table: "GarmentPurchaseRequests",
                type: "datetimeoffset",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidatedBy",
                table: "GarmentPurchaseRequests");

            migrationBuilder.DropColumn(
                name: "ValidatedDate",
                table: "GarmentPurchaseRequests");
        }
    }
}
