using AutoMapper;
using Com.Efrata.Service.Purchasing.Lib.Models.GarmentClosingDateModels;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentClosingDateViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Efrata.Service.Purchasing.Lib.AutoMapperProfiles
{
    public class GarmentClosingDateProfile : Profile
    {
        public GarmentClosingDateProfile()
        {
            CreateMap<GarmentClosingDate, GarmentClosingDateViewModel>()
              .ReverseMap();
        }
    }
}
