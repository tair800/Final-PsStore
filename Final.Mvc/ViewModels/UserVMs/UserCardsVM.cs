namespace Final.Mvc.ViewModels.UserVMs
{
    public class UserCardsVM
    {
        public List<SavedCardVM> SavedCards { get; set; } = new List<SavedCardVM>();

        public class SavedCardVM
        {
            public int Id { get; set; }
            public string Last4Digits { get; set; }
            public int ExpiryMonth { get; set; }
            public int ExpiryYear { get; set; }
        }
    }
}
