using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TPBack.Models;
using TPBack.Models.DTO;

namespace TPBack.Mapper
{
    public class TPMappings : Profile
    {

        public TPMappings()
        {
            CreateMap<Book, BookDTO>().ReverseMap();
        }

    }
}
