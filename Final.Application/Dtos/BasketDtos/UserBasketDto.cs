namespace Final.Application.Dtos.BasketDtos
{
    public class UserBasketDto
    {

        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public List<BasketGameDto> BasketGames { get; set; }
    }
}
