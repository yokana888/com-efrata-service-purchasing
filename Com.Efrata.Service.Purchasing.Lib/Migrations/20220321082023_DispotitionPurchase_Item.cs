using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class DispotitionPurchase_Item : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VatId",
                table: "PurchasingDispositionItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VatRate",
                table: "PurchasingDispositionItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VatId",
                table: "PurchasingDispositionItems");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "PurchasingDispositionItems");
        }
    }
}
