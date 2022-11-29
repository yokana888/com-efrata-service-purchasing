using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Update_PurchasingDispositions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncomeTaxId",
                table: "PurchasingDispositionItems");

            migrationBuilder.DropColumn(
                name: "IncomeTaxName",
                table: "PurchasingDispositionItems");

            migrationBuilder.DropColumn(
                name: "IncomeTaxRate",
                table: "PurchasingDispositionItems");

            migrationBuilder.DropColumn(
                name: "UseIncomeTax",
                table: "PurchasingDispositionItems");

            migrationBuilder.DropColumn(
                name: "UseVat",
                table: "PurchasingDispositionItems");

            migrationBuilder.AddColumn<string>(
                name: "DispositionNo",
                table: "PurchasingDispositions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IncomeTaxId",
                table: "PurchasingDispositions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "IncomeTaxName",
                table: "PurchasingDispositions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "IncomeTaxRate",
                table: "PurchasingDispositions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "UseIncomeTax",
                table: "PurchasingDispositions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseVat",
                table: "PurchasingDispositions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DispositionNo",
                table: "PurchasingDispositions");

            migrationBuilder.DropColumn(
                name: "IncomeTaxId",
                table: "PurchasingDispositions");

            migrationBuilder.DropColumn(
                name: "IncomeTaxName",
                table: "PurchasingDispositions");

            migrationBuilder.DropColumn(
                name: "IncomeTaxRate",
                table: "PurchasingDispositions");

            migrationBuilder.DropColumn(
                name: "UseIncomeTax",
                table: "PurchasingDispositions");

            migrationBuilder.DropColumn(
                name: "UseVat",
                table: "PurchasingDispositions");

            migrationBuilder.AddColumn<long>(
                name: "IncomeTaxId",
                table: "PurchasingDispositionItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "IncomeTaxName",
                table: "PurchasingDispositionItems",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "IncomeTaxRate",
                table: "PurchasingDispositionItems",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "UseIncomeTax",
                table: "PurchasingDispositionItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseVat",
                table: "PurchasingDispositionItems",
                nullable: false,
                defaultValue: false);
        }
    }
}
