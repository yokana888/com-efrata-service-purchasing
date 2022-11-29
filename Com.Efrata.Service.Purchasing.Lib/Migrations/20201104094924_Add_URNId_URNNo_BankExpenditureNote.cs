using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_URNId_URNNo_BankExpenditureNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "URNId",
                table: "BankExpenditureNoteItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "URNNo",
                table: "BankExpenditureNoteItems",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "URNId",
                table: "BankExpenditureNoteItems");

            migrationBuilder.DropColumn(
                name: "URNNo",
                table: "BankExpenditureNoteItems");
        }
    }
}
