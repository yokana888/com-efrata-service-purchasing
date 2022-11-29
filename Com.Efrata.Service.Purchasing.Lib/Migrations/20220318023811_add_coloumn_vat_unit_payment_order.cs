using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class add_coloumn_vat_unit_payment_order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VatId",
                table: "UnitPaymentOrders",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "VatRate",
                table: "UnitPaymentOrders",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VatId",
                table: "UnitPaymentOrders");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "UnitPaymentOrders");
        }
    }
}
