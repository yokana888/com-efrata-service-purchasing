using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Modify_Column_IncomeTaxCorrectionDate_on_UnitPaymentCorrectionNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncomeTaxCorrectionName",
                table: "UnitPaymentCorrectionNotes");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "IncomeTaxCorrectionDate",
                table: "UnitPaymentCorrectionNotes",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncomeTaxCorrectionDate",
                table: "UnitPaymentCorrectionNotes");

            migrationBuilder.AddColumn<string>(
                name: "IncomeTaxCorrectionName",
                table: "UnitPaymentCorrectionNotes",
                nullable: true);
        }
    }
}
