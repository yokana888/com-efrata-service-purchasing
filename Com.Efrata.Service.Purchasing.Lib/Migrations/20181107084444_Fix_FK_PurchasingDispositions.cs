using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Fix_FK_PurchasingDispositions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchasingDispositionDetails_PurchasingDispositions_PurchasingDispositionItemId",
                table: "PurchasingDispositionDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchasingDispositionDetails_PurchasingDispositionItems_PurchasingDispositionItemId1",
                table: "PurchasingDispositionDetails");

            migrationBuilder.DropIndex(
                name: "IX_PurchasingDispositionDetails_PurchasingDispositionItemId1",
                table: "PurchasingDispositionDetails");

            migrationBuilder.DropColumn(
                name: "PurchasingDispositionItemId1",
                table: "PurchasingDispositionDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasingDispositionDetails_PurchasingDispositionItems_PurchasingDispositionItemId",
                table: "PurchasingDispositionDetails",
                column: "PurchasingDispositionItemId",
                principalTable: "PurchasingDispositionItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchasingDispositionDetails_PurchasingDispositionItems_PurchasingDispositionItemId",
                table: "PurchasingDispositionDetails");

            migrationBuilder.AddColumn<long>(
                name: "PurchasingDispositionItemId1",
                table: "PurchasingDispositionDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchasingDispositionDetails_PurchasingDispositionItemId1",
                table: "PurchasingDispositionDetails",
                column: "PurchasingDispositionItemId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasingDispositionDetails_PurchasingDispositions_PurchasingDispositionItemId",
                table: "PurchasingDispositionDetails",
                column: "PurchasingDispositionItemId",
                principalTable: "PurchasingDispositions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasingDispositionDetails_PurchasingDispositionItems_PurchasingDispositionItemId1",
                table: "PurchasingDispositionDetails",
                column: "PurchasingDispositionItemId1",
                principalTable: "PurchasingDispositionItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
