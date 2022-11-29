using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_EPONo_EPOId_IncomeTaxBy_UnitReceiptNoteItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EPOId",
                table: "UnitReceiptNoteItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "EPONo",
                table: "UnitReceiptNoteItems",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncomeTaxBy",
                table: "UnitReceiptNoteItems",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EPOId",
                table: "UnitReceiptNoteItems");

            migrationBuilder.DropColumn(
                name: "EPONo",
                table: "UnitReceiptNoteItems");

            migrationBuilder.DropColumn(
                name: "IncomeTaxBy",
                table: "UnitReceiptNoteItems");
        }
    }
}
