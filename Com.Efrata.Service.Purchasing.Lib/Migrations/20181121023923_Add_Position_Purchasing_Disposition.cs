using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_Position_Purchasing_Disposition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EPODetailId",
                table: "PurchasingDispositionDetails");

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "PurchasingDispositions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "PurchasingDispositions");

            migrationBuilder.AddColumn<string>(
                name: "EPODetailId",
                table: "PurchasingDispositionDetails",
                nullable: true);
        }
    }
}
