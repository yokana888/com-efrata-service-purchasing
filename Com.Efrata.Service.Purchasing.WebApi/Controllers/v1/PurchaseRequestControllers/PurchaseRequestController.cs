using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.PurchaseRequestViewModel;
using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.PurchaseRequestModel;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.Efrata.Service.Purchasing.Lib.Facades;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using Com.Efrata.Service.Purchasing.Lib.Services;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.PurchaseRequestControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/purchase-requests")]
    [Authorize]

    public class PurchaseRequestController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper _mapper;
        private readonly PurchaseRequestFacade _facade;
        private readonly IdentityService identityService;

        public PurchaseRequestController(IMapper mapper, PurchaseRequestFacade facade, IdentityService identityService)
        {
            _mapper = mapper;
            _facade = facade;
            this.identityService = identityService;
        }

        [HttpGet]
        public IActionResult GetAllData(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            try
            {
                var Data = _facade.Read(page, size, order, keyword, filter);

                var newData = _mapper.Map<List<PurchaseRequestViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(
                    newData.AsQueryable().Select(s => new
                    {
                        s._id,
                        s.no,
                        s.date,
                        s.expectedDeliveryDate,
                        unit = new
                        {
                            division = new { s.unit.division.name },
                            s.unit.name
                        },
                        category = new { s.category.name },
                        s.isPosted,
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
                        { "total", Data.Item2 },
                        { "order", Data.Item3 },
                        { "page", page },
                        { "size", size }
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

        //[HttpGet]
        //public IActionResult Get()
        //{
        //    /* TODO API Result */

        //    /* Dibawah ini hanya dummy */

        //    return Ok(_mapper.Map<List<PurchaseRequestViewModel>>(_facade.Read()));
        //}

        //[HttpGet("{id}")]
        //public IActionResult Get(int id)
        //{
        //    /* TODO API Result */

        //    /* Dibawah ini hanya dummy */

        //    return Ok(_mapper.Map<PurchaseRequestViewModel>(_facade.ReadById(id)));
        //}

        //[HttpPost]
        //public IActionResult Post([FromBody]PurchaseRequestViewModel vm)
        //{
        //    PurchaseRequest m = _mapper.Map<PurchaseRequest>(vm);

        //    int Result = _facade.Create(m);

        //    /* TODO API Result */

        //    /* Dibawah ini hanya dummy */

        //    if (Result.Equals(0))
        //    {
        //        return StatusCode(500);
        //    }
        //    else
        //    {
        //        return Ok();
        //    }
        //}

        [HttpPost("post")]
        public IActionResult PRPost([FromBody]List<PurchaseRequestViewModel> ListPurchaseRequestViewModel)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
            try
            {
                _facade.PRPost(
                    ListPurchaseRequestViewModel.Select(vm => _mapper.Map<PurchaseRequest>(vm)).ToList(), identityService.Username
                );

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }

        [HttpPut("unpost/{id}")]
        public IActionResult PRUnpost([FromRoute]int id)
        {
            identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

            try
            {
                _facade.PRUnpost(id, identityService.Username);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE);
            }
        }

        //[HttpGet("posted")]
        //public IActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        //{
        //    identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
        //    Tuple<List<object>, int, Dictionary<string, string>> Data = _facade.ReadModelPosted(page, size, order, keyword, filter);

        //    return Ok(new
        //    {
        //        apiVersion = "1.0.0",
        //        statusCode = General.OK_STATUS_CODE,
        //        message = General.OK_MESSAGE,
        //        data = Data.Item1,
        //        info = new Dictionary<string, object>
        //    {
        //        { "count", Data.Item1.Count },
        //        { "total", Data.Item2 },
        //        { "order", Data.Item3 },
        //        { "page", page },
        //        { "size", size }
        //    },
        //    });

        //}
        [HttpGet("posted")]
        public IActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            var Data = _facade.ReadModelPosted(page, size, order, keyword, filter);

            var newData = _mapper.Map<List<PurchaseRequestViewModel>>(Data.Item1);

            List<object> listData = new List<object>();
            listData.AddRange(
                newData.AsQueryable().Select(s => new
                {
                    s._id,
                    s.no,
                    s.date,
                    s.expectedDeliveryDate,
                    unit = new
                    {
                        division = new { s.unit.division.name, s.unit.division._id, s.unit.division.code },
                        s.unit.name,
                        s.unit._id,
                        s.unit.code
                    },
                    category = new
                    {
                        s.category._id,
                        s.category.code,
                        s.category.name
                    },
                    budget = new
                    {
                        s.budget._id,
                        s.budget.name,
                        s.budget.code
                    },
                    s.isPosted,
                    s.remark,
                    s.items,
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
                    { "total", Data.Item2 },
                    { "order", Data.Item3 },
                    { "page", page },
                    { "size", size }
                },
            });
        }

    }
}
