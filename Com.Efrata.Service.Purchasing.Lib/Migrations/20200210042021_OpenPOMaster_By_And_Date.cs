using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class OpenPOMaster_By_And_Date : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovedOpenPOMDBy",
                table: "GarmentPurchaseRequestItems",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ApprovedOpenPOMDDate",
                table: "GarmentPurchaseRequestItems",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ApprovedOpenPOPurchasingBy",
                table: "GarmentPurchaseRequestItems",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ApprovedOpenPOPurchasingDate",
                table: "GarmentPurchaseRequestItems",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "OpenPOBy",
                table: "GarmentPurchaseRequestItems",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "OpenPODate",
                table: "GarmentPurchaseRequestItems",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedOpenPOMDBy",
                table: "GarmentPurchaseRequestItems");

            migrationBuilder.DropColumn(
                name: "ApprovedOpenPOMDDate",
                table: "GarmentPurchaseRequestItems");

            migrationBuilder.DropColumn(
                name: "ApprovedOpenPOPurchasingBy",
                table: "GarmentPurchaseRequestItems");

            migrationBuilder.DropColumn(
                name: "ApprovedOpenPOPurchasingDate",
                table: "GarmentPurchaseRequestItems");

            migrationBuilder.DropColumn(
                name: "OpenPOBy",
                table: "GarmentPurchaseRequestItems");

            migrationBuilder.DropColumn(
                name: "OpenPODate",
                table: "GarmentPurchaseRequestItems");
        }
    }
}
