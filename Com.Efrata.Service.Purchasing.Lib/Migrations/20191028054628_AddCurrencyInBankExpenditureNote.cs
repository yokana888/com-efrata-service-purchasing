using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class AddCurrencyInBankExpenditureNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "BankExpenditureNotes",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "BankExpenditureNotes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "CurrencyRate",
                table: "BankExpenditureNotes",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "BankExpenditureNotes");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "BankExpenditureNotes");

            migrationBuilder.DropColumn(
                name: "CurrencyRate",
                table: "BankExpenditureNotes");
        }
    }
}
