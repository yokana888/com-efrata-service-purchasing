using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class DivisionOnPPhExpenditure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DivisionCode",
                table: "PPHBankExpenditureNotes",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DivisionId",
                table: "PPHBankExpenditureNotes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DivisionName",
                table: "PPHBankExpenditureNotes",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DivisionCode",
                table: "PPHBankExpenditureNotes");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "PPHBankExpenditureNotes");

            migrationBuilder.DropColumn(
                name: "DivisionName",
                table: "PPHBankExpenditureNotes");
        }
    }
}
