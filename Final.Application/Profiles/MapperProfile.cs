using AutoMapper;
using Final.Application.Dtos.BasketDtos;
using Final.Application.Dtos.CategoryDtos;
using Final.Application.Dtos.DlcDtos;
using Final.Application.Dtos.GameDtos;
using Final.Application.Dtos.SettingsDto;
using Final.Application.Dtos.UserDtos;
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

            CreateMap<Game, GameReturnDto>()
                .ForMember(dest => dest.DlcNames, opt => opt.MapFrom(src => src.Dlcs.Select(d => d.Name)))
                .ForMember(dest => dest.DlcId, opt => opt.MapFrom(src => src.Dlcs.Any() ? src.Dlcs.FirstOrDefault().Id : 0))

                .ForMember(dest => dest.ImgUrl, opt => opt.MapFrom(src => url + "uploads/images/" + src.ImgUrl));


            // Mapping from GameUpdateDto to Game entity (for update)
            CreateMap<GameUpdateDto, Game>()
                .ForMember(dest => dest.ImgUrl, opt => opt.Ignore()); // ImgUrl handled separately

            // Mapping from Game entity to GameUpdateDto (for fetching data to edit form)
            CreateMap<Game, GameUpdateDto>()
                .ForMember(dest => dest.File, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));





            //category
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<Category, CategoryReturnDto>()
                .ForMember(dest => dest.GameNames, opt => opt.MapFrom(src => src.Games.Select(g => g.Title).ToList()));
            CreateMap<CategoryUpdateDto, Category>()
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            //user
            CreateMap<User, UserReturnDto>();
            CreateMap<RegisterDto, User>();

            //dlc
            CreateMap<DlcCreateDto, Dlc>();
            CreateMap<Dlc, DlcReturnDto>();
            CreateMap<DlcUpdateDto, Dlc>();

            //basket
            CreateMap<Basket, BasketDto>()
                .ForMember(dest => dest.Games, opt => opt.MapFrom(src => src.BasketGames));

            CreateMap<BasketGame, BasketGameDto>()
                .ForMember(dest => dest.GameName, opt => opt.MapFrom(src => src.Game.Title))
                .ForMember(dest => dest.GamePrice, opt => opt.MapFrom(src => src.Game.Price));


            //settings

            CreateMap<Setting, SettingsReturnDto>();

            CreateMap<SettingCreateDto, Setting>();
            CreateMap<SettingUpdateDto, Setting>();







            ////student
            //CreateMap<StudentCreateDto, Student>()
            //    .ForMember(s => s.FileName, map => map.MapFrom(d => d.File.Save(Directory.GetCurrentDirectory(), "uploads/images/")));

            //CreateMap<Student, StudentReturnDto>()
            //    .ForMember(d => d.FileName, map => map.MapFrom(d => url + "uploads/images/" + d.FileName));
        }
    }
}
