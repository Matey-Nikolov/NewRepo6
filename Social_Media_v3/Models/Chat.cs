namespace Social_Media_v3.Models
{
    public class Chat
    {
        public string? MessageTitle { get; set; }
        public string? MessageText { get; set; }

        public User_v3 Users { get; set; }
    }
}