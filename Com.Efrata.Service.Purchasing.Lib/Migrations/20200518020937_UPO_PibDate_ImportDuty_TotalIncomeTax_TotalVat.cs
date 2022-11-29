using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class UPO_PibDate_ImportDuty_TotalIncomeTax_TotalVat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ImportDuty",
                table: "UnitPaymentOrders",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PibDate",
                table: "UnitPaymentOrders",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<double>(
                name: "TotalIncomeTaxAmount",
                table: "UnitPaymentOrders",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalVatAmount",
                table: "UnitPaymentOrders",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportDuty",
                table: "UnitPaymentOrders");

            migrationBuilder.DropColumn(
                name: "PibDate",
                table: "UnitPaymentOrders");

            migrationBuilder.DropColumn(
                name: "TotalIncomeTaxAmount",
                table: "UnitPaymentOrders");

            migrationBuilder.DropColumn(
                name: "TotalVatAmount",
                table: "UnitPaymentOrders");
        }
    }
}
