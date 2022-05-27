using System.ComponentModel.DataAnnotations;

namespace Social_Media_v3.Models
{
    public class User_v3
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(18, ErrorMessage = "Name length can't be more than 18.")]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        
        [Required]
        public string Password { get; set; }

        public string? Bio { get; set; }

        public string? From { get; set; }
       // public string? To { get; set; }

        public string? MessageTitle { get; set; }
        public string? MessageText { get; set; }

    }
}