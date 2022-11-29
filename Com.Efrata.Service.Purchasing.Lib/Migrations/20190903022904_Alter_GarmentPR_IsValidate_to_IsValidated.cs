using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Alter_GarmentPR_IsValidate_to_IsValidated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsValidate",
                table: "GarmentPurchaseRequests",
                newName: "IsValidated");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsValidated",
                table: "GarmentPurchaseRequests",
                newName: "IsValidate");
        }
    }
}
