using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentExternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentInternalPurchaseOrderModel;
using Com.Efrata.Service.Purchasing.Lib.PDFTemplates;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentExternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentInternalPurchaseOrderViewModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentExternalPurchaseOrderControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment-external-purchase-orders")]
    [Authorize]
    public class GarmentExternalPurchaseOrderController : Controller
    {
        private string ApiVersion = "1.0.0";
        public readonly IServiceProvider serviceProvider;
        private readonly IMapper mapper;
        private readonly IGarmentExternalPurchaseOrderFacade facade;
        private readonly IGarmentInternalPurchaseOrderFacade IPOfacade;
        private readonly IdentityService identityService;

        public GarmentExternalPurchaseOrderController(IServiceProvider serviceProvider, IMapper mapper, IGarmentExternalPurchaseOrderFacade facade, IGarmentInternalPurchaseOrderFacade IPOfacade)
        {
            this.serviceProvider = serviceProvider;
            this.mapper = mapper;
            this.facade = facade;
            this.IPOfacade = IPOfacade;
            this.identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet("by-user")]
        public IActionResult GetByUser(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                string filterUser = string.Concat("'CreatedBy':'", identityService.Username, "'");
                if (filter == null || !(filter.Trim().StartsWith("{") && filter.Trim().EndsWith("}")) || filter.Replace(" ", "").Equals("{}"))
                {
                    filter = string.Concat("{", filterUser, "}");
                }
                else
                {
                    filter = filter.Replace("}", string.Concat(", ", filterUser, "}"));
                }

                return Get(page, size, order, keyword, filter);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet]
        public IActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var Data = facade.Read(page, size, order, keyword, filter);

                var viewModel = mapper.Map<List<GarmentExternalPurchaseOrderViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(
                    viewModel.AsQueryable().Select(s => new
                    {
                        s.Id,
                        s.Category,
                        s.OrderDate,
                        s.EPONo,
                        s.Supplier,
                        s.IsOverBudget,
                        s.IsApproved,
                        Items = s.Items.Select(i => new
                        {
                            i.PRNo
                        }),
                        s.CreatedBy,
                        s.IsPosted,
                        s.LastModifiedUtc
                    }).ToList()
                );

                var info = new Dictionary<string, object>
                    {
                        { "count", listData.Count },
                        { "total", Data.Item2 },
                        { "order", Data.Item3 },
                        { "page", page },
                        { "size", size }
                    };

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(listData, info);
                return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var indexAcceptPdf = Request.Headers["Accept"].ToList().IndexOf("application/pdf");

                GarmentExternalPurchaseOrder model = facade.ReadById(id);
                GarmentExternalPurchaseOrderViewModel viewModel = mapper.Map<GarmentExternalPurchaseOrderViewModel>(model);
                if (viewModel == null)
                {
                    throw new Exception("Invalid Id");
                }
                if (indexAcceptPdf < 0)
                {
                    viewModel.IsUnpost = facade.GetIsUnpost(id);
                    return Ok(new
                    {
                        apiVersion = ApiVersion,
                        statusCode = General.OK_STATUS_CODE,
                        message = General.OK_MESSAGE,
                        data = viewModel,
                    });
                }
                else
                {
                    int clientTimeZoneOffset = int.Parse(Request.Headers["x-timezone-offset"].First());
                    SupplierViewModel supplier = facade.GetSupplier(viewModel.Supplier.Id);

                    viewModel.Supplier = supplier==null? viewModel.Supplier : supplier;
                    foreach (var item in viewModel.Items)
                    {
                        GarmentInternalPurchaseOrder ipo = IPOfacade.ReadById(item.POId);
                        item.ShipmentDate = ipo==null ? item.ShipmentDate : ipo.ShipmentDate;
                        GarmentProductViewModel product = facade.GetProduct(item.Product.Id);
                        item.Product = product == null ? item.Product : product;
                    }

                    GarmentExternalPurchaseOrderPDFTemplate PdfTemplateLocal = new GarmentExternalPurchaseOrderPDFTemplate();
                    MemoryStream stream = PdfTemplateLocal.GeneratePdfTemplate(viewModel, clientTimeZoneOffset);

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = $"{viewModel.EPONo}.pdf"
                    };
                    
                }
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]GarmentExternalPurchaseOrderViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                validateService.Validate(ViewModel);

                var model = mapper.Map<GarmentExternalPurchaseOrder>(ViewModel);

                await facade.Create(model, identityService.Username);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
                return Created(String.Concat(Request.Path, "/", 0), Result);
            }
            catch (ServiceValidationExeption e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
                    .Fail(e);
                return BadRequest(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]GarmentExternalPurchaseOrderViewModel ViewModel)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                IValidateService validateService = (IValidateService)serviceProvider.GetService(typeof(IValidateService));

                validateService.Validate(ViewModel);

                var model = mapper.Map<GarmentExternalPurchaseOrder>(ViewModel);

                await facade.Update(id, model, identityService.Username);

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.CREATED_STATUS_CODE, General.OK_MESSAGE)
                    .Ok();
                return Created(String.Concat(Request.Path, "/", 0), Result);
            }
            catch (ServiceValidationExeption e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.BAD_REQUEST_STATUS_CODE, General.BAD_REQUEST_MESSAGE)
                    .Fail(e);
                return BadRequest(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute]int id)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            
                facade.Delete(id, identityService.Username);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }

        [HttpPost("post")]
        public IActionResult EPOPost([FromBody]List<GarmentExternalPurchaseOrderViewModel> ListExternalPurchaseOrderViewModel)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
            try
            {
                facade.EPOPost(
                    ListExternalPurchaseOrderViewModel.Select(vm => mapper.Map<GarmentExternalPurchaseOrder>(vm)).ToList(), identityService.Username
                );

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }

        [HttpPut("unpost/{id}")]
        public IActionResult EPOUnpost([FromRoute]int id)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            try
            {
                facade.EPOUnpost(id, identityService.Username);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }

        [HttpPut("cancel/{id}")]
        public IActionResult EPOCancel([FromRoute]int id)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            try
            {
                facade.EPOCancel(id, identityService.Username);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }

        [HttpPut("close/{id}")]
        public IActionResult EPOClose([FromRoute]int id)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            try
            {
                facade.EPOClose(id, identityService.Username);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }

        [HttpPost("approve")]
        public IActionResult EPOApprove([FromBody]List<GarmentExternalPurchaseOrderViewModel> ListExternalPurchaseOrderViewModel)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
            try
            {
                facade.EPOApprove(
                    ListExternalPurchaseOrderViewModel.Select(vm => mapper.Map<GarmentExternalPurchaseOrder>(vm)).ToList(), identityService.Username
                );

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }
        [HttpGet("by-supplier")]
        public IActionResult BySupplier(string keyword = null, string Filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var Data = facade.ReadBySupplier(keyword, Filter);

                var newData = mapper.Map<List<GarmentExternalPurchaseOrderViewModel>>(Data);

                List<object> listData = new List<object>();
                listData.AddRange(
                    newData.AsQueryable().Select(s => new
                    {
                        s.Id,
                        s.EPONo,
                        s.OrderDate,
                        s.DeliveryDate,
                        s.Supplier,
                        s.PaymentType,
                        s.PaymentMethod,
                        s.PaymentDueDays,
                        s.Currency,
                        s.IncomeTax,
                        s.IsIncomeTax,
                        s.IsUseVat,
                        s.Vat,
                        s.Category,
                        s.Remark,
                        s.IsPosted,
                        s.IsOverBudget,
                        s.IsApproved,
                        s.IsCanceled,
                        s.IsClosed,
                        s.LastModifiedUtc,
                        s.IsPayVAT,
                        s.IsPayIncomeTax,
                        Items = s.Items.Select(i => new
                        {
                            i.Id,
                            i.PRNo,
                            i.PRId,
                            i.PONo,
                            i.POId,
                            i.RONo,
                            i.PO_SerialNumber,
                            i.Product,
                            i.DealQuantity,
                            DOQuantity = (i.DealQuantity - i.DOQuantity),
                            i.DealUom,
                            i.Conversion,
                            i.SmallQuantity,
                            i.SmallUom,
                            i.PricePerDealUnit,
                            i.Remark
                        })
                    }).ToList()
                );

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = listData,
                    info = new Dictionary<string, object>
                {
                    { "count", listData.Count },
                },
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("by-ro")]
        public IActionResult ByRO(string keyword = null, string Filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var Data = facade.ReadItemByRO(keyword, Filter);

                var newData = mapper.Map<List<GarmentExternalPurchaseOrderItemViewModel>>(Data);

                List<object> listData = new List<object>();
                listData.AddRange(
                    newData.AsQueryable().Select(i => new
                    {
                        i.Id,
                        i.RONo,
                        i.PO_SerialNumber,
                        i.Product,
                        i.DealQuantity,
                        i.DealUom,
                        i.GarmentEPOId,
                        i.Article,
                        i.CreatedUtc
                    }).ToList()
                );

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = listData,
                    info = new Dictionary<string, object>
                {
                    { "count", listData.Count },
                },
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
        [HttpGet("by-epo-no-str")]
        public IActionResult ByEPONOCurrencyCode(string keyword = null, string Filter = "{}", int supplierId = 0, string currencyCode = null, string paymentType = null, string category = null, int page = 1, int size = 10)
        {

            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var Data = facade.ReadItemByEPONoSimply(keyword, supplierId, currencyCode, paymentType, category);

                var viewModel = mapper.Map<List<GarmentExternalPurchaseOrderViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(
                    viewModel.AsQueryable().Select(s => new
                    {
                        s.Id,
                        s.Category,
                        s.OrderDate,
                        s.EPONo,
                        s.Supplier,
                        s.IsOverBudget,
                        s.IsApproved,
                        Items = s.Items.Select(i => new
                        {
                            i.PRNo,
                        }),
                        s.CreatedBy,
                        s.IsPosted,
                        s.LastModifiedUtc
                    }).ToList()
                );

                var info = new Dictionary<string, object>
                    {
                        { "count", listData.Count },
                        { "total", Data.Item2 },
                        { "order", Data.Item3 },
                        { "page", page },
                        { "size", size }
                    };

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(listData, info);
                return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
        [HttpGet("by-epo-no")]
        public IActionResult ByEPONO(string keyword = null, string Filter = "{}",int supplierId = 0, int currencyId=0,int page=1, int size=10)
        {
            //try
            //{
            //    identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            //    var Data = facade.ReadItemByEPONoSimply(keyword, Filter,supplierId,currencyId);

            //    List<GarmentExternalPurchaseOrderViewModel> viewModel = mapper.Map<List<GarmentExternalPurchaseOrderViewModel>>(Data);

            //    return Ok(new
            //    {
            //        apiVersion = ApiVersion,
            //        statusCode = General.OK_STATUS_CODE,
            //        message = General.OK_MESSAGE,
            //        data = Data,
            //        info = new Dictionary<string, object>
            //    {
            //        { "count", Data. },
            //    },
            //    });
            //}
            //catch (Exception e)
            //{
            //    Dictionary<string, object> Result =
            //        new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
            //        .Fail();
            //    return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            //}

            try
            {
              identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var Data = facade.ReadItemByEPONoSimply(keyword, Filter, supplierId, currencyId);

                var viewModel = mapper.Map<List<GarmentExternalPurchaseOrderViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(
                    viewModel.AsQueryable().Select(s => new
                    {
                        s.Id,
                        s.Category,
                        s.OrderDate,
                        s.EPONo,
                        s.Supplier,
                        s.IsOverBudget,
                        s.IsApproved,
                        Items = s.Items.Select(i => new
                        {
                            i.PRNo,
                        }),
                        s.CreatedBy,
                        s.IsPosted,
                        s.LastModifiedUtc
                    }).ToList()
                );

                var info = new Dictionary<string, object>
                    {
                        { "count", listData.Count },
                        { "total", Data.Item2 },
                        { "order", Data.Item3 },
                        { "page", page },
                        { "size", size }
                    };

                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(listData, info);
                return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("unit-do/by-ro")]
        public IActionResult UnitDOByRO(string keyword = null, string Filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var Data = facade.ReadItemForUnitDOByRO(keyword, Filter);

                var newData = mapper.Map<List<GarmentExternalPurchaseOrderItemViewModel>>(Data);

                List<object> listData = new List<object>();
                listData.AddRange(
                    newData.AsQueryable().Select(i => new
                    {
                        i.Id,
                        i.RONo,
                        i.Article,
                    }).ToList()
                );

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = listData,
                    info = new Dictionary<string, object>
                    {
                        { "count", listData.Count },
                    },
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("by-po-serial-number-loader")]
        public IActionResult ByPOSerialNumberLoader(int page = 1, int size = 25, string order = "{}", string keyword = null, string Filter = "{}")
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            try
            {
                var result = facade.ReadItemByPOSerialNumberLoader(keyword, Filter);
                Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                        .Ok(result);
                return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("by-ro-loader")]
        public IActionResult ByROLoader(int page = 1, int size = 25, string order = "{}", string keyword = null, string Filter = "{}")
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            try
            {
                var result = facade.ReadItemByROLoader(keyword, Filter);
                Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                        .Ok(result);
                return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("subcon-delivery-loader")]
        public IActionResult SubconDeliveryLoader(int size = 10, string keyword = null, string Filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var result = facade.ReadEPOForSubconDeliveryLoader(keyword, Filter, size);
                Dictionary<string, object> Result =
                        new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                        .Ok(result);
                return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

        [HttpGet("by-epo-no-many")]
        public IActionResult ByEPONoMany([FromBody] string EPONo)
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var Data = facade.ReadEPONoMany(EPONo);
                var viewModel = mapper.Map<List<GarmentExternalPurchaseOrderViewModel>>(Data);

                List<object> listData = new List<object>();
                listData.AddRange(
                    viewModel.AsQueryable().Select(s => new
                    {
                        s.EPONo,
                        s.Supplier,
                    }).ToList()
                );
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.OK_STATUS_CODE, General.OK_MESSAGE)
                    .Ok(listData);
                return Ok(Result);
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
    }
}
