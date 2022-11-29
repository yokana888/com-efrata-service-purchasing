using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class AddingImportValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImportValue",
                table: "GarmentBeacukais",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImportValueId",
                table: "GarmentBeacukais",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportValue",
                table: "GarmentBeacukais");

            migrationBuilder.DropColumn(
                name: "ImportValueId",
                table: "GarmentBeacukais");
        }
    }
}
