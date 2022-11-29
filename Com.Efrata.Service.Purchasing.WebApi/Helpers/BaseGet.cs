using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.WebApi.Helpers
{
    public class BaseGet<TFacade> : Controller
        where TFacade : IReadable
    {
        private readonly TFacade facade;

        public BaseGet(TFacade facade)
        {
            this.facade = facade;
        }

        public ActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            Tuple<List<object>, int, Dictionary<string, string>> Data = this.facade.Read(page, size, order, keyword, filter);

            return Ok(new
            {
                apiVersion = "1.0.0",
                data = Data.Item1,
                info = new Dictionary<string, object>
                {
                    { "count", Data.Item1.Count },
                    { "total", Data.Item2 },
                    { "order", Data.Item3 },
                    { "page", page },
                    { "size", size }
                },
                message = General.OK_MESSAGE,
                statusCode = General.OK_STATUS_CODE
            });
        }
    }
}
