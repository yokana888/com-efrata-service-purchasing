using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class addingFlagDIspositionOndetailGarmentExternalPurhcaseOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDispositionCreatedAll",
                table: "GarmentExternalPurchaseOrderItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "EPO_POId",
                table: "GarmentDispositionPurchaseDetailss",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDispositionCreatedAll",
                table: "GarmentExternalPurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "EPO_POId",
                table: "GarmentDispositionPurchaseDetailss");
        }
    }
}
