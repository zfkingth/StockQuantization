using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stock.JQData
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Model.Securities, Model.Securities>();
             
            CreateMap<Model.Price, Model.Price>();
                  }
    }

}
