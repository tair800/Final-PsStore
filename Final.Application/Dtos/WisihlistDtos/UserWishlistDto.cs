﻿using Final.Application.Dtos.WisihlistDtos;

namespace Final.Application.Dtos.WishlistDtos
{

    public class UserWishlistDto
    {
        public string UserId { get; set; }
        public List<WishlistGameDto> WishlistGames { get; set; }
    }

}


