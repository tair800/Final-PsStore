namespace Final.Application.Dtos.ActivityDto
{
    public class ActivityDto
    {
        public string ActionType { get; set; }
        public string EntityName { get; set; }
        public string Details { get; set; }
        public DateTime Timestamp { get; set; }
        public string PerformedBy { get; set; }
    }
}
