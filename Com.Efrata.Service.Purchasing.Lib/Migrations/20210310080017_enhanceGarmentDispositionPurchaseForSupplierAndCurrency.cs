using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class enhanceGarmentDispositionPurchaseForSupplierAndCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarmentDispositionPurchaseDetail_GarmentDispositionPurchaseItem_GarmentDispositionPurchaseItemId",
                table: "GarmentDispositionPurchaseDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_GarmentDispositionPurchaseItem_GarmentDispositionPurchases_GarmentDispositionPurchaseId",
                table: "GarmentDispositionPurchaseItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GarmentDispositionPurchaseItem",
                table: "GarmentDispositionPurchaseItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GarmentDispositionPurchaseDetail",
                table: "GarmentDispositionPurchaseDetail");

            migrationBuilder.RenameTable(
                name: "GarmentDispositionPurchaseItem",
                newName: "GarmentDispositionPurchaseItems");

            migrationBuilder.RenameTable(
                name: "GarmentDispositionPurchaseDetail",
                newName: "GarmentDispositionPurchaseDetailss");

            migrationBuilder.RenameIndex(
                name: "IX_GarmentDispositionPurchaseItem_GarmentDispositionPurchaseId",
                table: "GarmentDispositionPurchaseItems",
                newName: "IX_GarmentDispositionPurchaseItems_GarmentDispositionPurchaseId");

            migrationBuilder.RenameIndex(
                name: "IX_GarmentDispositionPurchaseDetail_GarmentDispositionPurchaseItemId",
                table: "GarmentDispositionPurchaseDetailss",
                newName: "IX_GarmentDispositionPurchaseDetailss_GarmentDispositionPurchaseItemId");

            migrationBuilder.AddColumn<string>(
                name: "SupplierCode",
                table: "GarmentDispositionPurchases",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SupplierIsImport",
                table: "GarmentDispositionPurchases",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "GarmentDispositionPurchaseItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "GarmentDispositionPurchaseItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "CurrencyRate",
                table: "GarmentDispositionPurchaseItems",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarmentDispositionPurchaseItems",
                table: "GarmentDispositionPurchaseItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarmentDispositionPurchaseDetailss",
                table: "GarmentDispositionPurchaseDetailss",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GarmentDispositionPurchaseDetailss_GarmentDispositionPurchaseItems_GarmentDispositionPurchaseItemId",
                table: "GarmentDispositionPurchaseDetailss",
                column: "GarmentDispositionPurchaseItemId",
                principalTable: "GarmentDispositionPurchaseItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GarmentDispositionPurchaseItems_GarmentDispositionPurchases_GarmentDispositionPurchaseId",
                table: "GarmentDispositionPurchaseItems",
                column: "GarmentDispositionPurchaseId",
                principalTable: "GarmentDispositionPurchases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GarmentDispositionPurchaseDetailss_GarmentDispositionPurchaseItems_GarmentDispositionPurchaseItemId",
                table: "GarmentDispositionPurchaseDetailss");

            migrationBuilder.DropForeignKey(
                name: "FK_GarmentDispositionPurchaseItems_GarmentDispositionPurchases_GarmentDispositionPurchaseId",
                table: "GarmentDispositionPurchaseItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GarmentDispositionPurchaseItems",
                table: "GarmentDispositionPurchaseItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GarmentDispositionPurchaseDetailss",
                table: "GarmentDispositionPurchaseDetailss");

            migrationBuilder.DropColumn(
                name: "SupplierCode",
                table: "GarmentDispositionPurchases");

            migrationBuilder.DropColumn(
                name: "SupplierIsImport",
                table: "GarmentDispositionPurchases");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "GarmentDispositionPurchaseItems");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "GarmentDispositionPurchaseItems");

            migrationBuilder.DropColumn(
                name: "CurrencyRate",
                table: "GarmentDispositionPurchaseItems");

            migrationBuilder.RenameTable(
                name: "GarmentDispositionPurchaseItems",
                newName: "GarmentDispositionPurchaseItem");

            migrationBuilder.RenameTable(
                name: "GarmentDispositionPurchaseDetailss",
                newName: "GarmentDispositionPurchaseDetail");

            migrationBuilder.RenameIndex(
                name: "IX_GarmentDispositionPurchaseItems_GarmentDispositionPurchaseId",
                table: "GarmentDispositionPurchaseItem",
                newName: "IX_GarmentDispositionPurchaseItem_GarmentDispositionPurchaseId");

            migrationBuilder.RenameIndex(
                name: "IX_GarmentDispositionPurchaseDetailss_GarmentDispositionPurchaseItemId",
                table: "GarmentDispositionPurchaseDetail",
                newName: "IX_GarmentDispositionPurchaseDetail_GarmentDispositionPurchaseItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarmentDispositionPurchaseItem",
                table: "GarmentDispositionPurchaseItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GarmentDispositionPurchaseDetail",
                table: "GarmentDispositionPurchaseDetail",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GarmentDispositionPurchaseDetail_GarmentDispositionPurchaseItem_GarmentDispositionPurchaseItemId",
                table: "GarmentDispositionPurchaseDetail",
                column: "GarmentDispositionPurchaseItemId",
                principalTable: "GarmentDispositionPurchaseItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GarmentDispositionPurchaseItem_GarmentDispositionPurchases_GarmentDispositionPurchaseId",
                table: "GarmentDispositionPurchaseItem",
                column: "GarmentDispositionPurchaseId",
                principalTable: "GarmentDispositionPurchases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
