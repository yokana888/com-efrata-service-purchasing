using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class AddInternalPurchaseOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InternalPurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    BudgetCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BudgetId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BudgetName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CategoryCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CategoryId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CategoryName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DivisionCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DivisionId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DivisionName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ExpectedDeliveryDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsPosted = table.Column<bool>(type: "bit", nullable: false),
                    IsoNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PONo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PRDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PRId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PRNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalPurchaseOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InternalPurchaseOrderItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    POId = table.Column<long>(type: "bigint", nullable: false),
                    PRDetailId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ProductRemark = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Quantity = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UomId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UomUnit = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalPurchaseOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InternalPurchaseOrderItems_InternalPurchaseOrders_POId",
                        column: x => x.POId,
                        principalTable: "InternalPurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InternalPurchaseOrderItems_POId",
                table: "InternalPurchaseOrderItems",
                column: "POId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InternalPurchaseOrderItems");

            migrationBuilder.DropTable(
                name: "InternalPurchaseOrders");
        }
    }
}
