using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_PurchasingDispositions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchasingDispositions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Bank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Calculation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmationOrderNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Investation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentDueDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProformaNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierId = table.Column<long>(type: "bigint", nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchasingDispositions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchasingDispositionItems",
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
                    EPOId = table.Column<long>(type: "bigint", nullable: false),
                    EPONo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IncomeTaxId = table.Column<long>(type: "bigint", nullable: false),
                    IncomeTaxName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncomeTaxRate = table.Column<double>(type: "float", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PurchasingDispositionId = table.Column<long>(type: "bigint", nullable: false),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitId = table.Column<long>(type: "bigint", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UseIncomeTax = table.Column<bool>(type: "bit", nullable: false),
                    UseVat = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchasingDispositionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchasingDispositionItems_PurchasingDispositions_PurchasingDispositionId",
                        column: x => x.PurchasingDispositionId,
                        principalTable: "PurchasingDispositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchasingDispositionDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DealQuantity = table.Column<double>(type: "float", nullable: false),
                    DealUomId = table.Column<int>(type: "int", nullable: false),
                    DealUomUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PRId = table.Column<long>(type: "bigint", nullable: false),
                    PRNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaidPrice = table.Column<double>(type: "float", nullable: false),
                    PaidQuantity = table.Column<double>(type: "float", nullable: false),
                    PricePerDealUnit = table.Column<double>(type: "float", nullable: false),
                    PriceTotal = table.Column<double>(type: "float", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchasingDispositionItemId = table.Column<long>(type: "bigint", nullable: false),
                    PurchasingDispositionItemId1 = table.Column<long>(type: "bigint", nullable: true),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchasingDispositionDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchasingDispositionDetails_PurchasingDispositions_PurchasingDispositionItemId",
                        column: x => x.PurchasingDispositionItemId,
                        principalTable: "PurchasingDispositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchasingDispositionDetails_PurchasingDispositionItems_PurchasingDispositionItemId1",
                        column: x => x.PurchasingDispositionItemId1,
                        principalTable: "PurchasingDispositionItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchasingDispositionDetails_PurchasingDispositionItemId",
                table: "PurchasingDispositionDetails",
                column: "PurchasingDispositionItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchasingDispositionDetails_PurchasingDispositionItemId1",
                table: "PurchasingDispositionDetails",
                column: "PurchasingDispositionItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_PurchasingDispositionItems_PurchasingDispositionId",
                table: "PurchasingDispositionItems",
                column: "PurchasingDispositionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchasingDispositionDetails");

            migrationBuilder.DropTable(
                name: "PurchasingDispositionItems");

            migrationBuilder.DropTable(
                name: "PurchasingDispositions");
        }
    }
}
