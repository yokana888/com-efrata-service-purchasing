using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_Column_paymentDueDate_In_GarmentInternNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasUnitReceiptNote",
                table: "GarmentInternNotes");

            migrationBuilder.DropColumn(
                name: "InvoiceNoteNo",
                table: "GarmentInternNotes");

            migrationBuilder.DropColumn(
                name: "INNo",
                table: "GarmentInternNoteItems");

            migrationBuilder.DropColumn(
                name: "IsPayTax",
                table: "GarmentInternNoteItems");

            migrationBuilder.DropColumn(
                name: "ROId",
                table: "GarmentInternNoteDetails");

            migrationBuilder.DropColumn(
                name: "ReceiptQuantity",
                table: "GarmentInternNoteDetails");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentDueDays",
                table: "GarmentInternNoteDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PaymentDueDate",
                table: "GarmentInternNoteDetails",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "UOMId",
                table: "GarmentInternNoteDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UOMUnit",
                table: "GarmentInternNoteDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentDueDate",
                table: "GarmentInternNoteDetails");

            migrationBuilder.DropColumn(
                name: "UOMId",
                table: "GarmentInternNoteDetails");

            migrationBuilder.DropColumn(
                name: "UOMUnit",
                table: "GarmentInternNoteDetails");

            migrationBuilder.AddColumn<string>(
                name: "HasUnitReceiptNote",
                table: "GarmentInternNotes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNoteNo",
                table: "GarmentInternNotes",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "INNo",
                table: "GarmentInternNoteItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsPayTax",
                table: "GarmentInternNoteItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<double>(
                name: "PaymentDueDays",
                table: "GarmentInternNoteDetails",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<long>(
                name: "ROId",
                table: "GarmentInternNoteDetails",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<double>(
                name: "ReceiptQuantity",
                table: "GarmentInternNoteDetails",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
