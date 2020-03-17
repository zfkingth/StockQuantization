using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stock.WebAPI
{
    public class MyMappingProfile : Profile
    {
        public MyMappingProfile()
        {
            CreateMap<Model.Securities, Model.Securities>();
             
            CreateMap<Model.Price, Model.Price>();
                  }
    }

}
