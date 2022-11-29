using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class cleanDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrencyViewModel",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Rate = table.Column<double>(nullable: false),
                    Symbol = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyViewModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DivisionViewModel",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DivisionViewModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncomeTaxViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Rate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeTaxViewModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderExternal",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    no = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderExternal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupplierViewModel",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Contact = table.Column<string>(nullable: true),
                    Import = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PIC = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierViewModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UomViewModel",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Unit = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UomViewModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnitViewModel",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    DivisionId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
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
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAgent = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModifiedAgent = table.Column<string>(nullable: true),
                    LastModifiedBy = table.Column<string>(nullable: true),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    _id = table.Column<long>(nullable: false),
                    arrivalDate = table.Column<DateTimeOffset>(nullable: true),
                    billNo = table.Column<string>(nullable: true),
                    customsId = table.Column<long>(nullable: false),
                    doDate = table.Column<DateTimeOffset>(nullable: false),
                    doNo = table.Column<string>(nullable: true),
                    docurrencyId = table.Column<long>(nullable: true),
                    incomeTaxId = table.Column<int>(nullable: true),
                    isClosed = table.Column<bool>(nullable: false),
                    isCorrection = table.Column<bool>(nullable: false),
                    isCustoms = table.Column<bool>(nullable: false),
                    isInvoice = table.Column<bool>(nullable: false),
                    paymentBill = table.Column<string>(nullable: true),
                    paymentMethod = table.Column<string>(nullable: true),
                    paymentType = table.Column<string>(nullable: true),
                    remark = table.Column<string>(nullable: true),
                    shipmentNo = table.Column<string>(nullable: true),
                    shipmentType = table.Column<string>(nullable: true),
                    supplierId = table.Column<long>(nullable: true),
                    totalAmount = table.Column<double>(nullable: false),
                    useIncomeTax = table.Column<bool>(nullable: false),
                    useVat = table.Column<bool>(nullable: false)
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
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Composition = table.Column<string>(nullable: true),
                    Const = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ProductType = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    Tags = table.Column<string>(nullable: true),
                    UOMId = table.Column<string>(nullable: true),
                    Width = table.Column<string>(nullable: true),
                    Yarn = table.Column<string>(nullable: true)
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
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    UId = table.Column<string>(maxLength: 255, nullable: true),
                    deliveryOrdersId = table.Column<long>(nullable: true),
                    totalQty = table.Column<double>(nullable: false)
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
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAgent = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    GarmentDeliveryOrderViewModelId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModifiedAgent = table.Column<string>(nullable: true),
                    LastModifiedBy = table.Column<string>(nullable: true),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    _id = table.Column<long>(nullable: false),
                    currencyId = table.Column<long>(nullable: true),
                    incomeTaxRate = table.Column<double>(nullable: false),
                    paymentDueDays = table.Column<int>(nullable: false),
                    purchaseOrderExternalId = table.Column<long>(nullable: true),
                    useIncomeTax = table.Column<bool>(nullable: false),
                    useVat = table.Column<bool>(nullable: false)
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
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAgent = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    GarmentDeliveryOrderItemViewModelId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModifiedAgent = table.Column<string>(nullable: true),
                    LastModifiedBy = table.Column<string>(nullable: true),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    _id = table.Column<long>(nullable: false),
                    conversion = table.Column<double>(nullable: false),
                    dealQuantity = table.Column<double>(nullable: false),
                    doQuantity = table.Column<double>(nullable: false),
                    ePOItemId = table.Column<long>(nullable: false),
                    isSave = table.Column<bool>(nullable: false),
                    pOId = table.Column<int>(nullable: false),
                    pOItemId = table.Column<int>(nullable: false),
                    pRId = table.Column<long>(nullable: false),
                    pRItemId = table.Column<long>(nullable: false),
                    pRNo = table.Column<string>(nullable: true),
                    poSerialNumber = table.Column<string>(nullable: true),
                    pricePerDealUnit = table.Column<double>(nullable: false),
                    pricePerDealUnitCorrection = table.Column<double>(nullable: false),
                    priceTotal = table.Column<double>(nullable: false),
                    priceTotalCorrection = table.Column<double>(nullable: false),
                    productId = table.Column<long>(nullable: true),
                    purchaseOrderUomId = table.Column<string>(nullable: true),
                    quantityCorrection = table.Column<double>(nullable: false),
                    rONo = table.Column<string>(nullable: true),
                    receiptQuantity = table.Column<double>(nullable: false),
                    smallQuantity = table.Column<double>(nullable: false),
                    smallUomId = table.Column<string>(nullable: true),
                    unitId = table.Column<string>(nullable: true)
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
    }
}
