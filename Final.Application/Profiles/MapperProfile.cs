using AutoMapper;
using Final.Application.Dtos.CategoryDtos;
using Final.Application.Dtos.GameDtos;
using Final.Application.Extensions;
using Final.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace Final.Application.Profiles
{
    public class MapperProfile : Profile
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public MapperProfile(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            var request = _contextAccessor.HttpContext.Request;
            var urlBuilder = new UriBuilder(
                request.Scheme,
                request.Host.Host,
                request.Host.Port.Value
                );
            var url = urlBuilder.Uri.AbsoluteUri;

            //game
            CreateMap<GameCreateDto, Game>()
                .ForMember(s => s.ImgUrl, map => map.MapFrom(d => d.ImgUrl.Save(Directory.GetCurrentDirectory(), "uploads/images/")));



            //category
            CreateMap<CategoryCreateDto, Category>();

            ////student
            //CreateMap<StudentCreateDto, Student>()
            //    .ForMember(s => s.FileName, map => map.MapFrom(d => d.File.Save(Directory.GetCurrentDirectory(), "uploads/images/")));

            //CreateMap<Student, StudentReturnDto>()
            //    .ForMember(d => d.FileName, map => map.MapFrom(d => url + "uploads/images/" + d.FileName));
        }
    }
}
