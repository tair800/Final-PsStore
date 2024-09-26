using AutoMapper;
using Final.Application.Dtos.BasketDtos;
using Final.Application.Dtos.CategoryDtos;
using Final.Application.Dtos.DlcDtos;
using Final.Application.Dtos.GameDtos;
using Final.Application.Dtos.SettingsDto;
using Final.Application.Dtos.UserDtos;
using Final.Application.Dtos.WishlistDtos;
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
                .ForMember(dest => dest.DlcNames, opt => opt.MapFrom(src => src.Dlcs.Select(dlc => new DlcReturnDto
                {
                    Id = dlc.Id,
                    Name = dlc.Name,
                    Price = (int)dlc.Price,
                    ImgUrl = dlc.Image  // Ensure proper mapping from Dlc.Image to DlcReturnDto.ImgUrl
                }).ToList()))
                .ForMember(dest => dest.ImgUrl, opt => opt.MapFrom(src => url + "uploads/images/" + src.ImgUrl));  // Correctly map the main game image



            CreateMap<GameUpdateDto, Game>()
                .ForMember(dest => dest.ImgUrl, opt => opt.Ignore());

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

            CreateMap<UpdateUserDto, User>()
           .ForMember(dest => dest.Email, opt => opt.Ignore());

            //dlc
            CreateMap<DlcCreateDto, Dlc>();
            CreateMap<Dlc, DlcReturnDto>();
            CreateMap<DlcUpdateDto, Dlc>();

            //basket

            // Mapping BasketGame to BasketGameDto
            CreateMap<Basket, UserBasketDto>()
       .ForMember(dest => dest.BasketGames, opt => opt.MapFrom(src => src.BasketGames));


            CreateMap<BasketGame, BasketGameDto>()
      .ForMember(dest => dest.GameTitle, opt => opt.MapFrom(src => src.Game.Title))
      .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.GameId))
      .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Game.Price))  // Map from Game.Price
      .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));





            //settings

            CreateMap<Setting, SettingsReturnDto>();

            CreateMap<SettingCreateDto, Setting>();
            CreateMap<SettingUpdateDto, Setting>();


            //wishlist
            CreateMap<Wishlist, UserWishlistDto>()
               .ForMember(dest => dest.WishlistGames, opt => opt.MapFrom(src => src.WishlistGames));

            // Mapping for WishlistGame -> WishlistGameDto
            CreateMap<WishlistGame, WishlistGameDto>()
                .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.Game.Id))
                .ForMember(dest => dest.GameTitle, opt => opt.MapFrom(src => src.Game.Title))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Game.Price));




            ////student
            //CreateMap<StudentCreateDto, Student>()
            //    .ForMember(s => s.FileName, map => map.MapFrom(d => d.File.Save(Directory.GetCurrentDirectory(), "uploads/images/")));

            //CreateMap<Student, StudentReturnDto>()
            //    .ForMember(d => d.FileName, map => map.MapFrom(d => url + "uploads/images/" + d.FileName));
        }
    }
}
