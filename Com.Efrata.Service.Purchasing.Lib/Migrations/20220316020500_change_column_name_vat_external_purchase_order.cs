using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class change_column_name_vat_external_purchase_order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Vat",
                table: "GarmentExternalPurchaseOrders",
                newName: "VatRate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VatRate",
                table: "GarmentExternalPurchaseOrders",
                newName: "Vat");
        }
    }
}
