using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Change_column_PRDetailId_into_PRItemId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PRDetailId",
                table: "InternalPurchaseOrderItems");

            migrationBuilder.AddColumn<string>(
                name: "PRItemId",
                table: "InternalPurchaseOrderItems",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PRItemId",
                table: "InternalPurchaseOrderItems");

            migrationBuilder.AddColumn<string>(
                name: "PRDetailId",
                table: "InternalPurchaseOrderItems",
                maxLength: 255,
                nullable: true);
        }
    }
}
