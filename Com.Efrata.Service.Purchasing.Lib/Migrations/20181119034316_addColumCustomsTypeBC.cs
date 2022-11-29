using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class addColumCustomsTypeBC : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomsType",
                table: "GarmentBeacukais",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CurrencyViewModel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyViewModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DivisionViewModel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DivisionViewModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncomeTaxViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rate = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeTaxViewModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderExternal",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    no = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderExternal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupplierViewModel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Import = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PIC = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierViewModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UomViewModel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UomViewModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnitViewModel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DivisionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitViewModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitViewModel_DivisionViewModel_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "DivisionViewModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GarmentDeliveryOrderViewModel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    _id = table.Column<long>(type: "bigint", nullable: false),
                    arrivalDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    billNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    customsId = table.Column<long>(type: "bigint", nullable: false),
                    doDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    doNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    docurrencyId = table.Column<long>(type: "bigint", nullable: true),
                    incomeTaxId = table.Column<int>(type: "int", nullable: true),
                    isClosed = table.Column<bool>(type: "bit", nullable: false),
                    isCorrection = table.Column<bool>(type: "bit", nullable: false),
                    isCustoms = table.Column<bool>(type: "bit", nullable: false),
                    isInvoice = table.Column<bool>(type: "bit", nullable: false),
                    paymentBill = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    paymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    paymentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    shipmentNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    shipmentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    supplierId = table.Column<long>(type: "bigint", nullable: true),
                    totalAmount = table.Column<double>(type: "float", nullable: false),
                    useIncomeTax = table.Column<bool>(type: "bit", nullable: false),
                    useVat = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentDeliveryOrderViewModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentDeliveryOrderViewModel_CurrencyViewModel_docurrencyId",
                        column: x => x.docurrencyId,
                        principalTable: "CurrencyViewModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GarmentDeliveryOrderViewModel_IncomeTaxViewModel_incomeTaxId",
                        column: x => x.incomeTaxId,
                        principalTable: "IncomeTaxViewModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GarmentDeliveryOrderViewModel_SupplierViewModel_supplierId",
                        column: x => x.supplierId,
                        principalTable: "SupplierViewModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GarmentProductViewModel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Composition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Const = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UOMId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Width = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Yarn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentProductViewModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentProductViewModel_UomViewModel_UOMId",
                        column: x => x.UOMId,
                        principalTable: "UomViewModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GarmentBeacukaiItemViewModel",
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
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    deliveryOrdersId = table.Column<long>(type: "bigint", nullable: true),
                    totalQty = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentBeacukaiItemViewModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentBeacukaiItemViewModel_GarmentDeliveryOrderViewModel_deliveryOrdersId",
                        column: x => x.deliveryOrdersId,
                        principalTable: "GarmentDeliveryOrderViewModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GarmentDeliveryOrderItemViewModel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GarmentDeliveryOrderViewModelId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    _id = table.Column<long>(type: "bigint", nullable: false),
                    currencyId = table.Column<long>(type: "bigint", nullable: true),
                    incomeTaxRate = table.Column<double>(type: "float", nullable: false),
                    paymentDueDays = table.Column<int>(type: "int", nullable: false),
                    purchaseOrderExternalId = table.Column<long>(type: "bigint", nullable: true),
                    useIncomeTax = table.Column<bool>(type: "bit", nullable: false),
                    useVat = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentDeliveryOrderItemViewModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentDeliveryOrderItemViewModel_GarmentDeliveryOrderViewModel_GarmentDeliveryOrderViewModelId",
                        column: x => x.GarmentDeliveryOrderViewModelId,
                        principalTable: "GarmentDeliveryOrderViewModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GarmentDeliveryOrderItemViewModel_CurrencyViewModel_currencyId",
                        column: x => x.currencyId,
                        principalTable: "CurrencyViewModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GarmentDeliveryOrderItemViewModel_PurchaseOrderExternal_purchaseOrderExternalId",
                        column: x => x.purchaseOrderExternalId,
                        principalTable: "PurchaseOrderExternal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GarmentDeliveryOrderFulfillmentViewModel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GarmentDeliveryOrderItemViewModelId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    _id = table.Column<long>(type: "bigint", nullable: false),
                    conversion = table.Column<double>(type: "float", nullable: false),
                    dealQuantity = table.Column<double>(type: "float", nullable: false),
                    doQuantity = table.Column<double>(type: "float", nullable: false),
                    ePOItemId = table.Column<long>(type: "bigint", nullable: false),
                    isSave = table.Column<bool>(type: "bit", nullable: false),
                    pOId = table.Column<int>(type: "int", nullable: false),
                    pOItemId = table.Column<int>(type: "int", nullable: false),
                    pRId = table.Column<long>(type: "bigint", nullable: false),
                    pRItemId = table.Column<long>(type: "bigint", nullable: false),
                    pRNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    poSerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pricePerDealUnit = table.Column<double>(type: "float", nullable: false),
                    pricePerDealUnitCorrection = table.Column<double>(type: "float", nullable: false),
                    priceTotal = table.Column<double>(type: "float", nullable: false),
                    priceTotalCorrection = table.Column<double>(type: "float", nullable: false),
                    productId = table.Column<long>(type: "bigint", nullable: true),
                    purchaseOrderUomId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    quantityCorrection = table.Column<double>(type: "float", nullable: false),
                    rONo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    receiptQuantity = table.Column<double>(type: "float", nullable: false),
                    smallQuantity = table.Column<double>(type: "float", nullable: false),
                    smallUomId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    unitId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentDeliveryOrderFulfillmentViewModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentDeliveryOrderFulfillmentViewModel_GarmentDeliveryOrderItemViewModel_GarmentDeliveryOrderItemViewModelId",
                        column: x => x.GarmentDeliveryOrderItemViewModelId,
                        principalTable: "GarmentDeliveryOrderItemViewModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GarmentDeliveryOrderFulfillmentViewModel_GarmentProductViewModel_productId",
                        column: x => x.productId,
                        principalTable: "GarmentProductViewModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GarmentDeliveryOrderFulfillmentViewModel_UomViewModel_purchaseOrderUomId",
                        column: x => x.purchaseOrderUomId,
                        principalTable: "UomViewModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GarmentDeliveryOrderFulfillmentViewModel_UomViewModel_smallUomId",
                        column: x => x.smallUomId,
                        principalTable: "UomViewModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GarmentDeliveryOrderFulfillmentViewModel_UnitViewModel_unitId",
                        column: x => x.unitId,
                        principalTable: "UnitViewModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentBeacukaiItemViewModel_deliveryOrdersId",
                table: "GarmentBeacukaiItemViewModel",
                column: "deliveryOrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentDeliveryOrderFulfillmentViewModel_GarmentDeliveryOrderItemViewModelId",
                table: "GarmentDeliveryOrderFulfillmentViewModel",
                column: "GarmentDeliveryOrderItemViewModelId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentDeliveryOrderFulfillmentViewModel_productId",
                table: "GarmentDeliveryOrderFulfillmentViewModel",
                column: "productId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentDeliveryOrderFulfillmentViewModel_purchaseOrderUomId",
                table: "GarmentDeliveryOrderFulfillmentViewModel",
                column: "purchaseOrderUomId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentDeliveryOrderFulfillmentViewModel_smallUomId",
                table: "GarmentDeliveryOrderFulfillmentViewModel",
                column: "smallUomId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentDeliveryOrderFulfillmentViewModel_unitId",
                table: "GarmentDeliveryOrderFulfillmentViewModel",
                column: "unitId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentDeliveryOrderItemViewModel_GarmentDeliveryOrderViewModelId",
                table: "GarmentDeliveryOrderItemViewModel",
                column: "GarmentDeliveryOrderViewModelId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentDeliveryOrderItemViewModel_currencyId",
                table: "GarmentDeliveryOrderItemViewModel",
                column: "currencyId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentDeliveryOrderItemViewModel_purchaseOrderExternalId",
                table: "GarmentDeliveryOrderItemViewModel",
                column: "purchaseOrderExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentDeliveryOrderViewModel_docurrencyId",
                table: "GarmentDeliveryOrderViewModel",
                column: "docurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentDeliveryOrderViewModel_incomeTaxId",
                table: "GarmentDeliveryOrderViewModel",
                column: "incomeTaxId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentDeliveryOrderViewModel_supplierId",
                table: "GarmentDeliveryOrderViewModel",
                column: "supplierId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentProductViewModel_UOMId",
                table: "GarmentProductViewModel",
                column: "UOMId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitViewModel_DivisionId",
                table: "UnitViewModel",
                column: "DivisionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarmentBeacukaiItemViewModel");

            migrationBuilder.DropTable(
                name: "GarmentDeliveryOrderFulfillmentViewModel");

            migrationBuilder.DropTable(
                name: "GarmentDeliveryOrderItemViewModel");

            migrationBuilder.DropTable(
                name: "GarmentProductViewModel");

            migrationBuilder.DropTable(
                name: "UnitViewModel");

            migrationBuilder.DropTable(
                name: "GarmentDeliveryOrderViewModel");

            migrationBuilder.DropTable(
                name: "PurchaseOrderExternal");

            migrationBuilder.DropTable(
                name: "UomViewModel");

            migrationBuilder.DropTable(
                name: "DivisionViewModel");

            migrationBuilder.DropTable(
                name: "CurrencyViewModel");

            migrationBuilder.DropTable(
                name: "IncomeTaxViewModel");

            migrationBuilder.DropTable(
                name: "SupplierViewModel");

            migrationBuilder.DropColumn(
                name: "CustomsType",
                table: "GarmentBeacukais");
        }
    }
}
