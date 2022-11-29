using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_Table_BalanceStock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BalanceStocks",
                columns: table => new
                {
                    BalanceStockId = table.Column<string>(type: "varchar(30)", nullable: false),
                    ArticleNo = table.Column<string>(type: "varchar(50)", nullable: true),
                    ClosePrice = table.Column<decimal>(type: "Money", nullable: true),
                    CloseStock = table.Column<double>(nullable: true),
                    CreateDate = table.Column<DateTime>(type: "Datetime", nullable: true),
                    CreditPrice = table.Column<decimal>(type: "Money", nullable: true),
                    CreditStock = table.Column<double>(nullable: true),
                    DebitPrice = table.Column<decimal>(type: "Money", nullable: true),
                    DebitStock = table.Column<double>(nullable: true),
                    OpenPrice = table.Column<decimal>(type: "Money", nullable: true),
                    OpenStock = table.Column<double>(nullable: true),
                    POID = table.Column<string>(type: "varchar(100)", nullable: true),
                    POItemId = table.Column<int>(nullable: true),
                    PeriodeMonth = table.Column<string>(type: "varchar(50)", nullable: true),
                    PeriodeYear = table.Column<string>(type: "varchar(10)", nullable: true),
                    RO = table.Column<string>(type: "varchar(50)", nullable: true),
                    SmallestUnitQty = table.Column<string>(type: "varchar(50)", nullable: true),
                    StockId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceStocks", x => x.BalanceStockId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BalanceStocks");
        }
    }
}
