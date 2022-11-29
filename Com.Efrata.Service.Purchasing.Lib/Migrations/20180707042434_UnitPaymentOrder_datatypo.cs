using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class UnitPaymentOrder_datatypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "URNDetailId",
                table: "UnitPaymentOrderDetails");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPaid",
                table: "UnitPaymentOrders",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCorrection",
                table: "UnitPaymentOrders",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "URNItemId",
                table: "UnitPaymentOrderDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "URNItemId",
                table: "UnitPaymentOrderDetails");

            migrationBuilder.AlterColumn<string>(
                name: "IsPaid",
                table: "UnitPaymentOrders",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IsCorrection",
                table: "UnitPaymentOrders",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<long>(
                name: "URNDetailId",
                table: "UnitPaymentOrderDetails",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
