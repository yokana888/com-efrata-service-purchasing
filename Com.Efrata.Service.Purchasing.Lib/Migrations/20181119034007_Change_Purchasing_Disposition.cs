using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Change_Purchasing_Disposition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DivisionCode",
                table: "PurchasingDispositionItems");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "PurchasingDispositionItems");

            migrationBuilder.DropColumn(
                name: "DivisionName",
                table: "PurchasingDispositionItems");

            migrationBuilder.DropColumn(
                name: "UnitCode",
                table: "PurchasingDispositionItems");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "PurchasingDispositionItems");

            migrationBuilder.DropColumn(
                name: "UnitName",
                table: "PurchasingDispositionItems");

            migrationBuilder.AlterColumn<string>(
                name: "SupplierId",
                table: "PurchasingDispositions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyId",
                table: "PurchasingDispositions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "IncomeTaxId",
                table: "PurchasingDispositionItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "EPOId",
                table: "PurchasingDispositionItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "ProductId",
                table: "PurchasingDispositionDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "PRId",
                table: "PurchasingDispositionDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "EPODetailId",
                table: "PurchasingDispositionDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "DealUomId",
                table: "PurchasingDispositionDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "CategoryId",
                table: "PurchasingDispositionDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<string>(
                name: "DivisionCode",
                table: "PurchasingDispositionDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DivisionId",
                table: "PurchasingDispositionDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DivisionName",
                table: "PurchasingDispositionDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitCode",
                table: "PurchasingDispositionDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitId",
                table: "PurchasingDispositionDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitName",
                table: "PurchasingDispositionDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DivisionCode",
                table: "PurchasingDispositionDetails");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "PurchasingDispositionDetails");

            migrationBuilder.DropColumn(
                name: "DivisionName",
                table: "PurchasingDispositionDetails");

            migrationBuilder.DropColumn(
                name: "UnitCode",
                table: "PurchasingDispositionDetails");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "PurchasingDispositionDetails");

            migrationBuilder.DropColumn(
                name: "UnitName",
                table: "PurchasingDispositionDetails");

            migrationBuilder.AlterColumn<long>(
                name: "SupplierId",
                table: "PurchasingDispositions",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CurrencyId",
                table: "PurchasingDispositions",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "IncomeTaxId",
                table: "PurchasingDispositionItems",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "EPOId",
                table: "PurchasingDispositionItems",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DivisionCode",
                table: "PurchasingDispositionItems",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DivisionId",
                table: "PurchasingDispositionItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "DivisionName",
                table: "PurchasingDispositionItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitCode",
                table: "PurchasingDispositionItems",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UnitId",
                table: "PurchasingDispositionItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "UnitName",
                table: "PurchasingDispositionItems",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ProductId",
                table: "PurchasingDispositionDetails",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "PRId",
                table: "PurchasingDispositionDetails",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "EPODetailId",
                table: "PurchasingDispositionDetails",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DealUomId",
                table: "PurchasingDispositionDetails",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CategoryId",
                table: "PurchasingDispositionDetails",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
