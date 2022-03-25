using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Lingo.Domain
{
    //DO NOT TOUCH THIS FILE!
    public class User : IdentityUser<Guid>
    {
        [Required]
        public string NickName { get; set; }

        public int Score { get; set; }

        public int Rank { get; set; }

        public User()
        {
            Rank = int.MaxValue;
            Score = 0;
        }
    }
}
