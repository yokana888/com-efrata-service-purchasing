using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentDeliveryOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInvoiceModel;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentDeliveryOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInvoiceViewModels;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentDeliveryOrderDataUtils;
using Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentInvoiceDataUtils;
using Com.Efrata.Service.Purchasing.Test.Helpers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentDeliveryOrderControllers;
using Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentInvoiceControllers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentInvoiceTests
{
	//[Collection("TestServerFixture Collection")]
	public class GarmentInvoicesControllerTest
	{
		////private const string MediaType = "application/json";
		//private readonly string URI = "v1/garment-invoices";
		////private readonly string USERNAME = "dev2";

		//private GarmentInvoiceViewModel ViewModel
		//{
		//	get
		//	{
		//		return new GarmentInvoiceViewModel
		//		{
  //                  UId = null,
  //                  supplier = new SupplierViewModel {
		//				Name = "supplier",
		//			},
		//			items = new List<GarmentInvoiceItemViewModel>
		//			{
		//				new GarmentInvoiceItemViewModel
		//				{
		//					deliveryOrder= new GarmentDeliveryOrderViewModel
		//					{
		//						Id=It.IsAny<int>(),
		//						incomeTax=new IncomeTaxViewModel
		//						{
		//							Id=1,
		//							Name="aa",
		//							Rate=1999
		//						},
		//						useIncomeTax=true,
		//						useVat=false,
		//						docurrency= new CurrencyViewModel
		//						{
		//							Id=1,
		//							Code="code",
		//							Rate=9000
		//						},
		//						supplier= new SupplierViewModel
		//						{
		//							Id=1,
		//							Code="code",
		//							Name="name"
		//						}
		//					}
		//				}
		//			}

		//		};
		//	}
		//}

		//private TestServerFixture TestFixture { get; set; }

		//public GarmentInvoicesControllerTest(TestServerFixture fixture)
		//{
		//	TestFixture = fixture;
		//}
		//private HttpClient Client
		//{
		//	get { return this.TestFixture.Client; }
		//}
		//protected GarmentInvoiceDataUtil DataUtil
		//{
		//	get { return (GarmentInvoiceDataUtil)this.TestFixture.Service.GetService(typeof(GarmentInvoiceDataUtil )); }
		//}
		//protected int GetStatusCode(IActionResult response)
		//{
		//	return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
		//}
		//private GarmentInvoiceController GetController(Mock<IGarmentInvoice> facadeM, Mock<IValidateService> validateM, Mock<IMapper> mapper, Mock<IGarmentDeliveryOrderFacade> facadeDO)
		//{
		//	var user = new Mock<ClaimsPrincipal>();
		//	var claims = new Claim[]
		//	{
		//		new Claim("username", "unittestusername")
		//	};
		//	user.Setup(u => u.Claims).Returns(claims);

		//	var servicePMock = GetServiceProvider();
		//	if (validateM != null)
		//	{
		//		servicePMock
		//			.Setup(x => x.GetService(typeof(IValidateService)))
		//			.Returns(validateM.Object);
		//	}

		//	var controller = new GarmentInvoiceController(servicePMock.Object, mapper.Object, facadeM.Object, facadeDO.Object)
		//	{
		//		ControllerContext = new ControllerContext()
		//		{
		//			HttpContext = new DefaultHttpContext()
		//			{
		//				User = user.Object
		//			}
		//		}
		//	};
		//	controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
		//	controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
		//	controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";

		//	return controller;
		//}
		//private Mock<IServiceProvider> GetServiceProvider()
		//{
		//	var serviceProvider = new Mock<IServiceProvider>();
		//	serviceProvider
		//		.Setup(x => x.GetService(typeof(IdentityService)))
		//		.Returns(new IdentityService() { Token = "Token", Username = "Test" });

		//	serviceProvider
		//		.Setup(x => x.GetService(typeof(IHttpClientService)))
		//		.Returns(new HttpClientTestService());

		//	return serviceProvider;
		//}
		//private GarmentDeliveryOrder DeliveryOrderModel
		//{
		//	get
		//	{
		//		return new GarmentDeliveryOrder
		//		{
		//			Id = 1,
		//			IncomeTaxId=1,
		//			IncomeTaxName="name",
		//			IncomeTaxRate=10000
		//		};
		//	}
		//}
		//[Fact]
		//public async Task Should_Success_Get_All_Data()
		//{
		//	var response = await this.Client.GetAsync(URI);
		//	Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		//	// add error ^_^
		//	var responseError = await this.Client.GetAsync(URI + "?filter={'IsPosted':}");
		//	Assert.Equal(HttpStatusCode.InternalServerError, responseError.StatusCode);
		//}

		//[Fact]
		//public void Should_Success_Get_All_Data_By_User()
		//{
		//	var validateMock = new Mock<IValidateService>();
		//	validateMock.Setup(s => s.Validate(It.IsAny<GarmentInvoiceViewModel>())).Verifiable();

		//	var mockFacade = new Mock<IGarmentInvoice>();

		//	mockFacade.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, It.IsAny<string>()))
		//		.Returns(Tuple.Create(new List<GarmentInvoice>(), 0, new Dictionary<string, string>()));

		//	var mockMapper = new Mock<IMapper>();
		//	mockMapper.Setup(x => x.Map<List<GarmentInvoiceViewModel>>(It.IsAny<List<GarmentInvoice>>()))
		//		.Returns(new List<GarmentInvoiceViewModel> { ViewModel });

		//	var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();

		//	GarmentInvoiceController controller = GetController(mockFacade, validateMock, mockMapper, IPOmockFacade);
		//	var response = controller.GetByUser();
		//	Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
		//}
		//[Fact]
		//public async Task Should_Error_Get_Invalid_Id()
		//{
		//	var response = await this.Client.GetAsync($"{URI}/0");
		//	Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
		//}
		//[Fact]
		//public void Should_Sucscess_Get_Data_By_Id()
		//{
		//	var mockFacade = new Mock<IGarmentInvoice>();
		//	mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
		//		.Returns(Model);

		//	var mockMapper = new Mock<IMapper>();
		//	mockMapper.Setup(x => x.Map<GarmentInvoiceViewModel>(It.IsAny<GarmentInvoice>()))
		//		.Returns(ViewModel);
		//	mockMapper.Setup(x => x.Map<GarmentDeliveryOrderViewModel>(It.IsAny<GarmentDeliveryOrder>()))
		//		.Returns(new GarmentDeliveryOrderViewModel());

		//	var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
		//	IPOmockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
		//		 .Returns(DeliveryOrderModel);
		//	var validateMock = new Mock<IValidateService>();
		//	validateMock.Setup(s => s.Validate(It.IsAny<GarmentInvoiceViewModel>())).Verifiable();


		//	GarmentInvoiceController controller = GetController(mockFacade, validateMock, mockMapper, IPOmockFacade);
		//	var response = controller.Get(It.IsAny<int>());
		//	Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
		//}
		//private GarmentInvoice Model
		//{
		//	get
		//	{
		//		return new GarmentInvoice { };
		//	}
		//}
		//private ServiceValidationExeption GetServiceValidationExeption()
		//{
		//	Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
		//	List<ValidationResult> validationResults = new List<ValidationResult>();

		//	ViewModel.invoiceNo = ViewModel.invoiceNo;
		//	ViewModel.invoiceDate = ViewModel.invoiceDate;
		//	ViewModel.supplier = ViewModel.supplier;
		//	foreach (var item in ViewModel.items)
		//	{
		//		item.deliveryOrder = item.deliveryOrder;
		//		if (item.deliveryOrder != null)
		//		{
		//			item.deliveryOrder.paymentType = "type";
		//			item.deliveryOrder.paymentMethod = "method";
		//		}
		//	}
		//	System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(ViewModel, serviceProvider.Object, null);
		//	return new ServiceValidationExeption(validationContext, validationResults);
		//}
		//[Fact]
		//public async Task Should_Success_Create_Data()
		//{
		//	var validateMock = new Mock<IValidateService>();
		//	validateMock.Setup(s => s.Validate(It.IsAny<GarmentInvoiceViewModel>())).Verifiable();

		//	var mockMapper = new Mock<IMapper>();
		//	mockMapper.Setup(x => x.Map<GarmentInvoice>(It.IsAny<GarmentInvoiceViewModel>()))
		//		.Returns(Model);

		//	var mockFacade = new Mock<IGarmentInvoice>();
		//	mockFacade.Setup(x => x.Create(It.IsAny<GarmentInvoice>(), "unittestusername", 7))
		//	   .ReturnsAsync(1);

		//	var IPOvalidateMock = new Mock<IValidateService>();
		//	IPOvalidateMock.Setup(s => s.Validate(It.IsAny<GarmentDeliveryOrderViewModel>())).Verifiable();

		//	var IPOmockMapper = new Mock<IMapper>();
		//	IPOmockMapper.Setup(x => x.Map<List<GarmentDeliveryOrder>>(It.IsAny<List<GarmentDeliveryOrderViewModel>>()))
		//		.Returns(new List<GarmentDeliveryOrder>());

		//	var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
		//	IPOmockFacade.Setup(x => x.Create(It.IsAny<GarmentDeliveryOrder>(), "unittestusername", 7))
		//	   .ReturnsAsync(1);

		//	var controller = GetController(mockFacade, validateMock, mockMapper, IPOmockFacade);

		//	var response = await controller.Post(this.ViewModel);
		//	Assert.Equal((int)HttpStatusCode.Created, GetStatusCode(response));
		//}

		//[Fact]
		//public async Task Should_Validate_Create_Data()
		//{
		//	var validateMock = new Mock<IValidateService>();
		//	validateMock.Setup(s => s.Validate(It.IsAny<GarmentInvoiceViewModel>())).Throws(GetServiceValidationExeption());

		//	var mockMapper = new Mock<IMapper>();

		//	var mockFacade = new Mock<IGarmentInvoice>();

		//	var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();

		//	var controller = GetController(mockFacade, validateMock, mockMapper, IPOmockFacade);

		//	var response = await controller.Post(this.ViewModel);
		//	Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
		//}

		//[Fact]
		//public async Task Should_Validate_Create_Data_Empty()
		//{
		//	var validateMock = new Mock<IValidateService>();
		//	validateMock.Setup(s => s.Validate(It.IsAny<GarmentInvoiceViewModel>())).Throws(GetServiceValidationExeption());

		//	var mockMapper = new Mock<IMapper>();
		//	mockMapper.Setup(x => x.Map<GarmentInvoice>(It.IsAny<GarmentInvoice>()))
		//		.Returns(Model);

		//	var mockFacade = new Mock<IGarmentInvoice>();
		//	mockFacade.Setup(x => x.Create(It.IsAny<GarmentInvoice>(), "unittestusername", 7))
		//	   .ReturnsAsync(1);

		//	var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();

		//	var controller = GetController(mockFacade, validateMock, mockMapper, IPOmockFacade);

		//	var response = await controller.Post(this.ViewModel);
		//	Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
		//}
		//[Fact]
		//public async Task Should_Error_Create_Data()
		//{
		//	var validateMock = new Mock<IValidateService>();
		//	validateMock.Setup(s => s.Validate(It.IsAny<GarmentInvoiceViewModel>())).Verifiable();

		//	var mockMapper = new Mock<IMapper>();
		//	mockMapper.Setup(x => x.Map<GarmentInvoice>(It.IsAny<GarmentInvoiceViewModel>()))
		//		.Returns(Model);

		//	var mockFacade = new Mock<IGarmentInvoice>();
		//	mockFacade.Setup(x => x.Create(It.IsAny<GarmentInvoice>(), "unittestusername", 7))
		//	   .ReturnsAsync(1);

		//	var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();

		//	GarmentInvoiceController controller = new GarmentInvoiceController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, IPOmockFacade.Object);

		//	var response = await controller.Post(this.ViewModel);
		//	Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
		//}
	 
		//[Fact]
		//public async Task Should_Error_Update_Data()
		//{
		//	var validateMock = new Mock<IValidateService>();
		//	validateMock.Setup(s => s.Validate(It.IsAny<GarmentInvoiceViewModel>())).Verifiable();

		//	var mockMapper = new Mock<IMapper>();
		//	mockMapper.Setup(x => x.Map<GarmentInvoice>(It.IsAny<GarmentInvoiceViewModel>()))
		//		.Returns(Model);

		//	var mockFacade = new Mock<IGarmentInvoice>();
		//	mockFacade.Setup(x => x.Create(It.IsAny<GarmentInvoice>(), "unittestusername", 7))
		//	   .ReturnsAsync(1);

		//	var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();

		//	var controller = new GarmentInvoiceController(GetServiceProvider().Object, mockMapper.Object, mockFacade.Object, IPOmockFacade.Object);

		//	var response = await controller.Put(It.IsAny<int>(), It.IsAny<GarmentInvoiceViewModel>());
		//	Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
		//}

		//[Fact]
		//public void Should_Success_Delete_Data()
		//{
		//	var validateMock = new Mock<IValidateService>();
		//	var mockMapper = new Mock<IMapper>();

		//	var mockFacade = new Mock<IGarmentInvoice>();
		//	mockFacade.Setup(x => x.Delete(It.IsAny<int>(), "unittestusername"))
		//		.Returns(1);

		//	var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();


		//	var controller = GetController(mockFacade, validateMock, mockMapper, IPOmockFacade);

		//	var response = controller.Delete(It.IsAny<int>());
		//	Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
		//}

		//private GarmentInvoiceViewModel ViewModelTax
		//{
		
		//	get
		//	{
		//		return new GarmentInvoiceViewModel
		//		{
		//			invoiceNo = "InvoiceNo",
		//			invoiceDate= DateTimeOffset.Now,
		//			supplier = new SupplierViewModel
		//			{
		//				Import = true,
		//				Id = It.IsAny<int>(),
		//				PIC = "importTest",
		//				Contact = "0987654",
		//				Code = "SupplierImport",
		//				Name = "SupplierImport"
		//			},
		//			incomeTaxId= It.IsAny<int>(),
		//			incomeTaxName="name",
		//			incomeTaxNo="Inc",
		//			incomeTaxDate= DateTimeOffset.Now,
		//			incomeTaxRate=2,
		//			vatNo="vat",
		//			vatDate= DateTimeOffset.Now,
		//			useIncomeTax=true,
		//			useVat=true,
		//			isPayTax=true,
		//			hasInternNote=false,
		//			currency = new CurrencyViewModel
		//			{	Id= It.IsAny<int>(),
		//				Code = "TEST",
		//				Rate = 1,
		//				Symbol = "tst"
		//			},
		//			items = new List<GarmentInvoiceItemViewModel>
		//			{
		//				new GarmentInvoiceItemViewModel
		//				{
		//					deliveryOrder = new GarmentDeliveryOrderViewModel
		//					{
		//						Id =It.IsAny<int>(),
		//						doNo = "1245",
		//						doDate =  DateTimeOffset.Now,
		//						arrivalDate  =  DateTimeOffset.Now,
		//						totalAmount=2000,
		//						paymentType = "type",
		//						paymentMethod = "method",
		//						docurrency = new CurrencyViewModel
		//						{
		//							Id=It.IsAny<int>(),
		//							Code="USD",
		//							Rate=13700
		//						}
		//					},
								
		//					details= new List<GarmentInvoiceDetailViewModel>
		//					{
		//						new GarmentInvoiceDetailViewModel
		//						{
		//							ePOId=It.IsAny<int>(),
		//							ePONo="epono",
		//							pOId=It.IsAny<int>(),
		//							pRItemId=1,
		//							pRNo="prno",
		//							roNo="12343",
		//							product= new GarmentProductViewModel
		//							{
		//								Name="button",
		//								Code="btn"
		//							},
		//							uoms= new UomViewModel
		//							{
		//								Id="1",
		//								Unit="ROLL"
		//							},
		//							doQuantity=40,
		//							pricePerDealUnit=5000,
		//							paymentDueDays=2,
		//							useVat=true,
		//							useIncomeTax=true,
		//							pOSerialNumber="PM132434"
									
		//						}
		//					}
		//				}
		//			}
		//		};
		//	}
		//}

		//[Fact]
		//public void Should_Success_Get_PDF_IncomeTax()
		//{

		//	var validateMock = new Mock<IValidateService>();
		//	validateMock.Setup(s => s.Validate(It.IsAny<GarmentInvoiceViewModel>())).Verifiable();

		//	var mockFacade = new Mock<IGarmentInvoice>();

		//	mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
		//		.Returns(new GarmentInvoice());
		//	var mockMapper = new Mock<IMapper>();
		//	mockMapper.Setup(x => x.Map<GarmentInvoiceViewModel>(It.IsAny<GarmentInvoice>()))
		//		.Returns(ViewModelTax);
						 
		//	var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
		//	IPOmockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
		//	   .Returns(new GarmentDeliveryOrder { DOCurrencyRate = 13700 });

		//	var user = new Mock<ClaimsPrincipal>();
		//	var claims = new Claim[]
		//	{
		//		new Claim("username", "unittestusername")
		//	};
		//	user.Setup(u => u.Claims).Returns(claims);

		//	GarmentInvoiceController controller = GetController(mockFacade, validateMock, mockMapper, IPOmockFacade);
		//	controller.ControllerContext = new ControllerContext()
		//	{
		//		HttpContext = new DefaultHttpContext()
		//		{
		//			User = user.Object
		//		}
		//	};

		//	controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
		//	controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";

		//	var response = controller.GetIncomePDF(It.IsAny<int>());
		//	Assert.NotNull(response.GetType().GetProperty("FileStream"));
		//}

		//[Fact]
		//public void Should_Success_Get_PDF_Vat()
		//{

		//	var validateMock = new Mock<IValidateService>();
		//	validateMock.Setup(s => s.Validate(It.IsAny<GarmentInvoiceViewModel>())).Verifiable();

		//	var mockFacade = new Mock<IGarmentInvoice>();

		//	mockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
		//		.Returns(new GarmentInvoice());
					   
		//	var mockMapper = new Mock<IMapper>();
		//	mockMapper.Setup(x => x.Map<GarmentInvoiceViewModel>(It.IsAny<GarmentInvoice>()))
		//		.Returns(ViewModelTax);

		//	var IPOmockFacade = new Mock<IGarmentDeliveryOrderFacade>();
		//	IPOmockFacade.Setup(x => x.ReadById(It.IsAny<int>()))
		//	   .Returns(new GarmentDeliveryOrder { DOCurrencyRate = 13700 });

		//	var user = new Mock<ClaimsPrincipal>();
		//	var claims = new Claim[]
		//	{
		//		new Claim("username", "unittestusername")
		//	};
		//	user.Setup(u => u.Claims).Returns(claims);

		//	GarmentInvoiceController controller = GetController(mockFacade, validateMock, mockMapper, IPOmockFacade);
		//	controller.ControllerContext = new ControllerContext()
		//	{
		//		HttpContext = new DefaultHttpContext()
		//		{
		//			User = user.Object
		//		}
		//	};

		//	controller.ControllerContext.HttpContext.Request.Headers["Accept"] = "application/pdf";
		//	controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "0";

		//	var response = controller.GetVatPDF(It.IsAny<int>());
		//	Assert.NotNull(response.GetType().GetProperty("FileStream"));
		//}

	}
}
