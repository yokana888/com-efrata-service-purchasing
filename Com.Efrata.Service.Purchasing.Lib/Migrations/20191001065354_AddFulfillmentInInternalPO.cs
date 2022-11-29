using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class AddFulfillmentInInternalPO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InternalPurchaseOrderFulfillments",
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
                    DeliveryOrderDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeliveryOrderDeliveredQuantity = table.Column<double>(type: "float", nullable: false),
                    DeliveryOrderDetailId = table.Column<long>(type: "bigint", nullable: false),
                    DeliveryOrderId = table.Column<long>(type: "bigint", nullable: false),
                    DeliveryOrderItemId = table.Column<long>(type: "bigint", nullable: false),
                    DeliveryOrderNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    InterNoteDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    InterNoteDueDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    InterNoteNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    InterNoteValue = table.Column<double>(type: "float", nullable: false),
                    InvoiceDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    InvoiceNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    POItemId = table.Column<long>(type: "bigint", nullable: false),
                    SupplierDODate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UnitPaymentOrderDetailId = table.Column<long>(type: "bigint", nullable: false),
                    UnitPaymentOrderId = table.Column<long>(type: "bigint", nullable: false),
                    UnitPaymentOrderItemId = table.Column<long>(type: "bigint", nullable: false),
                    UnitReceiptNoteDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UnitReceiptNoteDeliveredQuantity = table.Column<double>(type: "float", nullable: false),
                    UnitReceiptNoteId = table.Column<long>(type: "bigint", nullable: false),
                    UnitReceiptNoteItemId = table.Column<long>(type: "bigint", nullable: false),
                    UnitReceiptNoteNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitReceiptNoteUom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitReceiptNoteUomId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalPurchaseOrderFulfillments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InternalPurchaseOrderFulfillments_InternalPurchaseOrderItems_POItemId",
                        column: x => x.POItemId,
                        principalTable: "InternalPurchaseOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InternalPurchaseOrderCorrections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CorrectionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CorrectionNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrectionPriceTotal = table.Column<double>(type: "float", nullable: false),
                    CorrectionQuantity = table.Column<double>(type: "float", nullable: false),
                    CorrectionRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    POFulfillmentId = table.Column<long>(type: "bigint", nullable: false),
                    UnitPaymentCorrectionId = table.Column<long>(type: "bigint", nullable: false),
                    UnitPaymentCorrectionItemId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalPurchaseOrderCorrections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InternalPurchaseOrderCorrections_InternalPurchaseOrderFulfillments_POFulfillmentId",
                        column: x => x.POFulfillmentId,
                        principalTable: "InternalPurchaseOrderFulfillments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InternalPurchaseOrderCorrections_POFulfillmentId",
                table: "InternalPurchaseOrderCorrections",
                column: "POFulfillmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalPurchaseOrderFulfillments_POItemId",
                table: "InternalPurchaseOrderFulfillments",
                column: "POItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InternalPurchaseOrderCorrections");

            migrationBuilder.DropTable(
                name: "InternalPurchaseOrderFulfillments");
        }
    }
}
