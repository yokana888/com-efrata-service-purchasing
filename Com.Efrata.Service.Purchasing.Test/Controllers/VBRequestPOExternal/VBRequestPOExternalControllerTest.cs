using Com.Efrata.Service.Purchasing.Lib.Facades.VBRequestPOExternal;
using Com.Efrata.Service.Purchasing.Lib.Models.ExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitPaymentOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.UnitReceiptNoteModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.VBRequestPOExternal
{
    public class VBRequestPOExternalControllerTest
    {


        Mock<IServiceProvider> GetServiceProvider()
        {
            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
              .Setup(s => s.GetService(typeof(IdentityService)))
              .Returns(new IdentityService() { TimezoneOffset = 1, Token = "token", Username = "username" });

            return serviceProviderMock;
        }

        protected VBRequestPOExternalController GetController(Mock<IServiceProvider> serviceProvider, Mock<IVBRequestPOExternalService> service)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            VBRequestPOExternalController controller = new VBRequestPOExternalController(service.Object, serviceProvider.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");

            return controller;
        }

        POExternalDto pOExternalDto
        {

            get
            {
                var externalPurchaseOrder = new ExternalPurchaseOrder()
                {
                    DivisionCode = "DivisionCode",
                    CurrencyCode = "IDR",
                    DivisionName= "DivisionName",
                    IsCanceled=false,
                    IsClosed=false,
                    IsPosted=true,
                    IsCreateOnVBRequest=false,
                    UseVat=true,
                    Items =new List<ExternalPurchaseOrderItem>()
                    {
                        new ExternalPurchaseOrderItem()
                        {
                            Details=new List<ExternalPurchaseOrderDetail>()
                            {
                                new ExternalPurchaseOrderDetail()
                                {
                                    ProductName="ProductName",
                                    IncludePpn=true
                                }
                            }
                        }
                    }
                };
                return new POExternalDto(externalPurchaseOrder);
            }
        }

        SPBDto sPBDto
        {
            get
            {
                GarmentInternNote element = new GarmentInternNote
                {
                    CurrencyCode="IDR",
                    INDate=DateTimeOffset.Now,
                    CurrencyRate=1,
                    IsCreatedVB=false,
                    SupplierCode= "SupplierCode",
                    SupplierName= "SupplierName",
                    Remark= "Remark",
                    Items=new List<GarmentInternNoteItem>()
                    {
                        new GarmentInternNoteItem()
                        {
                            Details =new List<GarmentInternNoteDetail>()
                            {
                                new GarmentInternNoteDetail()
                                {
                                    PaymentType="PaymentType"
                                }
                            }
                        }
                    }
                };

                GarmentInvoice invoice = new GarmentInvoice()
                {
                    Items=new List<GarmentInvoiceItem>()
                    {
                        new GarmentInvoiceItem()
                        {
                            Details =new List<GarmentInvoiceDetail>()
                            {
                                new GarmentInvoiceDetail()
                            }
                        }
                    }
                };
                return new SPBDto(element, new List<GarmentInvoice>() { invoice });
            }
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        [Fact]
        public void Get_Return_OK()
        {
            //Setup
            Mock<IVBRequestPOExternalService> serviceMock = new Mock<IVBRequestPOExternalService>();
            serviceMock.Setup(s => s.ReadPOExternal(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new List<POExternalDto>() { pOExternalDto });

            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            serviceProviderMock.Setup(s => s.GetService(typeof(IVBRequestPOExternalService))).Returns(serviceMock.Object);

            //Act
            VBRequestPOExternalController controller = GetController(serviceProviderMock, serviceMock);
            var response =controller.Get(null, null, null);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public void Get_Return_InternalServerError()
        {
            //Setup
            Mock<IVBRequestPOExternalService> serviceMock = new Mock<IVBRequestPOExternalService>();
            serviceMock.Setup(s => s.ReadPOExternal(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());

            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            serviceProviderMock.Setup(s => s.GetService(typeof(IVBRequestPOExternalService))).Returns(serviceMock.Object);

            //Act
            VBRequestPOExternalController controller = GetController(serviceProviderMock, serviceMock);
            var response = controller.Get(null, null, null);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void GetSPB_Return_OK()
        {
            //Setup
            Mock<IVBRequestPOExternalService> serviceMock = new Mock<IVBRequestPOExternalService>();
            serviceMock.Setup(s => s.ReadSPB(It.IsAny<string>(),  It.IsAny<string>(), It.IsAny<List<long>>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new List<SPBDto>() { sPBDto });

            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            serviceProviderMock.Setup(s => s.GetService(typeof(IVBRequestPOExternalService))).Returns(serviceMock.Object);

            //Act
            VBRequestPOExternalController controller = GetController(serviceProviderMock, serviceMock); 
            var response = controller.GetSPB(null, null, "[1, 2, 3, 4]", null, "UMUM");

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public void GetSPB_Return_InternalServerError()
        {
            //Setup
            Mock<IVBRequestPOExternalService> serviceMock = new Mock<IVBRequestPOExternalService>();
            serviceMock.Setup(s => s.ReadSPB(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<long>>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());

            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            serviceProviderMock.Setup(s => s.GetService(typeof(IVBRequestPOExternalService))).Returns(serviceMock.Object);

            //Act
            VBRequestPOExternalController controller = GetController(serviceProviderMock, serviceMock);
            var response = controller.GetSPB(null, null, null, null, "UMUM");

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void UpdateVBCreatedFlag_Return_OK()
        {
            //Setup
            Mock<IVBRequestPOExternalService> serviceMock = new Mock<IVBRequestPOExternalService>();
            serviceMock.Setup(s => s.UpdateSPB(It.IsAny<string>(), It.IsAny<int>())).Returns( 1);

            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            serviceProviderMock.Setup(s => s.GetService(typeof(IVBRequestPOExternalService))).Returns(serviceMock.Object);

            //Act
            VBRequestPOExternalController controller = GetController(serviceProviderMock, serviceMock);
            var response = controller.UpdateVBCreatedFlag(null, 1);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public void UpdateVBCreatedFlag_Return_InternalServerError()
        {
            //Setup
            Mock<IVBRequestPOExternalService> serviceMock = new Mock<IVBRequestPOExternalService>();
            serviceMock.Setup(s => s.UpdateSPB(It.IsAny<string>(), It.IsAny<int>())).Throws(new Exception());

            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            serviceProviderMock.Setup(s => s.GetService(typeof(IVBRequestPOExternalService))).Returns(serviceMock.Object);

            //Act
            VBRequestPOExternalController controller = GetController(serviceProviderMock, serviceMock);
            var response = controller.UpdateVBCreatedFlag(null, 1);

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }


        [Fact]
        public async Task AutoJournalEPO_Return_OK()
        {
            //Setup
            Mock<IVBRequestPOExternalService> serviceMock = new Mock<IVBRequestPOExternalService>();
            serviceMock.Setup(s => s.AutoJournalVBRequest(It.IsAny<VBFormDto>())).ReturnsAsync(1);

            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            serviceProviderMock.Setup(s => s.GetService(typeof(IVBRequestPOExternalService))).Returns(serviceMock.Object);

            //Act
            VBRequestPOExternalController controller = GetController(serviceProviderMock, serviceMock);
            var response =await controller.AutoJournalEPO(new VBFormDto());

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public async Task AutoJournalEPO_Return_InternalServerError()
        {
            //Setup
            Mock<IVBRequestPOExternalService> serviceMock = new Mock<IVBRequestPOExternalService>();
            serviceMock.Setup(s => s.AutoJournalVBRequest(It.IsAny<VBFormDto>())).Throws(new Exception());

            Mock<IServiceProvider> serviceProviderMock = GetServiceProvider();
            serviceProviderMock.Setup(s => s.GetService(typeof(IVBRequestPOExternalService))).Returns(serviceMock.Object);

            //Act
            VBRequestPOExternalController controller = GetController(serviceProviderMock, serviceMock);
            var response = await controller.AutoJournalEPO(new VBFormDto());

            //Assert
            int statusCode = this.GetStatusCode(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void Should_Success_Construct_SPBDto()
        {
            var upoDetails = new List<UnitPaymentOrderDetail>()
            {
                new UnitPaymentOrderDetail()
                {
                    Id=1,
                    UPOItemId=1,
                    URNItemId=1,
                }
            };
            var upoItems = new List<UnitPaymentOrderItem>()
            {
                new UnitPaymentOrderItem()
                {
                    Id=1,
                    UPOId=1,
                    URNId=1,
                    Details=upoDetails

                }
            };
            var upo = new UnitPaymentOrder()
            {
                Id = 1,
                Items = upoItems,


            };
            var urnItems = new List<UnitReceiptNoteItem>()
            {
                new UnitReceiptNoteItem()
                {
                    Id=1,
                    URNId=1,

                }
            };
            var urns = new List<UnitReceiptNote>()
            {
                new UnitReceiptNote()
                {
                    Id=1,
                    Items=urnItems

                }
            };
            var dto = new SPBDto(upo, upoDetails, upoItems, urnItems, urns);
            Assert.NotNull(dto);
        }

        [Fact]
        public void Should_QuantityCorrection_Greater_Than_Zero_SPBDto()
        {
            var upoDetails = new List<UnitPaymentOrderDetail>()
            {
                new UnitPaymentOrderDetail()
                {
                    Id=1,
                    UPOItemId=1,
                    QuantityCorrection=1,
                    URNItemId=1,
                }
            };
            var upoItems = new List<UnitPaymentOrderItem>()
            {
                new UnitPaymentOrderItem()
                {
                    Id=1,
                    UPOId=1,
                    URNId=1,
                    Details=upoDetails

                }
            };
            var upo = new UnitPaymentOrder()
            {
                Id = 1,
                Items = upoItems,
                
                
            };
            var urnItems = new List<UnitReceiptNoteItem>()
            {
                new UnitReceiptNoteItem()
                {
                    Id=1,
                    URNId=1,
                    
                }
            };
            var urns = new List<UnitReceiptNote>()
            {
                new UnitReceiptNote()
                {
                    Id=1,
                    Items=urnItems
                    
                }
            };
            var dto = new SPBDto(upo, upoDetails, upoItems, urnItems, urns);
            Assert.NotNull(dto);
        }
    
    }
}
