using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class GarmentPR_Validation_MD : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ValidatedBy",
                table: "GarmentPurchaseRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsValidatedMD1",
                table: "GarmentPurchaseRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsValidatedMD2",
                table: "GarmentPurchaseRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ValidatedMD1By",
                table: "GarmentPurchaseRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidatedMD1Date",
                table: "GarmentPurchaseRequests",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "ValidatedMD2By",
                table: "GarmentPurchaseRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidatedMD2Date",
                table: "GarmentPurchaseRequests",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsValidatedMD1",
                table: "GarmentPurchaseRequests");

            migrationBuilder.DropColumn(
                name: "IsValidatedMD2",
                table: "GarmentPurchaseRequests");

            migrationBuilder.DropColumn(
                name: "ValidatedMD1By",
                table: "GarmentPurchaseRequests");

            migrationBuilder.DropColumn(
                name: "ValidatedMD1Date",
                table: "GarmentPurchaseRequests");

            migrationBuilder.DropColumn(
                name: "ValidatedMD2By",
                table: "GarmentPurchaseRequests");

            migrationBuilder.DropColumn(
                name: "ValidatedMD2Date",
                table: "GarmentPurchaseRequests");

            migrationBuilder.AlterColumn<string>(
                name: "ValidatedBy",
                table: "GarmentPurchaseRequests",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
