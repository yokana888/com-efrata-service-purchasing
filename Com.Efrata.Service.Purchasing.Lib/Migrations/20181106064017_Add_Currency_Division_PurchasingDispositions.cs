using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_Currency_Division_PurchasingDispositions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "PurchasingDispositions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CurrencyId",
                table: "PurchasingDispositions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyRate",
                table: "PurchasingDispositions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DivisionCode",
                table: "PurchasingDispositionItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DivisionId",
                table: "PurchasingDispositionItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "DivisionName",
                table: "PurchasingDispositionItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "PurchasingDispositions");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "PurchasingDispositions");

            migrationBuilder.DropColumn(
                name: "CurrencyRate",
                table: "PurchasingDispositions");

            migrationBuilder.DropColumn(
                name: "DivisionCode",
                table: "PurchasingDispositionItems");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "PurchasingDispositionItems");

            migrationBuilder.DropColumn(
                name: "DivisionName",
                table: "PurchasingDispositionItems");
        }
    }
}
