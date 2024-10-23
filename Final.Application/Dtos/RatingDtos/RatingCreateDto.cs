namespace Final.Application.Dtos.RatingDtos
{
    public class RatingCreateDto
    {
        public int GameId { get; set; }
        public string UserId { get; set; }
        public int Score { get; set; }
    }
}
