using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class EditTableName_GarmentUnitExpenditureNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarmentUnitExpenditureNoteItem_GarmentUnitExpenditureNote_UENId",
                table: "GarmentUnitExpenditureNoteItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GarmentUnitExpenditureNoteItem",
                table: "GarmentUnitExpenditureNoteItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GarmentUnitExpenditureNote",
                table: "GarmentUnitExpenditureNote");

            migrationBuilder.RenameTable(
                name: "GarmentUnitExpenditureNoteItem",
                newName: "GarmentUnitExpenditureNoteItems");

            migrationBuilder.RenameTable(
                name: "GarmentUnitExpenditureNote",
                newName: "GarmentUnitExpenditureNotes");

            migrationBuilder.RenameIndex(
                name: "IX_GarmentUnitExpenditureNoteItem_UENId",
                table: "GarmentUnitExpenditureNoteItems",
                newName: "IX_GarmentUnitExpenditureNoteItems_UENId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarmentUnitExpenditureNoteItems",
                table: "GarmentUnitExpenditureNoteItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarmentUnitExpenditureNotes",
                table: "GarmentUnitExpenditureNotes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GarmentUnitExpenditureNoteItems_GarmentUnitExpenditureNotes_UENId",
                table: "GarmentUnitExpenditureNoteItems",
                column: "UENId",
                principalTable: "GarmentUnitExpenditureNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarmentUnitExpenditureNoteItems_GarmentUnitExpenditureNotes_UENId",
                table: "GarmentUnitExpenditureNoteItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GarmentUnitExpenditureNotes",
                table: "GarmentUnitExpenditureNotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GarmentUnitExpenditureNoteItems",
                table: "GarmentUnitExpenditureNoteItems");

            migrationBuilder.RenameTable(
                name: "GarmentUnitExpenditureNotes",
                newName: "GarmentUnitExpenditureNote");

            migrationBuilder.RenameTable(
                name: "GarmentUnitExpenditureNoteItems",
                newName: "GarmentUnitExpenditureNoteItem");

            migrationBuilder.RenameIndex(
                name: "IX_GarmentUnitExpenditureNoteItems_UENId",
                table: "GarmentUnitExpenditureNoteItem",
                newName: "IX_GarmentUnitExpenditureNoteItem_UENId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarmentUnitExpenditureNote",
                table: "GarmentUnitExpenditureNote",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarmentUnitExpenditureNoteItem",
                table: "GarmentUnitExpenditureNoteItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GarmentUnitExpenditureNoteItem_GarmentUnitExpenditureNote_UENId",
                table: "GarmentUnitExpenditureNoteItem",
                column: "UENId",
                principalTable: "GarmentUnitExpenditureNote",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
