using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class GarmentPR_Item_OpenPO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApprovedOpenPOMD",
                table: "GarmentPurchaseRequestItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsApprovedOpenPOPurchasing",
                table: "GarmentPurchaseRequestItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOpenPO",
                table: "GarmentPurchaseRequestItems",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApprovedOpenPOMD",
                table: "GarmentPurchaseRequestItems");

            migrationBuilder.DropColumn(
                name: "IsApprovedOpenPOPurchasing",
                table: "GarmentPurchaseRequestItems");

            migrationBuilder.DropColumn(
                name: "IsOpenPO",
                table: "GarmentPurchaseRequestItems");
        }
    }
}
