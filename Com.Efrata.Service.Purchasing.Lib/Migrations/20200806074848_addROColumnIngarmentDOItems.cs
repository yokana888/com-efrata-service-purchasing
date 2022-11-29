using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class addROColumnIngarmentDOItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "URNItemId",
                table: "GarmentDOItems",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RO",
                table: "GarmentDOItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RO",
                table: "GarmentDOItems");

            migrationBuilder.AlterColumn<string>(
                name: "URNItemId",
                table: "GarmentDOItems",
                nullable: true,
                oldClrType: typeof(long));
        }
    }
}
