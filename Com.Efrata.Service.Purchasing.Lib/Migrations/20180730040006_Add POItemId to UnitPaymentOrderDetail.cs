using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class AddPOItemIdtoUnitPaymentOrderDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EPODetailId",
                table: "UnitPaymentOrderDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "POItemId",
                table: "UnitPaymentOrderDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EPODetailId",
                table: "UnitPaymentOrderDetails");

            migrationBuilder.DropColumn(
                name: "POItemId",
                table: "UnitPaymentOrderDetails");
        }
    }
}
