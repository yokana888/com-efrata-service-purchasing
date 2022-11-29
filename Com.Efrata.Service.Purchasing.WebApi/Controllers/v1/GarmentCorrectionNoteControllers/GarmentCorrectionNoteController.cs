using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using Com.Efrata.Service.Purchasing.WebApi.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentCorrectionNoteViewModel;

namespace Com.Efrata.Service.Purchasing.WebApi.Controllers.v1.GarmentCorrectionNoteControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/garment-correction-notes")]
    [Authorize]
    public class GarmentCorrectionNoteController : Controller
    {
        private string ApiVersion = "1.0.0";
        public readonly IServiceProvider serviceProvider;
        private readonly IMapper mapper;
        private readonly IGarmentCorrectionNoteFacade facade;
        private readonly IdentityService identityService;

        public GarmentCorrectionNoteController(IServiceProvider serviceProvider, IMapper mapper, IGarmentCorrectionNoteFacade facade)
        {
            this.serviceProvider = serviceProvider;
            this.mapper = mapper;
            this.facade = facade;
            identityService = (IdentityService)serviceProvider.GetService(typeof(IdentityService));
        }

        [HttpGet]
        public IActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            try
            {
                identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;

                var Data = facade.Read(page, size, order, keyword, filter);

                var viewModel = mapper.Map<List<GarmentCorrectionNoteViewModel>>(Data.Item1);

                List<object> listData = new List<object>();
                listData.AddRange(
                    viewModel.AsQueryable()
                        .Select(s => new
                        {
                            s.Id,
                            s.CorrectionNo,
                            s.CorrectionDate,
                            s.CorrectionType,
                            s.Supplier,
                            s.DONo,
                            s.UseIncomeTax,
                            s.UseVat,
                            s.CreatedBy,
                            s.LastModifiedUtc
                        })
                        .ToList()
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
    }
}
