using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class GarmentPRItem_ApprovedOpenPOKadiv : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovedOpenPOKadivMdBy",
                table: "GarmentPurchaseRequestItems",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ApprovedOpenPOKadivMdDate",
                table: "GarmentPurchaseRequestItems",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "IsApprovedOpenPOKadivMd",
                table: "GarmentPurchaseRequestItems",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedOpenPOKadivMdBy",
                table: "GarmentPurchaseRequestItems");

            migrationBuilder.DropColumn(
                name: "ApprovedOpenPOKadivMdDate",
                table: "GarmentPurchaseRequestItems");

            migrationBuilder.DropColumn(
                name: "IsApprovedOpenPOKadivMd",
                table: "GarmentPurchaseRequestItems");
        }
    }
}
