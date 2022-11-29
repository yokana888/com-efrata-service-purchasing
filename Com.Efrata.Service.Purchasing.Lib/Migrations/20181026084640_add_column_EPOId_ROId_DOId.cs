using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class add_column_EPOId_ROId_DOId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RONo",
                table: "GarmentInternNoteDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EPONo",
                table: "GarmentInternNoteDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "DOId",
                table: "GarmentInternNoteDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "EPOId",
                table: "GarmentInternNoteDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ROId",
                table: "GarmentInternNoteDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DOId",
                table: "GarmentInternNoteDetails");

            migrationBuilder.DropColumn(
                name: "EPOId",
                table: "GarmentInternNoteDetails");

            migrationBuilder.DropColumn(
                name: "ROId",
                table: "GarmentInternNoteDetails");

            migrationBuilder.AlterColumn<string>(
                name: "RONo",
                table: "GarmentInternNoteDetails",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "EPONo",
                table: "GarmentInternNoteDetails",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
