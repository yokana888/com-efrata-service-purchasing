using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.WebApi.Helpers
{
    public class BaseDelete<TFacade> : Controller
        where TFacade : IDeleteable
    {
        private readonly TFacade facade;
        private readonly string apiVersion;

        public BaseDelete(TFacade facade, string apiVersion)
        {
            this.facade = facade;
            this.apiVersion = apiVersion;
        }

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                int Result = await facade.Delete(id);

                if (Result.Equals(0))
                {
                    Dictionary<string, object> ResultNotFound =
                       new ResultFormatter(apiVersion, General.NOT_FOUND_STATUS_CODE, General.NOT_FOUND_MESSAGE)
                       .Fail();
                    return NotFound(ResultNotFound);
                }

                return NoContent();
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(apiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
    }
}
