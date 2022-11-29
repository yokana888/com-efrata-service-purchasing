using Com.Efrata.Service.Purchasing.Lib.Facades.GarmentClosingDateFacades;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentClosingDateModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.Efrata.Service.Purchasing.Test.DataUtils.GarmentClosingDateDataUtils
{
    public class GarmentClosingDateDataUtil
    {
        private readonly GarmentClosingDateFacade facade;

        public GarmentClosingDateDataUtil(GarmentClosingDateFacade facade)
        {
            this.facade = facade;
        }

        public GarmentClosingDate GetNewData()
        {

            return new GarmentClosingDate
            {
                CloseDate=DateTimeOffset.Now
            };
        }

        public GarmentClosingDate CopyData(GarmentClosingDate data)
        {
            return new GarmentClosingDate
            {
                UId = data.UId,

                Id = data.Id,
                Active = data.Active,
                CreatedUtc = data.CreatedUtc,
                CreatedBy = data.CreatedBy,
                CreatedAgent = data.CreatedAgent,
                LastModifiedUtc = data.LastModifiedUtc,
                LastModifiedBy = data.LastModifiedBy,
                LastModifiedAgent = data.LastModifiedAgent,
                IsDeleted = data.IsDeleted,
                DeletedUtc = data.DeletedUtc,
                DeletedBy = data.DeletedBy,
                DeletedAgent = data.DeletedAgent,

                CloseDate = data.CloseDate.AddDays(30)

            };
        }

        public async Task<GarmentClosingDate> GetTestData(GarmentClosingDate data = null)
        {
            data = data ?? GetNewData();
            await facade.Create(data, "Unit Test");
            return data;
        }
    }
}
