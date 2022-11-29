using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Edit_Column_Garment_Invoice_Detail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DODetailDOId",
                table: "GarmentInvoiceDetails");

            migrationBuilder.AddColumn<long>(
                name: "DODetailId",
                table: "GarmentInvoiceDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DODetailId",
                table: "GarmentInvoiceDetails");

            migrationBuilder.AddColumn<long>(
                name: "DODetailDOId",
                table: "GarmentInvoiceDetails",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
