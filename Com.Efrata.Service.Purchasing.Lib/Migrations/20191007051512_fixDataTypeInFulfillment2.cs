using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class fixDataTypeInFulfillment2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "InterNoteValue",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true,
                oldClrType: typeof(double));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "InterNoteValue",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);
        }
    }
}
