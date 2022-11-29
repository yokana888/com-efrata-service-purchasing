using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class ActualBestCase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ActualNominal",
                table: "BudgetCashflowWorstCaseItems",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "BudgetCashflowWorstCaseItems",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualNominal",
                table: "BudgetCashflowWorstCaseItems");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "BudgetCashflowWorstCaseItems");
        }
    }
}
