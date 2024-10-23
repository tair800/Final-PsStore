using AutoMapper;
using Final.Application.Dtos.BasketDtos;
using Final.Application.Dtos.CategoryDtos;
using Final.Application.Dtos.CommentDtos;
using Final.Application.Dtos.DlcDtos;
using Final.Application.Dtos.GameDtos;
using Final.Application.Dtos.OrderDtos;
using Final.Application.Dtos.PromoDtos;
using Final.Application.Dtos.RatingDtos;
using Final.Application.Dtos.SettingsDto;
using Final.Application.Dtos.UserDtos;
using Final.Application.Dtos.WishlistDtos;
using Final.Application.Dtos.WisihlistDtos;
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
                    GameTitle = dlc.Game.Title,
                    GameId = dlc.GameId,
                    Name = dlc.Name,
                    Price = (int)dlc.Price,
                    Image = dlc.Image,
                    CreatedDate = dlc.CreatedDate,
                }).ToList()))
                .ForMember(dest => dest.ImgUrl, opt => opt.MapFrom(src => url + "uploads/images/" + src.ImgUrl));



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

            CreateMap<DlcUpdateDto, Dlc>()
                .ForMember(dest => dest.Image, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.Ignore());



            //basket

            CreateMap<Basket, UserBasketDto>()
            .ForMember(dest => dest.BasketGames, opt => opt.MapFrom(src => src.BasketGames));


            CreateMap<BasketGame, BasketGameDto>()
            .ForMember(dest => dest.GameTitle, opt => opt.MapFrom(src => src.Game.Title))
            .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.GameId))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Game.Price))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));





            //settings

            CreateMap<Setting, SettingsReturnDto>();
            CreateMap<SettingCreateDto, Setting>();
            CreateMap<SettingUpdateDto, Setting>();


            //wishlist
            CreateMap<Wishlist, UserWishlistDto>()
               .ForMember(dest => dest.WishlistGames, opt => opt.MapFrom(src => src.WishlistGames));

            CreateMap<WishlistGame, WishlistGameDto>()
                .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.Game.Id))
                .ForMember(dest => dest.GameTitle, opt => opt.MapFrom(src => src.Game.Title))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Game.Price))
                .ForMember(dest => dest.SalePrice, opt => opt.MapFrom(src => src.Game.SalePrice));


            //comment



            CreateMap<CommentCreateDto, Comment>();

            CreateMap<CommentUpdateDto, Comment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.GameId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<Comment, CommentReturnDto>();


            CreateMap<Comment, CommentReturnDto>()
          .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
          .ForMember(dest => dest.GameTitle, opt => opt.MapFrom(src => src.Game.Title));

            CreateMap<CommentHistory, CommentHistoryDto>()
          .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.PreviousContent));

            //order
            CreateMap<Order, OrderDto>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
           .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
           .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.CreatedDate))
           .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            // Map OrderItem -> OrderItemDto
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.GameId))
                .ForMember(dest => dest.GameTitle, opt => opt.MapFrom(src => src.Game.Title))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            // Map OrderDto -> OrderReturnDto
            CreateMap<OrderDto, OrderReturnDto>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems)); // Ensure nested mapping is done correctly


            //promo

            CreateMap<PromoCreateDto, Promo>();
            CreateMap<PromoUpdateDto, Promo>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Promo, PromoReturnDto>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => url + "uploads/images/" + src.Image))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            //map
            CreateMap<RatingCreateDto, Rating>();
            CreateMap<Rating, RatingReturnDto>();

        }
    }
}
