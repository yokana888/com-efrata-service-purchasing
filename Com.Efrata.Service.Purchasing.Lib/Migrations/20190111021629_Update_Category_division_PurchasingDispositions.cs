using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Update_Category_division_PurchasingDispositions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryCode",
                table: "PurchasingDispositionDetails");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "PurchasingDispositionDetails");

            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "PurchasingDispositionDetails");

            migrationBuilder.DropColumn(
                name: "DivisionCode",
                table: "PurchasingDispositionDetails");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "PurchasingDispositionDetails");

            migrationBuilder.DropColumn(
                name: "DivisionName",
                table: "PurchasingDispositionDetails");

            migrationBuilder.AddColumn<string>(
                name: "CategoryCode",
                table: "PurchasingDispositions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryId",
                table: "PurchasingDispositions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "PurchasingDispositions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DivisionCode",
                table: "PurchasingDispositions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DivisionId",
                table: "PurchasingDispositions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DivisionName",
                table: "PurchasingDispositions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryCode",
                table: "PurchasingDispositions");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "PurchasingDispositions");

            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "PurchasingDispositions");

            migrationBuilder.DropColumn(
                name: "DivisionCode",
                table: "PurchasingDispositions");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "PurchasingDispositions");

            migrationBuilder.DropColumn(
                name: "DivisionName",
                table: "PurchasingDispositions");

            migrationBuilder.AddColumn<string>(
                name: "CategoryCode",
                table: "PurchasingDispositionDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryId",
                table: "PurchasingDispositionDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "PurchasingDispositionDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DivisionCode",
                table: "PurchasingDispositionDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DivisionId",
                table: "PurchasingDispositionDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DivisionName",
                table: "PurchasingDispositionDetails",
                nullable: true);
        }
    }
}
