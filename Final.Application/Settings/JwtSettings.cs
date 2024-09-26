namespace Final.Application.Settings  // Correct namespace
{
    public class JwtSettings  // This is the class definition, not the namespace
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
