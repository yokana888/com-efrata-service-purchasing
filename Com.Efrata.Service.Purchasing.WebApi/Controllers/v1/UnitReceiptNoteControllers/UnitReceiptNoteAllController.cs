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
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.UnitReceiptNoteControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/unit-receipt-notes/all")]
    [Authorize]
    public class UnitReceiptNoteAllController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper _mapper;
        private readonly IUnitReceiptNoteFacade _facade;
        private readonly IdentityService identityService;

        public UnitReceiptNoteAllController(IMapper mapper, IUnitReceiptNoteFacade facade, IServiceProvider serviceProvider)
        {
            _mapper = mapper;
            _facade = facade;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet]
        public IActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            var Data = _facade.Read(page, size, order, keyword, filter);

            var newData = _mapper.Map<List<UnitReceiptNoteViewModel>>(Data.Data);

            List<object> listData = new List<object>();
            listData.AddRange(
                newData.AsQueryable().Select(s => new
                {
                    s._id,
                    s.no,
                    s.date,
                    s.supplier,
                    s.doNo,
                    unit = new
                    {
                        division = new { s.unit.division.name },
                        s.unit.name
                    },
                    s.items
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

        [HttpGet("by-no")]
        public IActionResult GetByNo(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            var Data = _facade.ReadByNoFiltered(page, size, order, keyword, filter);

            var newData = _mapper.Map<List<UnitReceiptNoteViewModel>>(Data.Data);

            List<object> listData = new List<object>();
            listData.AddRange(
                newData.AsQueryable().Select(s => new
                {
                    s._id,
                    s.no,
                    s.date,
                    s.supplier,
                    s.doNo,
                    unit = new
                    {
                        division = new { s.unit.division.name },
                        s.unit.name
                    },
                    s.items
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

        [HttpGet("by-list-of-no")]
        public IActionResult GetByListOfNo([Bind(Prefix = "urnNoList[]")]List<string> urnNoList)
        {
            //identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            var Data = _facade.GetByListOfNo(urnNoList);

            var newData = _mapper.Map<List<UnitReceiptNoteViewModel>>(Data);

            return Ok(new
            {
                apiVersion = ApiVersion,
                statusCode = General.OK_STATUS_CODE,
                message = General.OK_MESSAGE,
                data = newData
            });
        }

        [HttpPost("subledger")]
        public async Task<IActionResult> GetForSubLedger([FromBody] List<string> urnNoes)
        {
            //identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            try
            {
                var Data = await _facade.GetUnitReceiptNoteForSubledger(urnNoes);

                //var newData = _mapper.Map<List<UnitReceiptNoteViewModel>>(Data);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    statusCode = General.OK_STATUS_CODE,
                    message = General.OK_MESSAGE,
                    data = Data
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
    }
}
