using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentReports;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentReports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentReport
{
    public class RoFeatureControllerTest
    {
        private ROFeatureViewModel viewModel
        {
            get
            {
                return new ROFeatureViewModel
                {
                    Article = "",
                    KodeBarang = "",
                    NamaBarang = "",
                    PO = "",
                    QtyKeluar = 0,
                    QtyTerima = 0,
                    RONo = "",
                    UomKeluar = "",
                    UomMasuk = "",
                    items = new ROItemViewModel
                    {
                        Masuk = new List<RODetailMasukViewModel>
                        {
                            new RODetailMasukViewModel
                            {
                                KodeBarang = "",
                                NamaBarang = "",
                                NoBukti = "",
                                PO = "",
                                Qty = 0,
                                ReceiptDate = new DateTime(),
                                RONo = "",
                                Uom = ""
                            }
                        },
                        Keluar = new List<RODetailViewModel>
                        {
                            new RODetailViewModel
                            {
                                JumlahDO = 0,
                                KodeBarang = "",
                                NamaBarang = "",
                                NoBukti = "",
                                PO ="",
                                Qty = 0,
                                RO = "",
                                RONo = null,
                                TanggalKeluar = new DateTime(),
                                Tipe = "",
                                UnitDONo = "",
                                Uom = "",
                                UomDO = ""
                            }
                        }
                    }
                };
            }
        }

        private Mock<IServiceProvider> GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(IdentityService)))
                .Returns(new IdentityService() { Token = "Token", Username = "Test" });

            serviceProvider
                .Setup(x => x.GetService(typeof(IHttpClientService)))
                .Returns(new HttpClientTestService());

            return serviceProvider;
        }

        private ROFeatureController GetController(Mock<IROFeatureFacade> facadeM)
        {
            var user = new Mock<System.Security.Claims.ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);

            var servicePMock = GetServiceProvider();

            var controller = new ROFeatureController(facadeM.Object, servicePMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                    {
                        User = user.Object
                    }
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";

            return controller;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        [Fact]
        public void Should_Success_Get_RO()
        {
            var mockFacade = new Mock<IROFeatureFacade>();
            mockFacade.Setup(x => x.GetROReport(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Tuple.Create(new List<ROFeatureViewModel> { viewModel }, 25));

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<List<ROFeatureViewModel>>(It.IsAny<List<ROFeatureViewModel>>()))
                .Returns(new List<ROFeatureViewModel> { viewModel });

            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            ROFeatureController controller = new ROFeatureController(mockFacade.Object, GetServiceProvider().Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };

            var response = controller.GetReportRO(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));

        }

        [Fact]
        public void Should_Error_Get_RO()
        {
            var mockFacade = new Mock<IROFeatureFacade>();
            var mockMapper = new Mock<IMapper>();
            ROFeatureController controller = new ROFeatureController(mockFacade.Object, GetServiceProvider().Object);
            var response = controller.GetReportRO(null, 0, 0, null);
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
