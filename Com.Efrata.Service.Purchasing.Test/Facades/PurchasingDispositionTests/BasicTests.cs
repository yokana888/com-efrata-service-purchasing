using Com.Efrata.Service.Purchasing.Lib;
using Com.Efrata.Service.Purchasing.Lib.Facades;
using Com.Efrata.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade;
using Com.Efrata.Service.Purchasing.Lib.Facades.InternalPO;
using Com.Efrata.Service.Purchasing.Lib.Facades.PurchasingDispositionFacades;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.BankExpenditureNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchasingDispositionModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchasingDispositionViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.ExternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.InternalPurchaseOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.PurchaseRequestDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.PurchasingDispositionDataUtils;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Facades.PurchasingDispositionTests
{
    public class BasicTests
    {
        private const string ENTITY = "PurchasingDisposition";

        private const string USERNAME = "Unit Test";
        private IServiceProvider ServiceProvider { get; set; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Concat(sf.GetMethod().Name, "_", ENTITY);
        }

        private PurchasingDbContext _dbContext(string testName)
        {
            DbContextOptionsBuilder<PurchasingDbContext> optionsBuilder = new DbContextOptionsBuilder<PurchasingDbContext>();
            optionsBuilder
                .UseInMemoryDatabase(testName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            PurchasingDbContext dbContext = new PurchasingDbContext(optionsBuilder.Options);

            return dbContext;
        }

        private PurchasingDispositionDataUtil _dataUtil(PurchasingDispositionFacade facade, string testName)
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            PurchaseRequestFacade purchaseRequestFacade = new PurchaseRequestFacade(serviceProvider.Object, _dbContext(testName));
            PurchaseRequestItemDataUtil purchaseRequestItemDataUtil = new PurchaseRequestItemDataUtil();
            PurchaseRequestDataUtil purchaseRequestDataUtil = new PurchaseRequestDataUtil(purchaseRequestItemDataUtil, purchaseRequestFacade);

            InternalPurchaseOrderFacade internalPurchaseOrderFacade = new InternalPurchaseOrderFacade(serviceProvider.Object, _dbContext(testName));
            InternalPurchaseOrderItemDataUtil internalPurchaseOrderItemDataUtil = new InternalPurchaseOrderItemDataUtil();
            InternalPurchaseOrderDataUtil internalPurchaseOrderDataUtil = new InternalPurchaseOrderDataUtil(internalPurchaseOrderItemDataUtil, internalPurchaseOrderFacade, purchaseRequestDataUtil);

            ExternalPurchaseOrderFacade externalPurchaseOrderFacade = new ExternalPurchaseOrderFacade(serviceProvider.Object, _dbContext(testName));
            ExternalPurchaseOrderDetailDataUtil externalPurchaseOrderDetailDataUtil = new ExternalPurchaseOrderDetailDataUtil();
            ExternalPurchaseOrderItemDataUtil externalPurchaseOrderItemDataUtil = new ExternalPurchaseOrderItemDataUtil(externalPurchaseOrderDetailDataUtil);
            ExternalPurchaseOrderDataUtil externalPurchaseOrderDataUtil = new ExternalPurchaseOrderDataUtil(externalPurchaseOrderFacade, internalPurchaseOrderDataUtil, externalPurchaseOrderItemDataUtil);


            return new PurchasingDispositionDataUtil(facade, externalPurchaseOrderDataUtil);
        }

        [Fact]
        public async Task Should_Success_Get_Data()
        {
            PurchasingDispositionFacade facade = new PurchasingDispositionFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.Read();
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_Id()
        {
            PurchasingDispositionFacade facade = new PurchasingDispositionFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadModelById((int)model.Id);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Data_Optimized()
        {
            PurchasingDispositionFacade facade = new PurchasingDispositionFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadOptimized();
            Assert.NotEmpty(Response.Item1);
        }

        [Fact]
        public async Task Should_Success_Create_Data()
        {
            PurchasingDispositionFacade facade = new PurchasingDispositionFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var modelLocalSupplier = await _dataUtil(facade, GetCurrentMethod()).GetNewData();
            var ResponseLocalSupplier = await facade.Create(modelLocalSupplier, USERNAME, 7);
            Assert.NotEqual(0, ResponseLocalSupplier);

            var modelImportSupplier = await _dataUtil(facade, GetCurrentMethod()).GetNewData();
            modelImportSupplier.IncomeTaxBy = "Supplier";
            var ResponseImportSupplier = await facade.Create(modelImportSupplier, USERNAME, 7);
            Assert.NotEqual(0, ResponseImportSupplier);

            var modelDivisionGarment = await _dataUtil(facade, GetCurrentMethod()).GetNewData();
            modelDivisionGarment.DivisionName = "EFRATA";
            var ResponseDivisionGarment= await facade.Create(modelDivisionGarment, USERNAME, 7);
            Assert.NotEqual(0, ResponseLocalSupplier);
        }

        [Fact]
        public async Task Should_Success_Update_Data()
        {
            PurchasingDispositionFacade facade = new PurchasingDispositionFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();

            var datautil = await _dataUtil(facade, GetCurrentMethod()).GetNewData();
            var modelItem = datautil.Items.First();
            var modelDetail = modelItem.Details.First();
            //model.Items.Clear();
            modelItem.EPONo = "test";
            var ResponseAdd1 = await facade.Update((int)model.Id, model, USERNAME);
            Assert.NotEqual(0, ResponseAdd1);

            var dispoItem =
                    new PurchasingDispositionItem
                    {
                        EPOId = modelItem.EPOId,
                        EPONo = modelItem.EPONo,
                        IncomeTaxId = "1",
                        IncomeTaxName = "tax",
                        IncomeTaxRate = 1,
                        UseIncomeTax = true,
                        UseVat = true,
                        Details = new List<PurchasingDispositionDetail>
                       {
                            new PurchasingDispositionDetail
                            {
                                EPODetailId=modelDetail.EPODetailId,
                                
                                DealQuantity=10,
                                PaidQuantity=1000,
                                DealUomId="1",
                                DealUomUnit="test",
                                PaidPrice=1000,
                                PricePerDealUnit=100,
                                PriceTotal=10000,
                                PRId="1",
                                PRNo="test",
                                ProductCode="test",
                                ProductName="test",
                                ProductId="1",
                                UnitName = "test",
                                UnitCode = "test",
                                UnitId = "1",

                            }
                       }
                    };
            var dispoDetail = new PurchasingDispositionDetail
            {
                EPODetailId = modelDetail.EPODetailId,
                
                DealQuantity = 10,
                PaidQuantity = 1000,
                DealUomId = "1",
                DealUomUnit = "test",
                PaidPrice = 1000,
                PricePerDealUnit = 100,
                PriceTotal = 10000,
                PRId = "1",
                PRNo = "test",
                ProductCode = "test",
                ProductName = "test",
                ProductId = "1",

            };

            model.Items.First().Details.Add(dispoDetail);
            var ResponseAddDetail = await facade.Update((int)model.Id, model, USERNAME);
            Assert.NotEqual(0, ResponseAddDetail);

            model.IncomeTaxBy = "Supplier";
            var ResponseAddDetail2 = await facade.Update((int)model.Id, model, USERNAME);
            Assert.NotEqual(0, ResponseAddDetail2);

            model.Items.First().Details.Remove(modelDetail);
            var ResponseAddDetail1 = await facade.Update((int)model.Id, model, USERNAME);
            Assert.NotEqual(0, ResponseAddDetail1);

            model.Items.Add(dispoItem);
            var ResponseAdd = await facade.Update((int)model.Id, model, USERNAME);
            Assert.NotEqual(0, ResponseAdd);


            model.Items.Remove(modelItem);
            var ResponseAdd2 = await facade.Update((int)model.Id, model, USERNAME);
            Assert.NotEqual(0, ResponseAdd2);

        }

        [Fact]
        public async Task Should_Error_Update_Data()
        {
            PurchasingDispositionFacade facade = new PurchasingDispositionFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();

            Exception errorInvalidId = await Assert.ThrowsAsync<Exception>(async () => await facade.Update(0, model, USERNAME));
            Assert.NotNull(errorInvalidId.Message);

            model.Items = null;
            Exception errorNullItems = await Assert.ThrowsAsync<Exception>(async () => await facade.Update((int)model.Id, model, USERNAME));
            Assert.NotNull(errorNullItems.Message);
        }

        [Fact]
        public async Task Should_Success_Delete_Data()
        {
            PurchasingDispositionFacade facade = new PurchasingDispositionFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var Data = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            int Deleted = facade.Delete((int)Data.Id, USERNAME);
            Assert.True(Deleted > 0);
        }

        [Fact]
        public async Task Should_Success_Update_Position()
        {
            PurchasingDispositionFacade facade = new PurchasingDispositionFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var prepData = await _dataUtil(facade, GetCurrentMethod()).GetTestData();

            PurchasingDispositionUpdatePositionPostedViewModel data = new PurchasingDispositionUpdatePositionPostedViewModel()
            {
                Position = Lib.Enums.ExpeditionPosition.CASHIER_DIVISION,
                PurchasingDispositionNoes = new List<string>() { prepData.DispositionNo }
            };
            PurchasingDispositionUpdatePositionPostedViewModel nullModel = new PurchasingDispositionUpdatePositionPostedViewModel();
            Assert.True(nullModel.Validate(null).Count() > 0);
            int updated = await facade.UpdatePosition(data, USERNAME);
            Assert.True(updated > 0);
        }

        [Fact]
        //public async Task Should_Error_Delete_Data()
        //{
        //    PurchasingDispositionFacade facade = new PurchasingDispositionFacade(ServiceProvider, _dbContext(GetCurrentMethod()));

        //    Exception e = await Assert.ThrowsAsync<Exception>(async () => facade.Delete(0, USERNAME));
        //    Assert.NotNull(e.Message);
        //}

        public void Should_Error_Delete_Data()
        {
            PurchasingDispositionFacade facade = new PurchasingDispositionFacade(ServiceProvider, _dbContext(GetCurrentMethod()));

            //Exception e = await Assert.ThrowsAsync<Exception>(async () => facade.Delete(0, USERNAME));
            Exception e = Assert.Throws<Exception>(() => facade.Delete(0, USERNAME));
            Assert.NotNull(e.Message);
        }

        [Fact]
        public void Should_Success_Validate_Data()
        {
            PurchasingDispositionViewModel nullViewModel = new PurchasingDispositionViewModel();
            nullViewModel.Items = new List<PurchasingDispositionItemViewModel>();
            Assert.True(nullViewModel.Validate(null).Count() > 0);

            PurchasingDispositionViewModel viewModel = new PurchasingDispositionViewModel()
            {
                Currency = null,
                Supplier = null,
                Items = new List<PurchasingDispositionItemViewModel>
                {
                    new PurchasingDispositionItemViewModel(),
                    new PurchasingDispositionItemViewModel()
                    {
                        EPONo="testEpo",
                        Details=new List<PurchasingDispositionDetailViewModel>()
                    },
                    new PurchasingDispositionItemViewModel()
                    {
                        EPONo="testEpo",
                        Details=new List<PurchasingDispositionDetailViewModel>
                        {
                            new PurchasingDispositionDetailViewModel()
                            {
                                PaidPrice=0,
                                PaidQuantity=0
                            }
                        }
                    },
                    new PurchasingDispositionItemViewModel()
                    {
                        EPONo="testEpo1",
                        Details=new List<PurchasingDispositionDetailViewModel>()
                    }
                }
            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public async Task Should_Success_Get_Data_Disposition()
        {
            PurchasingDispositionFacade facade = new PurchasingDispositionFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod()).GetNewData();
            var ResponseCreate = await facade.Create(model, USERNAME, 7);
            var epoId = "";
            foreach (var epo in model.Items)
            {
                epoId = epo.EPOId; break;
            }
            var Response = facade.ReadDisposition(null, "{}", epoId);
            Assert.NotEmpty(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Data_By_DispositonNo()
        {
            var facade = new PurchasingDispositionFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            var model = await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.ReadByDisposition(model.DispositionNo);
            Assert.NotNull(Response);
        }

        [Fact]
        public async Task Should_Success_Get_Data_TotalPaidPrice()
        {
            PurchasingDispositionFacade facade = new PurchasingDispositionFacade(ServiceProvider, _dbContext(GetCurrentMethod()));
            await _dataUtil(facade, GetCurrentMethod()).GetTestData();
            var Response = facade.Read();
            var data = Response.Item1.Select(x => new PurchasingDispositionViewModel()
            {
                Active = x.Active,
                Amount = x.Amount,
                Bank = x.Bank,
                Calculation = x.Calculation,
                ConfirmationOrderNo = x.ConfirmationOrderNo,
                CreatedAgent = x.CreatedAgent,
                CreatedBy = x.CreatedBy,
                CreatedUtc = x.CreatedUtc,
                Currency = new Lib.ViewModels.IntegrationViewModel.CurrencyViewModel()
                {
                    code = x.CurrencyCode,
                    _id = x.CurrencyId
                },
                Id = x.Id,
                DispositionNo = x.DispositionNo,
                //Investation = x.Investation,
                //InvoiceNo = x.InvoiceNo,
                IsDeleted = x.IsDeleted,
                LastModifiedAgent = x.LastModifiedAgent,
                LastModifiedBy = x.LastModifiedBy,
                LastModifiedUtc = x.LastModifiedUtc,
                PaymentDueDate = x.PaymentDueDate,
                PaymentMethod = x.PaymentMethod,
                Position = x.Position,
                ProformaNo = x.ProformaNo,
                Remark = x.Remark,
                IncomeTaxValue=1000,
                VatValue=100,
                DPP=100000,
                Category = new Lib.ViewModels.IntegrationViewModel.CategoryViewModel()
                {
                    code = x.CategoryCode,
                    name = x.CategoryName,
                    _id = x.CategoryId
                },
                Supplier = new Lib.ViewModels.IntegrationViewModel.SupplierViewModel()
                {
                    code = x.SupplierCode,
                    name = x.SupplierName,
                    _id = x.SupplierId
                },
                Items = x.Items.Select(y => new PurchasingDispositionItemViewModel()
                {
                    Active = y.Active,
                    CreatedAgent = y.CreatedAgent,
                    CreatedBy = y.CreatedBy,
                    CreatedUtc = y.CreatedUtc,
                    EPOId = y.EPOId,
                    EPONo = y.EPONo,
                    Id = y.Id,
                    IncomeTax = new Lib.ViewModels.IntegrationViewModel.IncomeTaxViewModel()
                    {
                        name = y.IncomeTaxName,
                        rate = y.IncomeTaxRate.ToString(),
                        _id = y.IncomeTaxId
                    },
                    UseVat = y.UseVat,
                    UseIncomeTax = y.UseIncomeTax,
                    LastModifiedUtc = y.LastModifiedUtc,
                    LastModifiedBy = y.LastModifiedBy,
                    LastModifiedAgent = y.LastModifiedAgent,
                    IsDeleted = y.IsDeleted,
                    Details = y.Details.Select(z => new PurchasingDispositionDetailViewModel()
                    {
                        Active = z.Active,
                        IsDeleted = z.IsDeleted,
                        LastModifiedAgent = z.LastModifiedAgent,
                        
                        Unit = new Lib.ViewModels.IntegrationViewModel.UnitViewModel()
                        {
                            name = z.UnitName,
                            code = z.UnitCode,
                            _id = z.UnitId
                        },
                        Product = new Lib.ViewModels.IntegrationViewModel.ProductViewModel()
                        {
                            _id = z.ProductId,
                            code = z.ProductCode,
                            name = z.ProductName
                        },
                        PRNo = z.PRNo,
                        PRId = z.PRId,
                        PriceTotal = z.PriceTotal,
                        PaidQuantity = z.PaidQuantity,
                        CreatedAgent = z.CreatedAgent,
                        CreatedBy = z.CreatedBy,
                        CreatedUtc = z.CreatedUtc,
                        DealQuantity = z.DealQuantity,
                        DealUom = new Lib.ViewModels.IntegrationViewModel.UomViewModel()
                        {
                            unit = z.DealUomUnit,
                            _id = z.DealUomId
                        },
                        Id = z.Id,
                        PricePerDealUnit = z.PricePerDealUnit,
                        PaidPrice = z.PaidPrice,
                        LastModifiedUtc = z.LastModifiedUtc,
                        LastModifiedBy = z.LastModifiedBy
                    }).ToList()
                }).ToList()
            }).ToList();
            var totalPaidPriceResponse = facade.GetTotalPaidPrice(data);
            Assert.NotEmpty(totalPaidPriceResponse);
        }

        [Fact]
        public void Should_Success_Get_Data_DispositionMemoLoader()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new PurchasingDispositionFacade(ServiceProvider, dbContext);

            var Response = facade.GetDispositionMemoLoader(0);
            Assert.Null(Response);

            var purhcasingDisposition = new PurchasingDisposition() { Id = 1, CurrencyCode = "IDR" };
            var purchasingDispositionItem = new PurchasingDispositionItem() { PurchasingDispositionId = 1, UseVat = true, UseIncomeTax = true, EPONo = "1" };
            var unitPaymentOrder = new UnitPaymentOrder() { Id = 1 };
            var unitPaymentOrderItem = new UnitPaymentOrderItem() { Id = 1, UPOId = 1 };
            var unitPaymentOrderDetail = new UnitPaymentOrderDetail() { EPONo = "1", UPOItemId = 1  };

            dbContext.PurchasingDispositions.Add(purhcasingDisposition);
            dbContext.PurchasingDispositionItems.Add(purchasingDispositionItem);
            dbContext.UnitPaymentOrders.Add(unitPaymentOrder);
            dbContext.UnitPaymentOrderItems.Add(unitPaymentOrderItem);
            dbContext.UnitPaymentOrderDetails.Add(unitPaymentOrderDetail);
            dbContext.SaveChanges();

            var Response2 = facade.GetDispositionMemoLoader(1);
            Assert.NotNull(Response2);
        }

        [Fact]
        public void Should_Success_Get_Data_PaymentOrderMemoLoader()
        {
            var dbContext = _dbContext(GetCurrentMethod());
            var facade = new PurchasingDispositionFacade(ServiceProvider, dbContext);

            var bankExpenditureNote = new BankExpenditureNoteModel() { Id = 1, SupplierImport = false, CurrencyCode = "IDR" };
            var bankExpenditureNoteDetail = new BankExpenditureNoteDetailModel() { Id = 1, BankExpenditureNoteId = 1, UnitPaymentOrderNo = "Test" };
            var unitPaymentOrder = new UnitPaymentOrder() { Id = 1, UPONo = "Test", CurrencyCode = "IDR", DivisionId = "1", UseVat = true, UseIncomeTax = true };

            dbContext.BankExpenditureNotes.Add(bankExpenditureNote);
            dbContext.BankExpenditureNoteDetails.Add(bankExpenditureNoteDetail);
            dbContext.UnitPaymentOrders.Add(unitPaymentOrder);
            dbContext.SaveChanges();

            var Response = facade.GetUnitPaymentOrderMemoLoader("Test",1,false,"IDR");
            var Response2 = facade.GetUnitPaymentOrderMemoLoader("",1,false,"");

            Assert.NotNull(Response);
            Assert.NotNull(Response2);
        }
    }
}
