namespace Final.Application.Dtos.UserDtos
{
    public class SaveCardDto
    {
        public string CardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string CVC { get; set; }
    }

}
