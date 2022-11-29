using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class ppn_pph_dpp_PurchasingDisposition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DPP",
                table: "PurchasingDispositions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "IncomeTaxValue",
                table: "PurchasingDispositions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VatValue",
                table: "PurchasingDispositions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DPP",
                table: "PurchasingDispositions");

            migrationBuilder.DropColumn(
                name: "IncomeTaxValue",
                table: "PurchasingDispositions");

            migrationBuilder.DropColumn(
                name: "VatValue",
                table: "PurchasingDispositions");
        }
    }
}
