using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class editURN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "POId",
                table: "UnitReceiptNoteItems");

            migrationBuilder.DropColumn(
                name: "PRDetailId",
                table: "UnitReceiptNoteItems");

            migrationBuilder.AddColumn<long>(
                name: "PRItemId",
                table: "UnitReceiptNoteItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PRItemId",
                table: "UnitReceiptNoteItems");

            migrationBuilder.AddColumn<long>(
                name: "POId",
                table: "UnitReceiptNoteItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PRDetailId",
                table: "UnitReceiptNoteItems",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
