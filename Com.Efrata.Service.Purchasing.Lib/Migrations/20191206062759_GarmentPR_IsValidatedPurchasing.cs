using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class GarmentPR_IsValidatedPurchasing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsValidatedPurchasing",
                table: "GarmentPurchaseRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ValidatedPurchasingBy",
                table: "GarmentPurchaseRequests",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidatedPurchasingDate",
                table: "GarmentPurchaseRequests",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsValidatedPurchasing",
                table: "GarmentPurchaseRequests");

            migrationBuilder.DropColumn(
                name: "ValidatedPurchasingBy",
                table: "GarmentPurchaseRequests");

            migrationBuilder.DropColumn(
                name: "ValidatedPurchasingDate",
                table: "GarmentPurchaseRequests");
        }
    }
}
