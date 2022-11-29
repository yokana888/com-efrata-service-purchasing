using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Update_name_column_BalanceStockTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "POItemId",
                table: "BalanceStocks",
                newName: "EPOItemId");

            migrationBuilder.RenameColumn(
                name: "POID",
                table: "BalanceStocks",
                newName: "EPOID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EPOItemId",
                table: "BalanceStocks",
                newName: "POItemId");

            migrationBuilder.RenameColumn(
                name: "EPOID",
                table: "BalanceStocks",
                newName: "POID");
        }
    }
}
