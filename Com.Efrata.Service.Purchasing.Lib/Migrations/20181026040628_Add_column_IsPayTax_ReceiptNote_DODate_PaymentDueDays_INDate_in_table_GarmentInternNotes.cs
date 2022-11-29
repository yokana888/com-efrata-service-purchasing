using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_column_IsPayTax_ReceiptNote_DODate_PaymentDueDays_INDate_in_table_GarmentInternNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarmentInternNoteDetails_GarmentInternNoteItems_INItemId",
                table: "GarmentInternNoteDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_GarmentInternNoteItems_GarmentInternNotes_INNo",
                table: "GarmentInternNoteItems");

            migrationBuilder.DropIndex(
                name: "IX_GarmentInternNoteItems_INNo",
                table: "GarmentInternNoteItems");

            migrationBuilder.DropIndex(
                name: "IX_GarmentInternNoteDetails_INItemId",
                table: "GarmentInternNoteDetails");

            migrationBuilder.DropColumn(
                name: "GarmentDOId",
                table: "GarmentInternNoteDetails");

            migrationBuilder.DropColumn(
                name: "GarmentINDetailId",
                table: "GarmentInternNoteDetails");

            migrationBuilder.DropColumn(
                name: "INDetailId",
                table: "GarmentInternNoteDetails");

            migrationBuilder.DropColumn(
                name: "INItemId",
                table: "GarmentInternNoteDetails");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "INDate",
                table: "GarmentInternNotes",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "IsPayTax",
                table: "GarmentInternNoteItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DODate",
                table: "GarmentInternNoteDetails",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<long>(
                name: "GarmentItemINId",
                table: "GarmentInternNoteDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<double>(
                name: "PaymentDueDays",
                table: "GarmentInternNoteDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ReceiptQuantity",
                table: "GarmentInternNoteDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_GarmentInternNoteItems_GarmentINId",
                table: "GarmentInternNoteItems",
                column: "GarmentINId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentInternNoteDetails_GarmentItemINId",
                table: "GarmentInternNoteDetails",
                column: "GarmentItemINId");

            migrationBuilder.AddForeignKey(
                name: "FK_GarmentInternNoteDetails_GarmentInternNoteItems_GarmentItemINId",
                table: "GarmentInternNoteDetails",
                column: "GarmentItemINId",
                principalTable: "GarmentInternNoteItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GarmentInternNoteItems_GarmentInternNotes_GarmentINId",
                table: "GarmentInternNoteItems",
                column: "GarmentINId",
                principalTable: "GarmentInternNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarmentInternNoteDetails_GarmentInternNoteItems_GarmentItemINId",
                table: "GarmentInternNoteDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_GarmentInternNoteItems_GarmentInternNotes_GarmentINId",
                table: "GarmentInternNoteItems");

            migrationBuilder.DropIndex(
                name: "IX_GarmentInternNoteItems_GarmentINId",
                table: "GarmentInternNoteItems");

            migrationBuilder.DropIndex(
                name: "IX_GarmentInternNoteDetails_GarmentItemINId",
                table: "GarmentInternNoteDetails");

            migrationBuilder.DropColumn(
                name: "INDate",
                table: "GarmentInternNotes");

            migrationBuilder.DropColumn(
                name: "IsPayTax",
                table: "GarmentInternNoteItems");

            migrationBuilder.DropColumn(
                name: "DODate",
                table: "GarmentInternNoteDetails");

            migrationBuilder.DropColumn(
                name: "GarmentItemINId",
                table: "GarmentInternNoteDetails");

            migrationBuilder.DropColumn(
                name: "PaymentDueDays",
                table: "GarmentInternNoteDetails");

            migrationBuilder.DropColumn(
                name: "ReceiptQuantity",
                table: "GarmentInternNoteDetails");

            migrationBuilder.AddColumn<long>(
                name: "GarmentDOId",
                table: "GarmentInternNoteDetails",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "GarmentINDetailId",
                table: "GarmentInternNoteDetails",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "INDetailId",
                table: "GarmentInternNoteDetails",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "INItemId",
                table: "GarmentInternNoteDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GarmentInternNoteItems_INNo",
                table: "GarmentInternNoteItems",
                column: "INNo");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentInternNoteDetails_INItemId",
                table: "GarmentInternNoteDetails",
                column: "INItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_GarmentInternNoteDetails_GarmentInternNoteItems_INItemId",
                table: "GarmentInternNoteDetails",
                column: "INItemId",
                principalTable: "GarmentInternNoteItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GarmentInternNoteItems_GarmentInternNotes_INNo",
                table: "GarmentInternNoteItems",
                column: "INNo",
                principalTable: "GarmentInternNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
