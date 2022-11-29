using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_PaymentCorrection_PurchasingDispositions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PaymentCorrection",
                table: "PurchasingDispositions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentCorrection",
                table: "PurchasingDispositions");
        }
    }
}
