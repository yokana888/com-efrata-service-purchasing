using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_Coloumn_VatId_Vat_Rate_External_Purchase_Order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VatId",
                table: "ExternalPurchaseOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VatRate",
                table: "ExternalPurchaseOrders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VatId",
                table: "ExternalPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "ExternalPurchaseOrders");
        }
    }
}
