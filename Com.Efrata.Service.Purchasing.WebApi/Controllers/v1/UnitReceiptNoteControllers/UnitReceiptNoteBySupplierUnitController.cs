using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.UnitReceiptNoteViewModel;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.UnitReceiptNoteControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/unit-receipt-notes/by-supplier-unit")]
    [Authorize]
    public class UnitReceiptNoteBySupplierUnitController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper _mapper;
        private readonly IUnitReceiptNoteFacade _facade;
        private readonly IdentityService identityService;

        public UnitReceiptNoteBySupplierUnitController(IMapper mapper, IUnitReceiptNoteFacade facade, IServiceProvider serviceProvider)
        {
            _mapper = mapper;
            _facade = facade;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet]
        public IActionResult GetBySupplierUnit(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            var Data = _facade.ReadBySupplierUnit(page, size, order, keyword, filter);

            var newData = _mapper.Map<List<UnitReceiptNoteViewModel>>(Data.Data);

            List<object> listData = new List<object>();
            listData.AddRange(
                newData.Select(s => new
                {
                    s._id,
                    s.no,
                    s.date,
                    s.supplier,
                    s.doId,
                    s.doNo,
                    unit = new
                    {
                        division = new { s.unit.division.name },
                        s.unit.name
                    },
                    items = s.items.Select(i => {
                        i.categoryCode = _facade.GetPurchaseRequestCategoryCode(i.prId);
                        return i;
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
                    { "total", Data.TotalData },
                    { "order", Data.Order },
                    { "page", page },
                    { "size", size }
                },
            });
        }
    }
}
