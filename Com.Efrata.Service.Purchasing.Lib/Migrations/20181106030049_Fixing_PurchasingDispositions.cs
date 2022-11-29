using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Fixing_PurchasingDispositions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "IncomeTaxName",
                table: "PurchasingDispositionItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "IncomeTaxRate",
                table: "PurchasingDispositionItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "UseIncomeTax",
                table: "PurchasingDispositionItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseVat",
                table: "PurchasingDispositionItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<long>(
                name: "IncomeTaxId",
                table: "PurchasingDispositions",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "IncomeTaxName",
                table: "PurchasingDispositions",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "IncomeTaxRate",
                table: "PurchasingDispositions",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "UseIncomeTax",
                table: "PurchasingDispositions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseVat",
                table: "PurchasingDispositions",
                nullable: false,
                defaultValue: false);
        }
    }
}
